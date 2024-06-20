all:
	if [ ! -e "rpncalc/target/release/rpncalc" ]; then git clone https://github.com/hg2ecz/rpncalc && cd rpncalc && cargo build --release; fi

test:
	bash -c "time rpncalc/target/release/rpncalc -f fft4096.rpn"

clean:
	rm -rf rpncalc

