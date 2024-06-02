OBJS=benchmark

all:
	nim c --opt:speed benchmark.nim

clean:
	rm -rf $(OBJS)

PROG=./benchmark
test:
	$(PROG)
	@($(PROG);$(PROG);$(PROG);$(PROG);$(PROG);  $(PROG);$(PROG);$(PROG);$(PROG);$(PROG) )|../avgcalc.py
