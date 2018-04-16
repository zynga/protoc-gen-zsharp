#!/bin/bash

set -ex

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

if [ ! -d "$SCRIPT_DIR/.protoc" ]; then
  echo "Installing protoc"
  mkdir "$SCRIPT_DIR/.protoc"
  pushd $SCRIPT_DIR/.protoc
    curl -L https://github.com/google/protobuf/releases/download/v3.4.0/protoc-3.4.0-osx-x86_64.zip > protoc-3.4.0-osx-x86_64.zip
    unzip protoc-3.4.0-osx-x86_64.zip
  popd
fi

protoc="$SCRIPT_DIR/.protoc/bin/protoc"
zsharp="$SCRIPT_DIR/plugin/bazel-bin/protoc-gen-zsharp"

$protoc --proto_path=$SCRIPT_DIR/proto --cpp_out=$SCRIPT_DIR/plugin/src/lib/generated $SCRIPT_DIR/proto/event_plugin.proto

pushd $SCRIPT_DIR/plugin
  ./build_linux.sh
  ./build_osx.sh
popd

$SCRIPT_DIR/build_proto.sh

pushd $SCRIPT_DIR/runtime
  ./build.sh CreateNuget
popd

rm -rf $SCRIPT_DIR/bin
mkdir -p $SCRIPT_DIR/bin
cp $SCRIPT_DIR/plugin/protoc-gen-zsharp-linux $SCRIPT_DIR/bin
cp $SCRIPT_DIR/plugin/bazel-bin/protoc-gen-zsharp $SCRIPT_DIR/bin
cp $SCRIPT_DIR/runtime/bin/nuget/* $SCRIPT_DIR/bin

echo "All build artifacts are in the bin directory"
ls -l $SCRIPT_DIR/bin
