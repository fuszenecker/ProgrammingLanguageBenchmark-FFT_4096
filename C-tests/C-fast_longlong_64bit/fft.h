#define MULTYPE __int128
#define INTMUL (55+5)
#define INTMULSAMPLE (55-4)

struct _sample {
    long long i;
    long long q;
};
// step: N*log2(N)
void fft     (int log2point, struct _sample *restrict xy_out, const struct _sample *restrict xy_in);
