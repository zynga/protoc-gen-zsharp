# protoc-gen-zsharp

Build instructions for protoc-gen-zsharp plugin

1.) Install Bazel http://www.bazel.io/docs/install.html. We use this build tool because that's what google/protobufs uses. Bazel requires Java.

2.) Build with ```bazel build :protoc-gen-zsharp```

# zsharp runtime

* build .sln file included in the runtime folder 