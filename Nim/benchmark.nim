import fft    # here is my functions
import bitops
import std/[complex, strformat, times]

const LOG2FFTSIZE = 12
const FFT_REPEAT = 1000

const SIZE = cast[int](rotateLeftBits(1'u32, LOG2FFTSIZE))
const SIZE_PER2 = cast[int](rotateLeftBits(1'u32, LOG2FFTSIZE-1))

proc main() =
    var xy: array[SIZE, Complex32]
    for i in 0..<SIZE_PER2:
        xy[i] = complex(1.0'f32, 0.0'f32)
        xy[i+SIZE_PER2] = complex(-1.0'f32, 0.0'f32)

    var f = newfft()
    var fft_out: seq[Complex32];
    var timestart = epochTime()
    for i in 0..<FFT_REPEAT:
        fft_out = f.fft(LOG2FFTSIZE, xy)
    var eltime = 1000*(epochTime() - timestart)
    var s = fmt("{FFT_REPEAT:6} piece of {SIZE} pt FFT;  {eltime/FFT_REPEAT:9.5} ms/piece\n")
    echo s

    echo "bin        real             imag           absval";
    for i in 0..<6:
        var s = fmt("{i:3} {fft_out[i].re:16.4} {fft_out[i].im:16.4} {abs(fft_out[i]):16.4}")
        echo s

main()
