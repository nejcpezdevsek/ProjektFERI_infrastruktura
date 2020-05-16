import numpy as np
import cv2 



#Load the image
image = cv2.imread("scrabble.jpg")

#Resize the image
scale_percent = 50 # percent of original size
width = int(image.shape[1] * scale_percent / 100)
height = int(image.shape[0] * scale_percent / 100)
dim = (width, height)
image = cv2.resize(image,dim, interpolation = cv2.INTER_AREA)


#Convert image to grayscale
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
cv2.medianBlur(gray,5)

ret,thresh = cv2.threshold(gray,127,255,0)

contours, hierarchy = cv2.findContours(thresh, cv2.RETR_CCOMP, cv2.CHAIN_APPROX_SIMPLE)

cv2.drawContours(image,contours,-1,(0,255,0),3)

cv2.imshow("square",image)
cv2.waitKey()
cv2.destroyAllWindows()




