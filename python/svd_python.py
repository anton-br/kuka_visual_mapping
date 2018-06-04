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
from multiprocessing import Queue
from threading import Thread
from sklearn.externals import joblib
import keras.applications as zoo
from PIL import Image, ImageFilter
from sklearn.externals import joblib
from tqdm import tqdm_notebook as tqn
from collections import Counter, defaultdict
from sklearn.ensemble import RandomForestClassifier

# TODO 1. Стопорь роботоа и регулируй камеру! +
# 2. Проверить правильность сборки массива для обучения.
# 3. Надо поддреживать репрезентативность выборки, чтобы дерево не предсказывало для всего нули (как стандартную поверхность перемещения).
# 4. Облако точек

queue = []
class LoopThread(Thread):

    def __init__(self, name, cap):
        Thread.__init__(self)
        self.name = name
        self.cap = cap

    def run(self):
        print('i am working')
        global queue
        while(True):
            _, img = self.cap.read()
            queue.append(img)
            try:
                req = requests.get('http://127.0.0.1:5000/odom/')
            except:
                break

def create_driver(number, cap):
    name = 'loop_thread'
    my_thread = LoopThread(name, cap)
    my_thread.start()
    return my_thread

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

def update_speed(odom_val, global_speed, speed_keys):
    key = tuple(list(map(lambda a: np.round(a * 10).astype(np.int32), odom_val[:2])))
    global_speed[key].append(odom_val[-1])
    speed_keys.add(key)

def update_visual(odom_val, speed_time, visual_coord, signs, global_vis, vis_keys, ind):
    # fix delay between resived odom data and images. maybe it is not necessary
    curr_time = time.time()
    diff_time = curr_time - float(speed_time)
    print(diff_time)
    # print(odom_val)
    rx = np.cos(odom_val[2])
    ry = np.sin(odom_val[2])

    # odom_val[0] += diff_time * odom_val[-1] * ry
    # odom_val[1] += diff_time * odom_val[-1] * rx

    f = open('./hello/vis_{}.txt'.format(ind), 'w')
    o = open('./hello/odom_{}.txt'.format(ind), 'w')
    for i, coord in enumerate(visual_coord):
        coord_x = (coord[0] * rx - coord[1] * ry) + odom_val[0]
        coord_y = (coord[0] * ry + coord[1] * rx) + odom_val[1]
        coords = tuple((np.round(coord_x*10).astype(np.int32), np.round(coord_y*10).astype(np.int32)))
        f.write(str(coord[0]) +" "+str(coord[1]) + " " + str(signs[i][0]) + "\n")
        global_vis[coords].append(signs[i])
        vis_keys.add(coords)
    o.write(str(odom_val[0]) + " " + str(odom_val[1]) +" "+ str(odom_val[2]) +" "+ str(odom_val[3]))

    f.close()
    o.close()

def update_train(inter_keys, global_speed, global_vis, global_train, global_target):
    target = np.array(operator.itemgetter(*inter_keys)(global_speed))
    train = np.array(operator.itemgetter(*inter_keys)(global_vis))
    if len(inter_keys) > 1:
        target = np.array(list(target) + [None])[:-1]
        train = np.array(list(train) + [None])[:-1]
    print('before: ', train.shape, inter_keys, target)
    if isinstance(target[0], (list, np.ndarray)):
        target_list = []
        for tar in target:
            w = np.where(np.array(tar)>0.099)[0]
            if len(w) == 0: # когда скорость 0, у данной точки будет класс - 2, что наверное не правильно, тк 0 быть не может вообще
                target_list.append(0)
            else:
                target_list.append(np.min(np.array(tar)[w]))
        target = np.array(target_list)
    else:
        target = np.array([np.mean(target)])
    if len(target) == 0:
        return
    print('after: ', target)
    target_new = []
    for i, tar in enumerate(target):
        if tar > .16:
            tar = 1
        else:
            tar = 2
        if len(train.shape) > 1:
            num = [tar] * train.shape[0]
        else:
            num = [tar]*len(train[i]) if isinstance(train[i], (list, np.ndarray)) else [tar]
        target_new.append(num)
    if len(train.shape) == 1:
        train = np.concatenate(train)
    target = np.concatenate(target_new)
    print('train.shape: ', train.shape, 'target shape: ', target.shape)
    global_target.append(target)
    global_train.append(train)

