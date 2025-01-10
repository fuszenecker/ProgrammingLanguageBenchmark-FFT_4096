use num_complex::Complex;
use std::ptr::NonNull;
use std::slice;

pub type Cf64 = Complex<f64>;

#[repr(C)]
pub struct Fft {
    phasevec: [Cf64; 32],
}

// Public function
impl Fft {
    #[no_mangle] 
    pub extern "C" fn new() -> *const Fft {
        const PHLEN: usize = 32;
        let mut fft = Fft {
            phasevec: [Complex::new(0.0, 0.0); PHLEN],
        };
        for i in 0..PHLEN {
            let phase = -2.0 * std::f64::consts::PI / 2.0_f64.powi(i as i32 + 1);
            fft.phasevec[i] = Complex::new(phase.cos(), phase.sin());
        }
        Box::into_raw(Box::new(fft))
    }

    #[inline(never)]
    #[no_mangle] 
    pub extern "C" fn fft(ptr: *mut Fft, xy_out_ptr: *mut Cf64, xy_in_ptr: *const Cf64, length: i32) {
        if !ptr.is_null() { 
            unsafe { 
                let non_null_ptr = NonNull::new(ptr).expect("Pointer to Fft is null.");
                let this: &Fft = non_null_ptr.as_ref();
                let xy_out: &mut [Cf64] = slice::from_raw_parts_mut(xy_out_ptr, length.try_into().unwrap());
                let xy_in: &[Cf64] = slice::from_raw_parts(xy_in_ptr, length.try_into().unwrap());

                let log2point = xy_in.len().ilog2();
                // if we use these assert_eq checks, the compiler can produce a faster code
                assert_eq!(xy_out.len(), xy_in.len());
                assert_eq!(xy_in.len(), 1 << log2point);

                for (i, &xy_act) in xy_in.iter().enumerate() {
                    xy_out[i.reverse_bits() >> (usize::count_zeros(0) - log2point)] = xy_act;
                }

                // here begins the Danielson-Lanczos section;
                for l2pt in 0..log2point {
                    let wphase_xy = this.phasevec[l2pt as usize];
                    let mmax = 1 << l2pt;
                    let mut w_xy = Complex::new(1.0, 0.0);
                    for m in 0..mmax {
                        for i in (m..xy_out.len()).step_by(mmax << 1) {
                            let temp = w_xy * xy_out[i + mmax];
                            xy_out[i + mmax] = xy_out[i] - temp;
                            xy_out[i] += temp;
                        }
                        w_xy *= wphase_xy; // rotate
                    }
                }
            }
        }
    }


    #[inline(never)]
    #[no_mangle] 
    pub extern "C" fn free(ptr: *mut Fft) {
        if !ptr.is_null() { 
            unsafe { 
                let _ = Box::from_raw(ptr);
            }
        }
    }
}
