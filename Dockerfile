FROM ubuntu:14.04
MAINTAINER Ian Connolly <ian@connolly.io>
RUN apt-get update
RUN apt-get install -y mono-complete
RUN apt-get install -y curl
RUN apt-get install -y haskell-platform
RUN apt-get install -y git
RUN apt-get install -y make
