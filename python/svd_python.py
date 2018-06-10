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
from scipy import stats
from multiprocessing import Queue
from threading import Thread
from sklearn.externals import joblib
from scipy import ndimage
import keras.applications as zoo
from PIL import Image, ImageFilter
from sklearn.externals import joblib
from tqdm import tqdm_notebook as tqn
from collections import Counter, defaultdict
from sklearn.ensemble import RandomForestClassifier

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
    rx = np.cos(odom_val[2])
    ry = np.sin(odom_val[2])

    # odom_val[0] += diff_time * odom_val[-1] * ry
    # odom_val[1] += diff_time * odom_val[-1] * rx

    f = open('./hello/vis_{}.txt'.format(ind), 'w')
    o = open('./hello/odom_{}.txt'.format(ind), 'w')
    for i, coord in enumerate(visual_coord):
        coord_x = (coord[0] * rx + coord[1] * ry) + odom_val[0]
        coord_y = (-coord[0] * ry + coord[1] * rx) + odom_val[1]
        coords = tuple((np.round(coord_x*10).astype(np.int32), np.round(coord_y*10).astype(np.int32)))
        f.write(str(coord[0]) + " " + str(coord[1]) + " " + str(signs[i][0]) + "\n")
        global_vis[coords].append(signs[i])
        vis_keys.add(coords)
    o.write(str(odom_val[0]) + " " + str(odom_val[1]) + " " + str(odom_val[2]) + " " + str(odom_val[3]))

    f.close()
    o.close()

## TODO:
"""
1. inter keys == 2
2. target = []
"""
def update_train_bin(inter_keys, new_global_speed, global_vis, global_train, global_target):
    train = np.array(operator.itemgetter(*inter_keys)(global_vis))
    if type(train[0][0]) == np.ndarray:
        train = np.concatenate(train)
    target = np.array(operator.itemgetter(*tuple(map(lambda v: tuple(v), train)))(new_global_speed))
    target_list = []
    print(train, target)
    deleted = []
    for i, tar in enumerate(target):
        if isinstance(tar, (list, np.ndarray)):
            if len(tar) == 0:
                deleted.append(i)
                continue
            tar = np.mean(tar)
        if tar > .17:
            tar = 1
        else:
            tar = 2
        target_list.append(tar)

    train = np.delete(train, deleted, axis=0)
    global_target.append(target_list)
    global_train.append(train)


def update_train(inter_keys, global_speed, global_vis, global_train, global_target,):
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
        if tar > .17:
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

def load_image(queue=None, image_counter=0, mode='bin', path_img=None):
    if path_img is not None:
         img = Image.open(path_img)
         img = np.array(img.resize((224, 224)))
    else:
        img = queue.pop()
    if mode == 'bin':
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

def load_point_cloud(src="./right_points_3"):
    points = np.loadtxt(src)
    return points

def fill_graph(graph, global_vis, tree, acc, shape):
    for key, item in global_vis.items():
        Tx = key[0]
        Ty = key[1]
        xmatrix = np.round(Tx).astype(int)
        ymatrix = np.round(Ty).astype(int)

        if tree is not None:
            pred = tree.predict(np.array(item).reshape(-1, shape))
            it = np.array(item).reshape(-1, shape)[:,0].copy()
            it[it==0] = 2.
            acc.append(len(np.where(it == pred)[0])/len(pred))
            pred = np.round(np.mean(pred))
        else:
            pred = 1
        #
        # #if you want to write answers
        # item = np.array(item)[:,0]
        # counter = Counter(item)
        # if counter[0] * 3 < counter[1]:
        #     pred = 1
        # else:
        #     pred = 2

        graph[int(xmatrix + 90)][int(ymatrix + 90)] = int(pred)

def calculate_speeds(surface_speed, new_global_speed, distance):
    t = time.time()
    surface_odom = np.vstack(np.array(surface_speed)[:,0])
    odom_speed, surface_odom = np.mean(surface_odom[:,-1]), surface_odom[:,:3]
    distance += np.linalg.norm(surface_odom[-1] - surface_odom[-2])
    distance_speed = distance/(t - surface_speed[0][1])
    new_global_speed[tuple(current_surface)].append(np.mean([distance_speed, odom_speed]))
    print('new speed for {} by time and distance is {} and by odometry is {}'
          .format(current_surface, distance_speed, odom_speed))
    return distance

def create_new_surface(new_global_speed):
    surface_speed = []
    current_surface = stats.mode(global_vis[key])[0][0]
    surface_speed.append([odom_val, time.time()])
    surface_odom = np.vstack(np.array(surface_speed)[:,0])
    new_global_speed[tuple(current_surface)].append([np.mean(surface_odom[:,-1])])
    return 0, surface_speed, current_surface

