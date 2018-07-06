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

$protoc --proto_path=$SCRIPT_DIR/proto --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime/Generated $SCRIPT_DIR/proto/event_plugin.proto
$protoc --proto_path=$SCRIPT_DIR/proto --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime/Generated $SCRIPT_DIR/proto/event_source.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=checksum=true:$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/event_test.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/delta_test.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/simple_map.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/simple_list.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/simple_message.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/upgrade_test.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/file_test.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/no_events_test.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/complex_test.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/unittest_import_proto3.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/unittest_import_public_proto3.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/unittest_proto3.proto
$protoc --proto_path=$SCRIPT_DIR/proto/ --plugin=protoc-gen-zsharp=$zsharp \
  --zsharp_out=$SCRIPT_DIR/runtime/src/Zynga.Protobuf.Runtime.Tests $SCRIPT_DIR/proto/test/struct_test.proto
