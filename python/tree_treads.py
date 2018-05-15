# -*- coding: utf-8 -*-
import random
import time
from threading import Thread

from sklearn.ensemble import RandomForestClassifier



class TreeThread(Thread):

    def __init__(self, name, targets, train):
        Thread.__init__(self)
        self.name = name
        self.targets = targets
        self.train = train
        self.end = False
        self.tree = None

    def run(self):
        self.end = False
        self.tree = RandomForestClassifier().fit(self.train, self.target)
        self.end = True

def create_thread():
    name = "Thread #%s" % (1)
    targets = 0
    train = 0
    # my_thread = TreeThread(name, targets, train)
    # print('start thread')
    # my_thread.start()
    # while(not my_thread.end):
    #     pass
    # print(my_thread.tree)
if __name__ == "__main__":
    create_thread()