def main():
    post_answer(np.array([]))
    #update_floder()
    global queue
    global_vis = defaultdict(list) # visual features as values and global coordinates as keys
    global_speed = defaultdict(list) #  speed value as values and global coordinates as keys
    new_global_speed = defaultdict(list)
    global_target = [] # all target
    global_train = [] # all training values
    #surface_speed = [] # speed values on one surface
    all_trees = [] # trained trees with more then 80% accuracy
    inter = [] # intersection of vis_keys and speed_keys
    vis_keys = set() # global coordinates of visual features
    speed_keys = set() # global coordinates of speed values
    visual_coord = load_point_cloud() # point cloud
    first = True # flag indicating first image loading
    new_surface = True # flag indicating that new surface is coming
    tree = None # future tree
    image_counter = 0 # help to save data to files
    mode = 'bin' # mode of program work

    if mode == 'bin':
        shape = 3
    else:
        shape = 128

    while True:
        global_time = time.time()
        new_img = requests.get("http://127.0.0.1:5000/odom/") # шарп говорит когда начинать обрабатыавть новую картинку
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

        # TODO:
        """
        1. Переводим одометрию в координаты
        2. Если есть пересечения с визуальными признаками, запоминаем какой был признак и сохраняем скорости в массив + запоминаем текущее время
        3. Пока, при смене одометрии, мы находимся на той же поверхности, собираем данные в один массив
        4. Как только поверхность меняется, к данному визуальному признаку в соответствие ставится средняя скорость прохождения данного участка, с учетом времени
        """

        update_speed(odom_val, global_speed, speed_keys)
        f = open('texts\\speed.txt', 'w')
        for speed in global_speed.keys():
            f.write(str(speed) + ":" + str(global_speed[speed]) + '\n')
        f.close()

        key = tuple(list(map(lambda a: np.round(a * 10).astype(np.int32), odom_val[:2])))
        if new_surface:
            if key not in global_vis.keys():
                print('key not found in global_vis, cant create new surface')
            else:
                new_surface = False
                distance, surface_speed, current_surface = create_new_surface(new_global_speed)
                print('NEW surface was created!, odom is {}, surfase is {}'.format(odom_val, current_surface))
        else:
            if np.sum(stats.mode(global_vis[key])[0][0] == current_surface) == 3:
                surface_speed.append([odom_val, time.time()])
                print('vis was founded!')
                distance = calculate_speeds(surface_speed, new_global_speed, distance)

            elif len(surface_speed) == 1:
                print('too fast we found diff surface')
                new_global_speed[tuple(current_surface)].append(surface_speed[0][-1])

            else:
                print('THE END')
                calculate_speeds(surface_speed, new_global_speed, distance)
                print('goblal speed are: ', new_global_speed)

                distance, surface_speed, current_surface = create_new_surface(new_global_speed)
                print('NEW surface was created!, odom is {}, surfase is {}'.format(odom_val, current_surface))

        # если шарп готов к получению картинки, получаем её и обрабатываем
        if len(queue) == 0:
            continue
        img = load_image(queue, image_counter, mode)

        #Получем визуальные признаки с изображения
        signs = get_vis_signs(img, image_counter).reshape(-1, shape)

        # Получаем глобальные координаты для этих признаков
        update_visual(odom_val, data['time'], visual_coord, signs, global_vis, vis_keys, image_counter)
        inter_keys = vis_keys & speed_keys
        print("Num inter: ", len(inter_keys))
        if len(inter_keys) != 0:
            inter.append(inter_keys)
            if mode == 'bin':
                update_train_bin(inter_keys, new_global_speed, global_vis, global_train, global_target, mode)
            else:
                update_train(inter_keys, global_speed, global_vis, global_train, global_target, mode)

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

        # if tree is not None:
        #     mass = tree.predict(signs.reshape(-1, shape))
        #     a = signs.reshape(-1, shape)[:,0].copy()
        #     print('two: ', len(np.where(a == 0.)[0]), 'mass: ', len(mass))
        #     a[a==0] = 2
        #     print("tree acc: ", len(np.where(a == mass)[0]), " / ",  len(mass))
        # else:
        #     mass = np.ones(shape=(784, 1), dtype=np.int32)

        #real answers
        post = signs.reshape(-1, shape)[:,0].astype(np.int32)
        post[post == 0] = 2

        graph = np.ones((180, 180), dtype=np.int32)
        acc = []
        #fill_graph(graph, global_vis, tree, acc, shape)
        if len(acc) == 0:
            print('Tree is none')
        else:
            print('Tree acc: ', np.mean(acc))
            if  np.mean(acc) > 0.8:
                all_trees.append([tree, len(global_train)])
                print('tree was appended')
            else:
                graph = np.ones((180, 180), dtype=np.int8)
                tree, lenght = all_trees[-1]
                acc_new = []
                fill_graph(graph, global_vis, tree, acc_new, shape)
                print('New acc: ', np.mean(acc_new))
                global_train = global_train[:lenght]
                global_target = global_target[:lenght]

        #TODO вычислять отдельные препятствия и, если рядом есть другое препятствие, оставлять зазор, если он больше длины робота
        struct = ndimage.generate_binary_structure(2, 2)
        graph = graph - 1
        graph = ndimage.binary_dilation(graph, structure=struct, iterations=3).astype(graph.dtype)
        graph = graph + 1
        t = time.time()
        np.savetxt('./hello/graph_{}'.format(image_counter), graph)
        print('time to save: ', time.time() - t)
        image_counter += 1
        post_answer(graph)
        print('iter time: ', time.time() - global_time)
if __name__ == "__main__":
    sys.exit(main())
