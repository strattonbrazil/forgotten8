#!/usr/bin/env python

import sys
import math
import json
import argparse
from PyQt4.QtGui import *
from PyQt4.QtCore import *

class Window(QWidget):
    def __init__(self, layeredFile):
        super(QWidget, self).__init__()

        self.pick = None
        self.move = None

        with open(layeredFile, "r") as f:
            self.layeredData = json.load(f)
            if "base_image" not in self.layeredData:
                print("no 'base_image' in layered file: %s" % layeredFile)
                quit(1)
            elif "output_cs" not in self.layeredData:
                print("no 'output_cs' in layered file: %s" % layeredFile)
                quit(1)
            elif "output_images" not in self.layeredData:
                print("no 'output_images' in layered file: %s" % layeredFile)
                quit(1)
        
        # part the user is currently picking
        self.partIndex = 0

        self.layeredFile = layeredFile
        self.layeredDir = QDir(layeredFile)
        self.layeredDir.cdUp()

        self.setWindowTitle("Layered Workflow")
        self.setGeometry(100, 100, 800, 400)

        self.setupImageReader()
        if self.imageReader.imageCount() > 1: # animated
            self.frameIndex = -1
            self.updateTimer = QTimer()
            self.updateTimer.setInterval(self.imageReader.nextImageDelay())
            self.updateTimer.timeout.connect(self.updateFrame)
            self.updateTimer.start()

    def setupImageReader(self):
        imgPath = self.layeredDir.absoluteFilePath(self.layeredData["base_image"])
        self.imageReader = QImageReader(imgPath)


    def updateFrame(self):
        self.frameIndex = self.frameIndex + 1
        if self.frameIndex == self.imageReader.imageCount(): # reload
            self.setupImageReader()
            self.frameIndex = 0

        self.imageReader.jumpToImage(self.frameIndex)
        self.repaint()

    def paintEvent(self, event):
        painter = QPainter()
        painter.begin(self)

        self.drawBackground(painter)

        img = self.imageReader.read()
        self.imgOrigin = QPoint(0.5 * (self.width() - img.width()), 0.5 * (self.height() - img.height()))
        painter.drawImage(self.imgOrigin, img)

        if self.move:
            painter.fillRect(self.pick.x(), self.pick.y(),
                             self.move.x() - self.pick.x(), self.move.y() - self.pick.y(), QColor(255, 255, 0, 100))

        if self.partIndex != len(self.layeredData["parts"]):
            part = self.getActivePart()

            if "rect" in part: # draw what was previously saved
                painter.fillRect(self.imgOrigin.x() + part["rect"]["x"],
                                 self.imgOrigin.y() + part["rect"]["y"],
                                 part["rect"]["width"],
                                 part["rect"]["height"],
                                 QColor(0, 0, 255, 100))

            msg = "Choose part: '%s'" % part["name"]
            if "rect" in part:
                msg += ", (s) to skip"
            if self.move:
                msg += ", (a) to accept"
            lineWidth = painter.fontMetrics().width(msg)
            msgX = 0.5 * (self.width() - lineWidth)
            self.drawOutlinedText(painter, msgX, 50, msg)

        painter.end()

    def drawOutlinedText(self, painter, x, y, text):
        painter.setPen(QColor(0,0,0))
        painter.drawText(x, y, text)
        painter.setPen(QColor(255,255,255))
        painter.drawText(x - 2, y - 2, text)

    def getActivePart(self):
        return self.layeredData["parts"][self.partIndex]

    def drawBackground(self, painter):
        tileSize = 32
        bgColors = [ QColor(160, 160, 160), QColor(128, 128, 128) ]
        # painter.setPen(QColor(168, 34, 3))

        for row in range(0, 1 + self.height() / tileSize):
            for column in range(0, 1 + self.width() / tileSize):
                tileColor = bgColors[(row + column) % len(bgColors)]
                painter.fillRect(column*tileSize, row*tileSize, tileSize, tileSize, tileColor)

    def mousePressEvent(self, event):
        self.pick = event.pos()
        self.move = None

    def mouseMoveEvent(self, event):
        self.move = event.pos()

    def keyPressEvent(self, event):
        part = self.getActivePart()
        if event.key() == Qt.Key_S and "rect" in part:
            self.partIndex += 1
            self.move = None
        elif event.key() == Qt.Key_A and self.move:
            part["rect"] = {
                "x" : self.pick.x() - self.imgOrigin.x(),
                "y" : self.pick.y() - self.imgOrigin.y(),
                "width" : self.move.x() - self.pick.x() - 1,
                "height" : self.move.y() - self.pick.y() - 1
            }
            self.partIndex += 1
            self.move = None

        # TODO: check if done
        if self.partIndex == len(self.layeredData["parts"]):
            answer = QMessageBox.question(self, "Save changes? ", "Save changes to %s?" % self.layeredFile)
            if answer == QMessageBox.Yes or answer == QMessageBox.Ok:
                print("saving changes")
                with open(self.layeredFile, "w") as f:
                    json.dump(self.layeredData, f, indent=4)
            else:
                print("changes ignored, closing")
            exit(0)

def start():
    parser = argparse.ArgumentParser(description='Takes a scene object and generates gifs from regions and updates points.')
    parser.add_argument('scene_file', 
                        help='a scene file with hierarchy')

    args = parser.parse_args()

    app = QApplication(sys.argv)
    window = Window(args.scene_file)
    window.show()
    
    sys.exit(app.exec_())
	
if __name__ == '__main__':
   start()