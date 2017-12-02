#!/bin/bash

bazel build :protoc-gen-zsharp --verbose_failures --apple_crosstool_transition   --experimental_objc_crosstool=all --macos_sdk_version=10.11
