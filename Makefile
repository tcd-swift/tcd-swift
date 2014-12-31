CSHARPCOMPILER = dmcs

all: bin/coco.exe
	mkdir -p generated
	mkdir -p bin
	mono bin/coco.exe -frames src/frames -o generated -namespace TCDSwift src/grammar/TCDSwift.ATG
	$(CSHARPCOMPILER) src/Main.cs generated/*.cs src/common/*.cs src/backend/*.cs -out:bin/tcdscc.exe

bin/coco.exe:
	mkdir -p bin
	curl http://www.ssw.uni-linz.ac.at/coco/CS/Coco.exe > bin/coco.exe

clean:
	rm -rf generated/
	rm -rf bin/

dominatortests:
	$(CSHARPCOMPILER) src/test/DominatorTreeTest.cs -out:bin/dominatortests.exe
	mono bin/dominatortests.exe

ssatests:
	$(CSHARPCOMPILER) src/test/SSATest.cs -out:bin/ssatests.exe
	mono bin/ssatests.exe