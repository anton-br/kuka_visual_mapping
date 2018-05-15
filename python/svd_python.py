import os
import sys
import json
import requests

import pickle
import cv2
import time
import keras
import operator
import numpy as np
import keras.applications as zoo
from PIL import Image, ImageFilter
from sklearn.externals import joblib
from tqdm import tqdm_notebook as tqn
from collections import Counter, defaultdict
from sklearn.ensemble import RandomForestClassifier

keras.backend.set_learning_phase(0)
def init_keras_model():
    model = zoo.MobileNet(include_top=True, weights='imagenet')

    inp = model.get_layer(index=0)
    out = model.get_layer(name='conv_dw_4_relu').output
    for layer in model.layers:
        layer.trainable = False

    headless = keras.models.Model(inp.input, out)
    del model

    headless.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy'])
    return headless

model = init_keras_model()

class Requester:
    def __init__(self):
        ready = None
        image = None
        pred_mask = None

def change_status(status='True'):
    #питон готов принимать новую картинку, если status=True, иначе не готов
    ready = {'ready': status}
    ready_js = json.dumps(ready)
    requests.post("http://127.0.0.1:5000/ready/", json=ready_js).json()

def post_answer(mass):
    #готов ответ
    val = {'perm_mass': mass.tolist()}
    val_js = json.dumps(val)
    requests.post("http://127.0.0.1:5000/new_val/", json=val_js).json()

def update_speed(data, global_speed, speed_keys):
    coord_speed = np.array(data['coordSpeed'].replace(',', '.').split(';'))
    coord_speed = list(filter(None, coord_speed))
    coord_speed = np.array(list(map(lambda z: np.array(z.split(), dtype=np.float32), coord_speed)))
    for speed in coord_speed:
        key = tuple(speed[:-1].astype(np.int32))
        global_speed[key].append(speed[-1])
        speed_keys.add(key)

def update_visual(data, signs, global_vis, vis_keys):
    str_vis = np.array(list(map(lambda vis: vis.split(' '), data['coordVis'].split('; ')[:-1])), dtype=np.int32)
    for i, coord in enumerate(str_vis):
        global_vis[tuple(coord)].append(signs[i])
        vis_keys.add(tuple(coord))

def update_train(inter_keys, global_speed, global_vis, global_train, global_target):
    target = np.array(operator.itemgetter(*inter_keys)(global_speed))
    train = np.array(operator.itemgetter(*inter_keys)(global_vis))
    print('before: ', train.shape, inter_keys, target)
    if isinstance(target[0], list):
        target_list = []
        for tar in target:
            w = np.where(np.array(tar)>0.08)
            if len(w[0]) == 0: # когда скорость 0, у данной точки будет класс - 2, что наверное не правильно, тк 0 быть не может вообще
                target_list.append(0)
            else:
                target_list.append(np.mean(np.array(tar)[w]))
    else:
        target = np.array([np.mean(target)])
    if len(target) == 0:
        return
    print('after: ', target)
    target_new = []
    for i, tar in enumerate(target):
        if tar > .17:
            tar = 1
        else:
            tar = 2
        if len(train.shape) > 1:
            num = [tar] * train.shape[0]
        else:
            num = [tar]*len(train[i]) if isinstance(train[i], list) else [tar]
            train = np.concatenate(train)
        target_new.append(num)

    target = np.concatenate(target_new)
    print('target shape: ', target.shape)
    global_target.append(target)
    global_train.append(train)

def load_image(robot_num=25, width=224, height=224, quality=150, path_img=None, a=0, mode='bin'):
    if path_img is not None:
         img = Image.open(path_img)
         img = np.array(img.resize((224, 224)))
    else:
        cap = cv2.VideoCapture()
        cap.open('http://192.168.88.%i:8080/stream?topic=/camera/rgb/image_raw&width=%i&height=%i&quality=%i'
                 %(int(robot_num), int(width), int(height), int(quality)))
        _, img = cap.read()
    if mode == 'bin':
        img = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
        img[:,:,0] = 0.
        img[:,:,-1] = 255.
        img[:,:,1] = np.array([0. if i < 100. else 255. for i in img[:,:,1].ravel()]).reshape(224, 224)
        img = cv2.cvtColor(img, cv2.COLOR_HSV2BGR)
    else:
        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

    # cv2.imwrite('./images/img/{}_{}.png'.format(a, a), img)
    return img

