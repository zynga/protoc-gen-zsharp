#!/bin/bash

protoc --proto_path=./test --plugin=protoc-gen-zsharp=./bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./test ./test/event.proto