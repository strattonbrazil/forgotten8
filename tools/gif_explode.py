#!/usr/bin/env python

import sys
import argparse
import os
from PyQt4.QtGui import *
from PyQt4.QtCore import *

def start():
    parser = argparse.ArgumentParser(description='Takes a scene object and generates gifs from regions and updates points.')
    parser.add_argument('gif_file', help='path to an animated gif')
    parser.add_argument('output_dir', help='path to dump individual files')
    parser.add_argument('prefix', help='prefix for files')

    args = parser.parse_args()

    app = QApplication(sys.argv)

    imageReader = QImageReader(args.gif_file)
    
    dstDir = QDir(args.output_dir)
    if not dstDir.exists():
        print("creating: " + str(dstDir.absolutePath()))
        os.makedirs(str(dstDir.absolutePath()))
    
    for i in range(imageReader.imageCount()):
        img = imageReader.read()

        dstSpritePath = args.prefix + "_%i_delay%i.png" % (i+1, imageReader.nextImageDelay())
        print("writing to: " + dstSpritePath)

        img.save(dstDir.absoluteFilePath(dstSpritePath))

        imageReader.jumpToNextImage()

    sys.exit(1)

if __name__ == '__main__':
    start()