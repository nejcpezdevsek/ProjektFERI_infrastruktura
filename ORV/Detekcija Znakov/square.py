import numpy as np
import cv2 
import argparse



def trafficSignSquare():
    cap = cv2.VideoCapture(1)

    while(cap.isOpened()):
        _, image = cap.read()    
        blur = cv2.GaussianBlur(image,(5,5),0)
        hsv_image = cv2.cvtColor(blur, cv2.COLOR_BGR2HSV)

        
        low = np.array([90,80,50])
        high = np.array([110,255,255])
        mask = cv2.inRange(hsv_image, low, high)
        kernel = np.ones((3,3),np.uint8)
        mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)
        mask = cv2.morphologyEx(mask, cv2.MORPH_CLOSE, kernel)

        color = cv2.bitwise_and(image, image, mask=mask)
        largestArea = 0
        imageArea = image.shape[0]*image.shape[1]

        contours = cv2.findContours(mask,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE)[-2]
        if len(contours) > 0: 
            for contour in contours:
                    rect = cv2.minAreaRect(contour)
                    box = cv2.boxPoints(rect)
                    box = np.int0(box)
                    
                    #print("Calculation 1: {} - {} = {}".format(box[0],box[1],box[0]-box[1]))
                    sideOne = np.linalg.norm(box[0]-box[1])
                # print("Calculation 2: {} - {} = {}".format(box[0],box[3],box[0]-box[3]))
                    sideTwo = np.linalg.norm(box[0]-box[3])
                    # count area of the rectangle
                    area = sideOne*sideTwo

                    #Find largest contour
                    if area > largestArea:
                        largestArea = area
                        largestRect = box
                    #print("Largest Area:{}".format(largestArea))

        if largestArea > imageArea*0.02:
                    cv2.drawContours(image,[largestRect],0,(0,0,255),2)
                

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
    trafficSignSquare()


if __name__ == '__main__':
    main()
