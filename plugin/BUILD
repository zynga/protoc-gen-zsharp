cc_library(
  name = "zsharp",
  hdrs = glob(["src/lib/*.h"]),
  srcs = glob(["src/lib/*.cc"]),
  deps = [
    "@protobuf//:protobuf",
    "@protobuf//:protoc_lib",
  ],
  visibility = ["//visibility:public"],
)

cc_binary(
  name = "protoc-gen-zsharp",
  srcs = ["src/main/protoc-gen-zsharp.cc"],
  deps = [
    "@protobuf//:protoc_lib",
    ":zsharp",
  ],

  # Travis Android builds use a really old c++ stdlib.
  # This option forces a "FULLY_STATIC" build per
  # <https://bazel.io/docs/be/c-cpp.html#cc_binary.linkstatic>
  # linkopts = ["-static"],
#  linkopts = select({
#    ":darwin_mode": [], 
#    "//conditions:default": ["-static"]
#  }),
)