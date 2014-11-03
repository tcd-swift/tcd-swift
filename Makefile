all: bin/coco
	mkdir -p generated/
	mkdir -p bin/
	mono bin/coco -frames src/frames -o generated -namespace TCDSwift src/grammar/Swift.ATG
	dmcs src/Main.cs /opt/tcd-swift/generated/*.cs -out:bin/tcdscc.exe

bin/coco:
	mkdir -p bin/
	curl http://www.ssw.uni-linz.ac.at/coco/CS/Coco.exe > bin/coco

clean:
	rm -rf generated/
	rm -rf bin/
