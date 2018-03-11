protoc --proto_path=./proto --cpp_out=./plugin/src/lib/generated ./proto/event_plugin.proto
protoc --proto_path=./proto --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime/Generated ./proto/event_plugin.proto
protoc --proto_path=./proto --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime/Generated ./proto/event_source.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=checksum=true:./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/event_test.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/delta_test.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/simple_map.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/simple_list.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/simple_message.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/upgrade_test.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/file_test.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/no_events_test.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/complex_test.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/unittest_import_proto3.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/unittest_import_public_proto3.proto
protoc --proto_path=./proto/ --plugin=protoc-gen-zsharp=./plugin/bazel-bin/protoc-gen-zsharp.exe --zsharp_out=./runtime/src/Zynga.Protobuf.Runtime.Tests ./proto/test/unittest_proto3.proto
