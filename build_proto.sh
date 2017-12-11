#!/bin/bash

protoc --proto_path=./proto --cpp_out=./plugin/src/lib/generated ./proto/event_plugin.proto
protoc --proto_path=./proto --csharp_out=./runtime/src/Zynga.Protobuf.Runtime/Generated ./proto/event_source.proto
protoc --proto_path=./proto --csharp_out=./runtime/src/Zynga.Protobuf.Runtime/Generated ./proto/event_plugin.proto


