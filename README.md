# protoc-gen-zsharp
The main purpose of this project is to add support for event sourcing and other modern c# features within Protobuf targeting the c# codegen. This is a standalone c# protoc plugin that will generate code compatible with the Google.Protobuf runtime provided by google.

# CSharp proto-gen plugin build steps
Build instructions for protoc-gen-zsharp c++ plugin

1.) Install Bazel http://www.bazel.io/docs/install.html. We use this build tool because that's what google/protobufs uses. Bazel requires Java.

2.) Install proper development tools for the proper OS:

* OSX: Xcode 8.1+ (may also require that you have the proper OSX SDKs get from https://github.com/phracker/MacOSX-SDKs)
* Windows: Visual Studio 2015+ 
* Linux: GCC/G++ (tested and built on Ubuntu)

3.) Install protoc locally on the machine and include in your path

4.) Run build_proto.sh|cmd

5.) cd into plugin folder 

6.) Build with ```bazel build :protoc-gen-zsharp```

# Zynga.Protobuf.Runtime

1.) Install dotnet core 2.0 and latest Mono

2.) (optional) Install c# compatible IDE (Rider or Visual Studio)
* build .sln file included in the runtime folder 

3.) Build via commandline
* cd runtime 
* run ```build.sh|cmd build```

