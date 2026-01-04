TARGET=./dist/fft-benchmark
ARCH=x64
DESTDIR=dist
VERSION=1.9.1.2

CC=gcc
CFLAGS =-Wall -O2 -funroll-all-loops -fPIC

PARAMS_CPU = -march=native
ifeq ($(shell uname -m), armv7l)
    PARAMS_CPU = -mcpu=cortex-a5 -mfpu=neon-fp16 
endif
CFLAGS+= $(PARAMS_CPU)

CFLAGS+=-DLOG2FFTSIZE=12
CFLAGS+=-DFFT_REPEAT=10000

# improve speed
CFLAGS+=-DMOD_SPEED

LDFLAGS=-shared
OBJS=fft.o

TARGET=benchmark

all: libfft libfftr
	dotnet build -c Release -p:VersionPrefix=$(VERSION) fft-benchmark.csproj
	dotnet publish -c Release -o $(DESTDIR) --self-contained -r linux-$(ARCH) -p:PublishTrimmed=False -p:VersionPrefix=$(VERSION) fft-benchmark.csproj

libfft: $(OBJS)
	$(CC) $(OBJS) $(LDFLAGS) -o libfft.so

libfftr: 
	cd rust && ./build.sh && cp target/release/libfftr.so ..

clean:
	rm -fr $(TARGET) bin obj *.o *.so BenchmarkDotNet.Artifacts dist *.deb

package:
	sed -e 's/%%VERSION%%/$(VERSION)/g' packaging/DEBIAN/control.in > packaging/DEBIAN/control
	dotnet publish -c Release -r linux-$(ARCH) --self-contained -o packaging/opt/fft-benchmark -p:PublishTrimmed=False -p:VersionPrefix=$(VERSION) fft-benchmark.csproj
	dpkg-deb --build packaging fft-benchmark-$(VERSION)-$(ARCH).deb

docker:
    dotnet publish -c Release -o dist --self-contained -r linux-$(ARCH) -p:PublishTrimmed=False -p:VersionPrefix=$(VERSION) fft-benchmark.csproj
    docker build -t fft-benchmark:$(VERSION) .

PROG=$(TARGET)
test:
	./$(PROG)
	@(./$(PROG);./$(PROG);./$(PROG);./$(PROG);./$(PROG);  ./$(PROG);./$(PROG);./$(PROG);./$(PROG);./$(PROG) )|../avgcalc.py

test2:	test

fft.o: ../other_tests/C-tests/C-fast_double/fft.c
	$(CC) $(CFLAGS) -o $@ -c $<
