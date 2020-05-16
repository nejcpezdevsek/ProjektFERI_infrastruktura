import numpy as np
import cv2
import matplotlib.pylab as plt

#Load the image
image = cv2.imread("circle.jpg")



#Resize image
"""
scale_percent = 90 # percent of original size
width = int(image.shape[1] * scale_percent / 100)
height = int(image.shape[0] * scale_percent / 100)
dim = (width, height)
image = cv2.resize(image,dim, interpolation = cv2.INTER_AREA)
"""

#Convert image to grayscale
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

#Make a treshhold
thresh = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)[1]

# Find circles with HoughCircles
circles = cv2.HoughCircles(thresh, cv2.HOUGH_GRADIENT, 1, minDist=50 ,param1=100, param2=15, minRadius=10,maxRadius=100)

if circles is not None:
    circles = np.round(circles[0, :]).astype("int")
    for (x,y,r) in circles:
        cv2.circle(image, (x,y), r, (36,255,12), 2)

cv2.imshow('thresh', thresh)
cv2.imshow('image', image)
cv2.waitKey()
cv2.destroyAllWindows()

