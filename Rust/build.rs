const SRCFILE: &str = "src/fft_c.c";
fn main() {
    cc::Build::new()
        .file(SRCFILE)
        .flag("-O3")
        .flag("-Wall")
        .flag("-march=native")
        .compile("fft_normal");
    cc::Build::new()
        .file(SRCFILE)
        .flag("-O3")
        .flag("-Wall")
        .flag("-march=native")
        .flag("-ffast-math") // unsafe (because float arithmetic is not commutative)
        .flag("-Dunsafemath") // #ifdef
        .compile("fft_unsafemath");
    println!("cargo::rerun-if-changed={SRCFILE}");
}
