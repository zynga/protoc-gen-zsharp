#!/bin/bash

set -ex

cd `dirname $0`
script_dir=`pwd`

docker build -t bazel .

docker run -v $script_dir:/opt/zsharp bazel \
  /bin/bash -c "bazel build :protoc-gen-zsharp; mv bazel-bin/protoc-gen-zsharp protoc-gen-zsharp-linux;bazel clean"
