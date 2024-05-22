import socket

import cv2
from cvzone.HandTrackingModule import HandDetector

cap = cv2.VideoCapture(0)
cap.set(3, 1280)
cap.set(4, 720)
success, img = cap.read()
h, w, _ = img.shape
detector = HandDetector(detectionCon=0.8, maxHands=2)

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressLeftHandPort = ("127.0.0.1", 5052)
serverAddressRightHandPort = ("127.0.0.1", 5053)

while True:
    # Get image frame
    success, img = cap.read()
    # Find the hand and its landmarks
    hands, img = detector.findHands(img)  # with draw

    if hands:
        for hand in hands:
            data = []

            for lm in hand["lmList"]:
                data.extend([lm[0], h - lm[1], lm[2]])

            if hand["type"] == "Right":
                sock.sendto(str.encode(str(data)), serverAddressRightHandPort)
            elif hand["type"] == "Left":
                sock.sendto(str.encode(str(data)), serverAddressLeftHandPort)

    # Display
    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord('q'):
        break