def load_image(cap=None, robot_num=25, width=224, height=224, quality=150, path_img=None, image_counter=0, mode='bin'):
    if path_img is not None:
         img = Image.open(path_img)
         img = np.array(img.resize((224, 224)))
    else:
        _, img = cap.read()
    if mode == 'bin':
        print(image_counter)
        cv2.imwrite('./images/img/{}_{}.png'.format(image_counter, image_counter), img)
        img = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
        img[:,:,0] = 0.
        img[:,:,-1] = 255.
        img[:,:,1] = np.array([0. if i < 100. else 255. for i in img[:,:,1].ravel()]).reshape(224, 224)
        img = cv2.cvtColor(img, cv2.COLOR_HSV2BGR)
    else:
        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

    # cv2.imwrite('./images/img/{}_{}.png'.format(a, a), img)
    return img

def get_vis_signs(img, image_counter, mode='bin'):
    img = Image.fromarray(img)
    img = img.filter(ImageFilter.MedianFilter(size=11))
    img.save('./images/img/{}.png'.format(image_counter))
    if mode == 'bin':
        img = img.resize((28, 28))
        visual_signs = (np.array(img)/255.).copy()
    else:
        img = np.array(img)/255.
        if len(img.shape) == 3:
            img = img.reshape(1, *img.shape)
        visual_signs = model.predict(img)
    return visual_signs

def update_floder():
    open('texts\\speed.txt', 'w').write('')
    open('texts\\inter.txt', 'a').write('')
    open('texts\\target.txt', 'a').write('')
    open('texts\\train.txt', 'a').write('')
    open('texts\\middle_vis.txt', 'w').write('')

def load_point_cloud(src='./../../../Desktop/new_cloud_lazer.npy'):
    point_cloud = np.load(src)[:,:-1][:-1][::-1]
    shape = point_cloud.shape[0]
    if shape < 784:
        raise Exception('Point cloud have invalid shape. %s < 784.'%shape)
    delete_num = shape - 784
    delete_ind = np.random.randint(1, shape-1, delete_num)
    point_cloud = np.delete(point_cloud, delete_ind, 0)
    point_cloud = np.array(list(map(lambda a: [a[1], a[0]], point_cloud)))
    return point_cloud

# def load_visual_coord(src='./dist_map.txt'):
#     f = open(src, 'r')
#     coord = f.read()
#     f.close()
#     vis = np.array(coord.split(' '), dtype=np.float32).reshape(-1, 2).astype(np.float32) / 100
#     return vis

