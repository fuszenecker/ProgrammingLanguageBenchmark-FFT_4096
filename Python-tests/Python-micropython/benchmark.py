#!/usr/bin/micropython
# -*- coding: utf8 -*-

#from __future__ import print_function

import fft
import utime

LOG2FFTSIZE = 12
FFT_REPEAT = 100

SIZE = 1<<LOG2FFTSIZE
xy = [0+0j]*SIZE

if __name__ == "__main__":

    for i in range(int(SIZE/2)):
        xy[i]= 1.

    for i in range(int(SIZE/2), SIZE):
        xy[i]=-1.

    timestart = utime.time()
    for i in range(FFT_REPEAT):
        fft_out = fft.fft(LOG2FFTSIZE, xy)
    eltime = 1000*(utime.time() - timestart)
    print ("%6d piece of %d pt FFT;  %9.5f ms/piece\n"%(FFT_REPEAT, 1<<LOG2FFTSIZE, eltime/FFT_REPEAT))

    for i in range(6):
        print (i, fft_out[i])
