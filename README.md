tcd-swift
=========

An implementation of the Swift programming language for CS4071 Compiler Design II @ Trinity College, Dublin

## Developing

#### Mac OSX

_Requirements:_ [Boot2Docker](https://github.com/boot2docker/boot2docker)

* ```brew install docker```
* ```boot2docker init```
* ```boot2docker up```
* ```docker pull ianconnolly/tcd-swift```

Running ```./launch.sh``` will drop you into a shell inside the docker
container with the repository mounted in ```/opt/tcd-swift```.


So  ```cd /opt/tcd-swift``` will allow you to run make commands like ```make all```.

The docker pull will take awhile the first time, needs to be done only once.
Any updates to it will be incremental and should be much quicker.

#### Linux

##### Debian
See: [https://docs.docker.com/installation/debian/](https://docs.docker.com/installation/debian/)

##### Ubuntu
See: [http://docs.docker.com/installation/ubuntulinux/](http://docs.docker.com/installation/ubuntulinux/)

Once docker is installed:

* ```docker pull ianconnolly/tcd-swift```
* ```git clone https://github.com/tcd-swift/tcd-swift```
* ```cd tcd-swift && ./launch.sh```
* ```cd /opt/tcd-swift```