def main():
    post_answer(np.array([]))
    #update_floder()
    global queue
    global_vis = defaultdict(list)
    global_speed = defaultdict(list)
    global_target = []
    global_train = []
    vis_keys = set()
    speed_keys = set()
    visual_coord = load_point_cloud()#load_visual_coord()
    flag_tree = 0
    first = True
    tree = None # будущее дерево
    ind = 0 # индекс чтобы не считывать одни и те же данные несколько раз
    data_counter = 0
    image_counter = 0
    inter = []
    mode = 'bin'

    if mode == 'bin':
        shape = 3
    else:
        shape = 128

    while True:
        global_time = time.time()
        new_img = requests.get("http://127.0.0.1:5000/odom/")#шарп говорит когда начинать обрабатыавть новую картинку
        try:
            data = new_img.json()
        except:
            continue

        if data['data_odom'] == '':
            continue

        try:
            if first:
                # req_par = requests.get("http://127.0.0.1:5000/new_image/")
                # params = req_par.json()
                # robot_num = int(params['robot_num'].split('.')[-1])
                # width = int(params['width'])
                # height = int(params['height'])
                # quality = int(params['height'])
                robot_num = 21
                width = 224
                height = 224
                quality = 150

            # img = load_image(path_img='./mirea_images/real_images/V01-14-19-1419.jpg')
                cap = cv2.VideoCapture('http://192.168.88.%i:8080/stream?topic=/camera/rgb/image_raw&width=%i&height=%i&quality=%i'
                         %(int(robot_num), int(width), int(height), int(quality)))
                for i in range(15):
                    _ = cap.read()
                thread = create_driver(25, cap)
                first = False
        except:
            continue

        odom_val = np.array(data['data_odom'].replace(',', '.').split(' '))
        odom_val = np.array(list(filter(None, odom_val)), dtype=np.float32)
        # обновляем инфу о скорости, если она есть. Скорость тут, потому что необходимо, чтобы разница между координатами и картинкой была минимальна
        update_speed(odom_val, global_speed, speed_keys)
        f = open('texts\\speed.txt', 'w')
        for speed in global_speed.keys():
            f.write(str(speed) + ":" + str(global_speed[speed]) + '\n')
        f.close()
        # если шарп готов к получению картинки, получаем её и обрабатываем

        if len(queue) == 0:
            continue
        img = queue.pop()
        #print('len queue: ', len(queue))
        cv2.imwrite('./images/img/{}_{}.png'.format(image_counter, image_counter), img)
        img = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
        img[:,:,0] = 0.
        img[:,:,-1] = 255.
        img[:,:,1] = np.array([0. if i < 100. else 255. for i in img[:,:,1].ravel()]).reshape(224, 224)
        img = cv2.cvtColor(img, cv2.COLOR_HSV2BGR)
        #img = load_image(cap, robot_num=robot_num, width=width, height=height, quality=quality, image_counter=image_counter)

        #print("loading: ", time() - t)

        #Получем визуальные признаки с изображения
        signs = get_vis_signs(img, image_counter).reshape(-1, shape)

        #t = time()
        image_counter += 1
            # Получаем глобальные координаты для этих признаков
        update_visual(odom_val, data['time'], visual_coord, signs, global_vis, vis_keys, ind)
        ind +=1
        inter_keys = vis_keys & speed_keys
        print("Num inter: ", len(inter_keys))
        if len(inter_keys) != 0:
            inter.append(inter_keys)
            flag_tree = 0
            update_train(inter_keys, global_speed, global_vis, global_train, global_target)

            try:
                y = np.concatenate(global_target)
            except:
                print("can't concatenate target")
                y = np.array(global_target)
            try:
                X = np.concatenate(global_train)
            except:
                print("can't concatenate train")
                X = np.array(global_train)

            t = time.time()
            ones = list(X[np.where(y == 1)[0][-500:]])
            two = list(X[np.where(y == 2)[0][-500:]])
            X = np.array(ones + two)
            y = np.array([1] * len(ones) + [2] * len(two))
            # np.savetxt('X', X)
            # np.savetxt('y', y)
            np.random.seed(10)
            np.random.shuffle(X)
            np.random.seed(10)
            np.random.shuffle(y)

            tree = RandomForestClassifier(class_weight='balanced').fit(X.reshape(-1, shape), y)
            f = open('texts\\training_data.txt', 'w')
            for y_data, x_data in zip(y, X):
                f.write('ans: ' + str(y_data) + '\n' + 'tr: ' + str(x_data) + '\n')
            f.close()
            vis_keys = vis_keys - inter_keys
            speed_keys = speed_keys - inter_keys

        if tree is not None:
            mass = tree.predict(signs.reshape(-1, shape))
            a = signs.reshape(-1, shape)[:,0].copy()
            print('two: ', len(np.where(a == 0.)[0]), 'mass: ', len(mass))
            a[a==0] = 2
            print("tree acc: ", len(np.where(a == mass)[0]), " / ",  len(mass))
        else:
            mass = np.ones(shape=(784, 1), dtype=np.int32)

        post = signs.reshape(-1, shape)[:,0].astype(np.int32)
        post[post == 0] = 2
        graph = np.ones((180, 180), dtype=np.int32)
        for key, item in global_vis.items():
            Tx = key[0]
            Ty = key[1]
            xmatrix = np.round(Tx).astype(int);
            ymatrix = np.round(Ty).astype(int);
            item = np.array(item)[:,0]
            counter = Counter(item)
            if counter[0] * 3 < counter[1]:
                it = 1
            else:
                it = 2
            graph[int(xmatrix + 90)][int(ymatrix + 90)] = int(it);
            # graph[int(xmatrix + 91)][int(ymatrix + 91)] = int(it);
            # graph[int(xmatrix + 90)][int(ymatrix + 91)] = int(it);
            # graph[int(xmatrix + 91)][int(ymatrix + 90)] = int(it);
            # graph[int(xmatrix + 89)][int(ymatrix + 89)] = int(it);
            # graph[int(xmatrix + 90)][int(ymatrix + 89)] = int(it);
            # graph[int(xmatrix + 89)][int(ymatrix + 90)] = int(it);

        post_answer(graph)
        print('iter time: ', time.time() - global_time)
if __name__ == "__main__":
    sys.exit(main())
