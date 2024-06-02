import bitops
import std/[complex, math]

type Fft = object
    phasevec: array[32, Complex32]

proc newfft*(): Fft =
    var phasevec: array[32, Complex32]
    for i in 0..<32:
        let point = toFloat(cast[int](rotateLeftBits(2u32, i)))
        phasevec[i] = complex32(cos(-2'f32*PI/point), sin(-2'f32*PI/point))
    var f = Fft()
    f.phasevec = phasevec
    return f

proc fft*(self: Fft, log2point: int, xy_in: openArray[Complex32]): seq[Complex32] =
    let size = cast[int](rotateLeftBits(1'u32, log2point))
    var xy_out = newSeq[Complex32](size)

    for i in 0..<size:
        let ui = cast[uint32](i)
        xy_out[rotateRightBits(reverseBits(ui), 32 - log2point)] = xy_in[i]

    # here begins the Danielson-Lanczos section
    let n = size
    var l2pt=0
    var mmax=1
    while n > mmax:
        let istep = mmax * 2

        let wphase_XY = self.phasevec[l2pt]
        l2pt+=1

        var w_XY = complex32(1.0, 0.0)
        for m in 0..<mmax:
            var i = m
            while i < n:
                var tempXY = w_XY * xy_out[i+mmax]
                xy_out[i+mmax]  = xy_out[i] - tempXY
                xy_out[i     ] += tempXY
                i += istep
            w_XY *= wphase_XY; # rotate
        mmax=istep
    return xy_out
