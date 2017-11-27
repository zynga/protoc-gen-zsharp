#include "src/lib/csharp_generator.h"
#include <google/protobuf/compiler/plugin.h>

using namespace google::protobuf;

int main(int argc, char** argv) {
  zynga::protobuf::compiler::csharp::Generator generator;
  return compiler::PluginMain(argc, argv, &generator);
}