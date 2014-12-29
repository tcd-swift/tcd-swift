CSHARPCOMPILER = dmcs

all: bin/coco.exe
	mkdir -p generated
	mkdir -p bin
	mono bin/coco.exe -frames src/frames -o generated -namespace TCDSwift src/grammar/TCDSwift.ATG
	$(CSHARPCOMPILER) src/Main.cs generated/*.cs src/common/*.cs -out:bin/tcdscc.exe

bin/coco.exe:
	mkdir -p bin
	curl http://www.ssw.uni-linz.ac.at/coco/CS/Coco.exe > bin/coco.exe

clean:
	rm -rf generated/
	rm -rf bin/
