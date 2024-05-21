fn main() {
    cc::Build::new()
        .file("src/fft_c.c")
        .flag("-O3")
        .flag("-Wall")
        .flag("-march=native")
        .compile("fft_normal");
    cc::Build::new()
        .file("src/fft_c.c")
        .flag("-O3")
        .flag("-Wall")
        .flag("-march=native")
        .flag("-ffast-math") // unsafe (because float arithmetic is not commutative)
        .flag("-Dunsafemath") // #ifdef
        .compile("fir_unsafemath");
    println!("cargo::rerun-if-changed=src/fir.c");
}
