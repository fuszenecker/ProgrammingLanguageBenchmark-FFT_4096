use num_complex::Complex;
pub type Cf32 = Complex<f32>;

extern "C" {
    pub fn fft_c(log2point: i32, xy_out: *mut Cf32, xy_in: *const Cf32);
    pub fn fft_cu(log2point: i32, xy_out: *mut Cf32, xy_in: *const Cf32);
}

pub fn fftc(xy_out: &mut [Cf32], xy_in: &[Cf32]) {
    assert_eq!(xy_in.len(), xy_out.len());
    let log2point = xy_in.len().ilog2() as i32;
    unsafe { fft_c(log2point, xy_out.as_mut_ptr(), xy_in.as_ptr()) };
}

pub fn fftcu(xy_out: &mut [Cf32], xy_in: &[Cf32]) {
    assert_eq!(xy_in.len(), xy_out.len());
    let log2point = xy_in.len().ilog2() as i32;
    unsafe { fft_cu(log2point, xy_out.as_mut_ptr(), xy_in.as_ptr()) };
}
