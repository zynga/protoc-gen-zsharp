# protoc-gen-zsharp
The main purpose of this project is to add support for event sourcing and other modern c# features within Protobuf targeting the c# codegen. This is a standalone c# protoc plugin that will generate code compatible with the Google.Protobuf runtime provided by google.

The original C# codegen was forked from the google protobuf source (https://github.com/google/protobuf/tree/master/src/google/protobuf/compiler/csharp)

[![Build status](https://ci.appveyor.com/api/projects/status/g42go2oy0lf7r73d/branch/master?svg=true)](https://ci.appveyor.com/project/wackoisgod/protoc-gen-zsharp/branch/master)

# CSharp proto-gen plugin build steps
Build instructions for protoc-gen-zsharp c++ plugin

1.) Install Bazel http://www.bazel.io/docs/install.html. We use this build tool because that's what google/protobufs uses. Bazel requires Java.

2.) Install proper development tools for the proper OS:

* OSX: Xcode 8.1+ (may also require that you have the proper OSX SDKs get from https://github.com/phracker/MacOSX-SDKs)
* Windows: Visual Studio 2015+ 
* Linux: GCC/G++ (tested and built on Ubuntu)

3.) Install protoc locally on the machine and include in your path

4.) cd into plugin folder 

5.) Build with ```bazel build :protoc-gen-zsharp```

6.) Run build_proto.sh|cmd from the root folder

# Zynga.Protobuf.Runtime

1.) Install dotnet core 2.0 and latest Mono

2.) (optional) Install c# compatible IDE (Rider or Visual Studio)
* build .sln file included in the runtime folder 

3.) Build via commandline
* cd runtime 
* run ```build.sh|cmd build```

