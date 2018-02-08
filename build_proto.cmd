protoc --proto_path=./proto --cpp_out=./plugin/src/lib/generated ./proto/event_plugin.proto
protoc --proto_path=./proto --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime/Generated ./proto/event_plugin.proto
protoc --proto_path=./proto --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime/Generated ./proto/event_source.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=checksum=true:./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/event_test.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/delta_test.proto


