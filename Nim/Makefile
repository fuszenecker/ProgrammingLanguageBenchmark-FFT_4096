OBJS=benchmark

all:
	nim c -d:release --opt:speed benchmark.nim

clean:
	rm -rf $(OBJS)

PROG=./benchmark
test:
	$(PROG)
	@($(PROG);$(PROG);$(PROG);$(PROG);$(PROG);  $(PROG);$(PROG);$(PROG);$(PROG);$(PROG) )|../avgcalc.py
