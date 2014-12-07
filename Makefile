CSHARPCOMPILER = dmcs

all: bin/coco.exe
	mkdir -p generated
	mkdir -p bin
	mono bin/coco.exe -frames src/frames -trace FIS -o generated -namespace TCDSwift src/grammar/TCDSwift.ATG
	$(CSHARPCOMPILER) src/Main.cs generated/*.cs -out:bin/tcc.exe

bin/coco.exe:
	mkdir -p bin
	curl http://www.ssw.uni-linz.ac.at/coco/CS/Coco.exe > bin/coco.exe

clean:
	rm -rf generated/
	rm -rf bin/
