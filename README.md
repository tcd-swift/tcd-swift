tcd-swift
=========

An implementation of the Swift programming language for CS4071 Compiler Design II @ Trinity College, Dublin

## Developing

#### Mac OSX

_Requirements:_ [Boot2Docker](https://github.com/boot2docker/boot2docker)

* ```brew install docker```
* ```boot2docker init```
* ```boot2docker up```
* ```docker build ianconnolly/tcd-swift```

Running ```./launch.sh``` will drop you into a shell inside the docker
container with the repository mounted in ```/opt/tcd-swift```.


So  ```cd /opt/tcd-swift``` will allow you to run make commands like ```make all```.

The docker build will take awhile the first time, needs to be done only once.
Any updates to it will be incremental and should be much quicker.
