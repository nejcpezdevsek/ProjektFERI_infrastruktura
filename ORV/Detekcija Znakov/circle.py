import numpy as np
import cv2
import matplotlib.pylab as plt

def trafficSignCircle():
        
    cap = cv2.VideoCapture(1)


    while(cap.isOpened()):
        _,image = cap.read()
        blur = cv2.GaussianBlur(image,(5,5),0)
        hsv_image = cv2.cvtColor(blur,cv2.COLOR_BGR2HSV)

        low = np.array([90,80,50])
        high = np.array([110,255,255])
        mask = cv2.inRange(hsv_image,low,high)
        kernel = np.ones((2,3),np.uint8)
        mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)
        mask = cv2.morphologyEx(mask, cv2.MORPH_CLOSE, kernel)
        color = cv2.bitwise_and(image, image, mask=mask)


        
        #Make a treshhold
        ret,thresh = cv2.threshold(hsv_image,127,255,cv2.THRESH_BINARY_INV)    
        
        # Find circles with HoughCircles
        circles = cv2.HoughCircles(mask, cv2.HOUGH_GRADIENT, 1, minDist=500 ,param1=100, param2=15, minRadius=25,maxRadius=200)
        
        if circles is not None:
            circles = np.round(circles[0, :]).astype("int")
            for (x,y,r) in circles:
                cv2.circle(image, (x,y), r, (0,255,0), 2)
        
        cv2.imshow("image",image)
        cv2.imshow("mask",color)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    cap.release()
    cv2.destroyAllWindows()

 """
        #Resize the image
        scale_percent = 110 # percent of original size
        width = int(image.shape[1] * scale_percent / 100)
        height = int(image.shape[0] * scale_percent / 100)
        dim = (width, height)
        image = cv2.resize(image,dim, interpolation = cv2.INTER_AREA)
    """


def main():
    trafficSignCircle()


if __name__ == '__main__':
    main()