def get_vis_signs(img, a, mode='bin'):
    img = Image.fromarray(img)
    a += 1
    img = img.filter(ImageFilter.MedianFilter(size=11))
    # img.save('./images/img/{}.png'.format(a))
    if mode == 'bin':
        img = img.resize((28, 28))
        visual_signs = (np.array(img)/255.).copy()
    else:
        img = np.array(img)/255.
        if len(img.shape) == 3:
            img = img.reshape(1, *img.shape)
        visual_signs = model.predict(img)
    return visual_signs

def main():
    change_status('True')
    global_vis = defaultdict(list)
    global_speed = defaultdict(list)
    global_target = []
    global_train = []
    vis_keys = set()
    speed_keys = set()
    flag_tree = 0
    tree = None # будущее дерево
    ind = 0 # индекс чтобы не считывать одни и те же данные несколько раз
    v = 0
    k = 0
    inter = []
    mode = 'bin'

    if mode == 'bin':
        shape = 2
    else:
        shape = 128

    while True:
        new_img = requests.get("http://127.0.0.1:5000/new_image/")#шарп говорит когда начинать обрабатыавть новую картинку
        try:
            data = new_img.json()
        except:
            continue
        # обновляем инфу о скорости, если она есть.
        if data['coordSpeed'] != '':
            if int(data['ind']) >= ind:
                update_speed(data, global_speed, speed_keys)
                ind += 1
                f = open('texts\\speed.txt', 'w')
                f.write(str(global_speed))
                f.close()
        # если шарп готов к получению картинки, получаем её и обрабатываем
        if data['image'] == 'True':
            change_status('False')

            robot_num = int(data['robot_num'])
            width = int(data['width'])
            height = int(data['height'])
            quality = int(data['height'])

            t = time.time()
            img = load_image(path_img='./mirea_images/real_images/V01-14-19-1419.jpg')
            #img = load_image(robot_num=robot_num, width=width, height=height, quality=quality, a=k)

            print("loading: ", time.time() - t)

            #Получем визуальные признаки с изображения
            signs = get_vis_signs(img, k).reshape(-1, shape)

            # Получаем глобальные координаты для этих признаков
            update_visual(data, signs, global_vis, vis_keys)

            # Ищем пересечения между инфой о скорости и визуальными признаками
            inter_keys = vis_keys & speed_keys
            print("Num inter: ", len(inter_keys))

            # Если есть новые пересечения, обновляем выборку для обучения
            if len(inter_keys) != 0:
                inter.append(inter_keys)
                flag_tree = 0
                update_train(inter_keys, global_speed, global_vis, global_train, global_target)

                try:
                    y = np.concatenate(global_target)
                except:
                    print("can't concatenate targets")
                    y = np.array(global_target)
                try:
                    X = np.concatenate(global_train)
                except:
                    print("can't concatenate train")
                    X = np.array(global_train)

                t = time.time()
                tree = RandomForestClassifier().fit(X.reshape(-1, shape), y)
                v += 1
                f = open('texts\\inter.txt', 'a')
                f.write('\n{}'.format(v) + str(inter))
                f.close()

                print('training time: ', time.time() - t)
                f = open('texts\\target.txt', 'a')
                f.write('\n{}'.format(v) + str(y))
                f.close()

                f = open('texts\\train.txt', 'a')
                f.write('\n{}'.format(v) + str(X))
                f.close()

                vis_keys = set()
                speed_keys = set()

                flag_tree += 1
            if tree is not None:
                mass = tree.predict(signs.reshape(-1, shape))
            else:

                # # f = open('texts\\vis.txt', 'w')
                # # f.write(str(vis_keys))
                # # f.close()
                #
                # f = open('texts\\middle_vis.txt', 'w')
                # for i in vis_keys:
                #     if i[0] in (-1, -2, 0, 1, 2):
                #         f.write(str(i) + "\n")
                # f.close()
                #
                with open('data.pickle', 'wb') as f:
                    pickle.dump(global_vis, f)
                # f = open('texts\\vis_cor.txt', 'w')
                # f.write(str(global_vis))
                # f.close()
                #
                # f = open('texts\\speed.txt', 'a')
                # f.write('\n{}'.format(k) + str(global_speed))
                # f.close()
                k+=1

                mass = np.ones(shape=(784, 128), dtype=np.int32) + 1
            post_answer(mass)
            change_status('True')

if __name__ == "__main__":
    sys.exit(main())
