#include <time.h>
#include <stdio.h>
#include <string.h>
#include "fft.h"
#ifdef CHECKFFT
# include "dft_test.h"
#endif

#define SIZE (1<<LOG2FFTSIZE)

FLOAT_TYPE in_re[SIZE];
FLOAT_TYPE in_im[SIZE];
FLOAT_TYPE out_re[SIZE];
FLOAT_TYPE out_im[SIZE];

complex float cplxin[SIZE];
complex float cplxout[SIZE];

int main(int argc, char **argv) {
    double eltime;
    struct timespec gstart, gend;

    //for(i=0; i<SIZE/2; i++) { xy.re[i]= 1.; xy.im[i]= 0.; }
    //for(   ; i<SIZE  ; i++) { xy.re[i]=-1.; xy.im[i]= 0.; }
    for(int i=0; i<SIZE; i++) {
	in_re[i]= 1.+i/1000.;
	in_im[i]= i/2000.;
	cplxin[i] = in_re[i] + I*in_im[i];
    }

// FFT-vector io
    clock_gettime(CLOCK_PROCESS_CPUTIME_ID, &gstart);
    for (int i=0; i<FFT_REPEAT; i++) fft_vec(LOG2FFTSIZE, out_re, out_im, in_re, in_im);
    clock_gettime(CLOCK_PROCESS_CPUTIME_ID, &gend);

    eltime = 1000.0*(gend.tv_sec - gstart.tv_sec) + (gend.tv_nsec - gstart.tv_nsec)/1000000.;
    printf("vecio  %6d piece(s) of %d pt FFT;  %9.5f ms/piece\n", FFT_REPEAT, 1<<LOG2FFTSIZE, eltime/FFT_REPEAT);

// FFT-complex io
    clock_gettime(CLOCK_PROCESS_CPUTIME_ID, &gstart);
    for (int i=0; i<FFT_REPEAT; i++) fft_cplx(LOG2FFTSIZE, cplxout, cplxin);
    clock_gettime(CLOCK_PROCESS_CPUTIME_ID, &gend);

    eltime = 1000.0*(gend.tv_sec - gstart.tv_sec) + (gend.tv_nsec - gstart.tv_nsec)/1000000.;
    printf("cplxio %6d piece(s) of %d pt FFT;  %9.5f ms/piece\n", FFT_REPEAT, 1<<LOG2FFTSIZE, eltime/FFT_REPEAT);

#ifdef CHECKFFT
    if (argc > 1 && !strcmp(argv[1], "check")) {
	fprintf(stderr, "Check with dft ...\n");
	dft_test(cplxout, cplxin, LOG2FFTSIZE, 1e-6); // compare & relative error
    }
#endif
    for(int i=0; i<6; i++) {
	printf("%3d %16.4f %16.4f\n", i, out_re[i], out_im[i]);
	printf("%3d %16.4f %16.4f\n", i, creal(cplxout[i]), cimag(cplxout[i]));
    }
    return 0;
}
