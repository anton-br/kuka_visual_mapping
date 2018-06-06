#!/usr/bin/env python
import rospy
import numpy as np
import sensor_msgs.point_cloud2 as pc2
from sensor_msgs.msg import PointCloud2, PointField, PointCloud
from geometry_msgs.msg import PointStamped, Point
from cv_bridge import CvBridge, CvBridgeError
import cv2
import numpy as np
import tf
import tf2_ros
from nav_msgs.msg import Odometry
from geometry_msgs.msg import Twist
from sensor_msgs.msg import Image
import pcl

bridge = CvBridge()

def odom_callback(odom_data):
    print(type(odom_data.twist.twist.linear.x))
    pass
def image_callback(image):
    try:
   	cv2_img = bridge.imgmsg_to_cv2(image, 'bgr8')
    except CvBridgeError as e:
	print(e)
    else:
	cv2.imwrite('blablba.png', cv2_img)
	print(cv2_img[:10])

import time
def point_callback(point):
    points_list = []

    print('new cycle')
    for data in pc2.read_points(point, skip_nans=False):
        points_list.append([data[0], data[1], data[2]])
    print('complete to reading points')

    new_point = pcl.PointCloud(points_list)
    cloud = new_point.make_voxel_grid_filter()
    a = 0.044
    cloud.set_leaf_size(a, a, a)
    new_filt = cloud.filter()
    print('points was filtered')
    list_of_points = []
    save_list = []
    for filt in new_filt.to_array():
        pcl_cloud = PointStamped()
        pcl_cloud.header = point.header
        pcl_cloud.header.stamp = rospy.Time.now()
        pcl_cloud.point = Point(*filt)
        save_list.append([filt[0], filt[1], filt[2]])
        list_of_points.append(pcl_cloud)
    print('create massive of points \nnum of points: ' + str(len(list_of_points)))

    rate = rospy.Rate(10)
    listener = tf.TransformListener()
    start_frame = "/camera_depth_optical_frame"
    goal_frame = "/base_laser_front_link"

    print('started transformation')
    listener.waitForTransform(start_frame, goal_frame, rospy.Time(0), rospy.Duration(20.0))
    while not rospy.is_shutdown():
        new_cloud = []
        try:
            now = rospy.Time(0)#rospy.Time.now()
            listener.waitForTransform(start_frame, goal_frame, now, rospy.Duration(20.0))

            for point in list_of_points:
                pt = listener.transformPoint(goal_frame, point).point
                new_cloud.append([pt.x, pt.y, pt.z])

        except (tf.Exception, tf.LookupException, tf.ConnectivityException) as e:
            print('failed:, ', e)
            return
        else:
            print('complete!')
            np.save('new_cloud.npy', np.array(new_cloud))
            print(len(new_cloud))
            rospy.signal_shutdown("Quit")
        rate.sleep()


def listener():
    rospy.init_node('listener', anonymous=True)
    rospy.Subscriber("/camera/depth/points", PointCloud2, point_callback)

    rospy.spin()

if __name__ == '__main__':
    listener()
