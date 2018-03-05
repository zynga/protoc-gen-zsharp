// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
// https://developers.google.com/protocol-buffers/
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// Author: kenton@google.com (Kenton Varda)
//  Based on original Protocol Buffers design by
//  Sanjay Ghemawat, Jeff Dean, and others.

#include <algorithm>
#include <google/protobuf/stubs/hash.h>
#include <limits>
#include <vector>

#include "src/lib/csharp_helpers.h"
#include "src/lib/csharp_names.h"
#include <google/protobuf/descriptor.pb.h>
#include <google/protobuf/io/printer.h>
#include <google/protobuf/wire_format.h>
#include <google/protobuf/stubs/strutil.h>
#include <google/protobuf/stubs/substitute.h>

#include "src/lib/csharp_field_base.h"
#include "src/lib/csharp_enum_field.h"
#include "src/lib/csharp_map_field.h"
#include "src/lib/csharp_message_field.h"
#include "src/lib/csharp_options.h"
#include "src/lib/csharp_primitive_field.h"
#include "src/lib/csharp_repeated_enum_field.h"
#include "src/lib/csharp_repeated_message_field.h"
#include "src/lib/csharp_repeated_primitive_field.h"
#include "src/lib/csharp_wrapper_field.h"
#include "src/lib/generated/event_plugin.pb.h"


using namespace google::protobuf;

namespace zynga {
namespace protobuf {
namespace compiler {
namespace csharp {

CSharpType GetCSharpType(FieldDescriptor::Type type) {
  switch (type) {
    case FieldDescriptor::TYPE_INT32:
      return CSHARPTYPE_INT32;
    case FieldDescriptor::TYPE_INT64:
      return CSHARPTYPE_INT64;
    case FieldDescriptor::TYPE_UINT32:
      return CSHARPTYPE_UINT32;
    case FieldDescriptor::TYPE_UINT64:
      return CSHARPTYPE_UINT32;
    case FieldDescriptor::TYPE_SINT32:
      return CSHARPTYPE_INT32;
    case FieldDescriptor::TYPE_SINT64:
      return CSHARPTYPE_INT64;
    case FieldDescriptor::TYPE_FIXED32:
      return CSHARPTYPE_UINT32;
    case FieldDescriptor::TYPE_FIXED64:
      return CSHARPTYPE_UINT64;
    case FieldDescriptor::TYPE_SFIXED32:
      return CSHARPTYPE_INT32;
    case FieldDescriptor::TYPE_SFIXED64:
      return CSHARPTYPE_INT64;
    case FieldDescriptor::TYPE_FLOAT:
      return CSHARPTYPE_FLOAT;
    case FieldDescriptor::TYPE_DOUBLE:
      return CSHARPTYPE_DOUBLE;
    case FieldDescriptor::TYPE_BOOL:
      return CSHARPTYPE_BOOL;
    case FieldDescriptor::TYPE_ENUM:
      return CSHARPTYPE_ENUM;
    case FieldDescriptor::TYPE_STRING:
      return CSHARPTYPE_STRING;
    case FieldDescriptor::TYPE_BYTES:
      return CSHARPTYPE_BYTESTRING;
    case FieldDescriptor::TYPE_GROUP:
      return CSHARPTYPE_MESSAGE;
    case FieldDescriptor::TYPE_MESSAGE:
      return CSHARPTYPE_MESSAGE;

      // No default because we want the compiler to complain if any new
      // types are added.
  }
  GOOGLE_LOG(FATAL)<< "Can't get here.";
  return (CSharpType) -1;
}

std::string StripDotProto(const std::string& proto_file) {
  int lastindex = proto_file.find_last_of(".");
  return proto_file.substr(0, lastindex);
}

std::string GetFileNamespace(const FileDescriptor* descriptor) {
  if (descriptor->options().has_csharp_namespace()) {
    return descriptor->options().csharp_namespace();
  }
  return UnderscoresToCamelCase(descriptor->package(), true, true);
}

bool HasFileEventSource(const FileDescriptor* descriptor) {
  if (descriptor != NULL) {
      const FileOptions& op = descriptor->options();
      return op.HasExtension(com::zynga::runtime::protobuf::file_event_sourced);
  }

  return false;
}

bool IsGoogleMessage(const Descriptor* descriptor) {
  // this is a hack but there is no way to exclude GoogleMessageTypes
  // other than checking their ClassName :/
  if (descriptor != NULL) {
     std::string ns = GetClassName(descriptor);
     return (ns.find("Google.Protobuf.WellKnownTypes") != std::string::npos);
  }

  return false;
}

// Returns the Pascal-cased last part of the proto file. For example,
// input of "google/protobuf/foo_bar.proto" would result in "FooBar".
std::string GetFileNameBase(const FileDescriptor* descriptor) {
    std::string proto_file = descriptor->name();
    int lastslash = proto_file.find_last_of("/");
    std::string base = proto_file.substr(lastslash + 1);
    return UnderscoresToPascalCase(StripDotProto(base));
}

std::string GetReflectionClassUnqualifiedName(const FileDescriptor* descriptor) {
  // TODO: Detect collisions with existing messages,
  // and append an underscore if necessary.
  return GetFileNameBase(descriptor) + "Reflection";
}

// TODO(jtattermusch): can we reuse a utility function?
std::string UnderscoresToCamelCase(const std::string& input,
                                   bool cap_next_letter,
                                   bool preserve_period) {
  string result;
  // Note:  I distrust ctype.h due to locales.
  for (int i = 0; i < input.size(); i++) {
    if ('a' <= input[i] && input[i] <= 'z') {
      if (cap_next_letter) {
        result += input[i] + ('A' - 'a');
      } else {
        result += input[i];
      }
      cap_next_letter = false;
    } else if ('A' <= input[i] && input[i] <= 'Z') {
      if (i == 0 && !cap_next_letter) {
        // Force first letter to lower-case unless explicitly told to
        // capitalize it.
        result += input[i] + ('a' - 'A');
      } else {
        // Capital letters after the first are left as-is.
        result += input[i];
      }
      cap_next_letter = false;
    } else if ('0' <= input[i] && input[i] <= '9') {
      result += input[i];
      cap_next_letter = true;
    } else {
      cap_next_letter = true;
      if (input[i] == '.' && preserve_period) {
        result += '.';
      }
    }
  }
  // Add a trailing "_" if the name should be altered.
  if (input[input.size() - 1] == '#') {
    result += '_';
  }
  return result;
}

std::string UnderscoresToPascalCase(const std::string& input) {
  return UnderscoresToCamelCase(input, true);
}

// Convert a string which is expected to be SHOUTY_CASE (but may not be *precisely* shouty)
// into a PascalCase string. Precise rules implemented:

// Previous input character      Current character         Case
// Any                           Non-alphanumeric          Skipped
// None - first char of input    Alphanumeric              Upper
// Non-letter (e.g. _ or 1)      Alphanumeric              Upper
// Numeric                       Alphanumeric              Upper
// Lower letter                  Alphanumeric              Same as current
// Upper letter                  Alphanumeric              Lower
std::string ShoutyToPascalCase(const std::string& input) {
  string result;
  // Simple way of implementing "always start with upper"
  char previous = '_';
  for (int i = 0; i < input.size(); i++) {
    char current = input[i];
    if (!ascii_isalnum(current)) {
      previous = current;
      continue;      
    }
    if (!ascii_isalnum(previous)) {
      result += ascii_toupper(current);
    } else if (ascii_isdigit(previous)) {
      result += ascii_toupper(current);
    } else if (ascii_islower(previous)) {
      result += current;
    } else {
      result += ascii_tolower(current);
    }
    previous = current;
  }
  return result;
}

// Attempt to remove a prefix from a value, ignoring casing and skipping underscores.
// (foo, foo_bar) => bar - underscore after prefix is skipped
// (FOO, foo_bar) => bar - casing is ignored
// (foo_bar, foobarbaz) => baz - underscore in prefix is ignored
// (foobar, foo_barbaz) => baz - underscore in value is ignored
// (foo, bar) => bar - prefix isn't matched; return original value
std::string TryRemovePrefix(const std::string& prefix, const std::string& value) {
  // First normalize to a lower-case no-underscores prefix to match against
  std::string prefix_to_match = "";
  for (size_t i = 0; i < prefix.size(); i++) {
    if (prefix[i] != '_') {
      prefix_to_match += ascii_tolower(prefix[i]);
    }
  }
  
  // This keeps track of how much of value we've consumed
  size_t prefix_index, value_index;
  for (prefix_index = 0, value_index = 0;
      prefix_index < prefix_to_match.size() && value_index < value.size();
      value_index++) {
    // Skip over underscores in the value
    if (value[value_index] == '_') {
      continue;
    }
    if (ascii_tolower(value[value_index]) != prefix_to_match[prefix_index++]) {
      // Failed to match the prefix - bail out early.
      return value;
    }
  }

  // If we didn't finish looking through the prefix, we can't strip it.
  if (prefix_index < prefix_to_match.size()) {
    return value;
  }

  // Step over any underscores after the prefix
  while (value_index < value.size() && value[value_index] == '_') {
    value_index++;
  }

  // If there's nothing left (e.g. it was a prefix with only underscores afterwards), don't strip.
  if (value_index == value.size()) {
    return value;
  }

  return value.substr(value_index);
}

// Format the enum value name in a pleasant way for C#:
// - Strip the enum name as a prefix if possible
// - Convert to PascalCase.
// For example, an enum called Color with a value of COLOR_BLUE should
// result in an enum value in C# called just Blue
std::string GetEnumValueName(const std::string& enum_name, const std::string& enum_value_name) {
  std::string stripped = TryRemovePrefix(enum_name, enum_value_name);
  std::string result = ShoutyToPascalCase(stripped);
  // Just in case we have an enum name of FOO and a value of FOO_2... make sure the returned
  // string is a valid identifier.
  if (ascii_isdigit(result[0])) {
    result = "_" + result;
  }
  return result;
}

std::string ToCSharpName(const std::string& name, const FileDescriptor* file) {
  std::string result = GetFileNamespace(file);
  if (result != "") {
    result += '.';
  }
  string classname;
  if (file->package().empty()) {
    classname = name;
  } else {
    // Strip the proto package from full_name since we've replaced it with
    // the C# namespace.
    classname = name.substr(file->package().size() + 1);
  }
  result += StringReplace(classname, ".", ".Types.", true);
  return "global::" + result;
}

std::string GetReflectionClassName(const FileDescriptor* descriptor) {
  std::string result = GetFileNamespace(descriptor);
  if (!result.empty()) {
    result += '.';
  }
  result += GetReflectionClassUnqualifiedName(descriptor);
  return "global::" + result;
}

std::string GetClassName(const Descriptor* descriptor) {
  return ToCSharpName(descriptor->full_name(), descriptor->file());
}

std::string GetClassName(const EnumDescriptor* descriptor) {
  return ToCSharpName(descriptor->full_name(), descriptor->file());
}

// Groups are hacky:  The name of the field is just the lower-cased name
// of the group type.  In C#, though, we would like to retain the original
// capitalization of the type name.
std::string GetFieldName(const FieldDescriptor* descriptor) {
  if (descriptor->type() == FieldDescriptor::TYPE_GROUP) {
    return descriptor->message_type()->name();
  } else {
    return descriptor->name();
  }
}

std::string GetFieldConstantName(const FieldDescriptor* field) {
  return GetPropertyName(field) + "FieldNumber";
}

std::string GetPropertyName(const FieldDescriptor* descriptor) {
  // TODO(jtattermusch): consider introducing csharp_property_name field option
  std::string property_name = UnderscoresToPascalCase(GetFieldName(descriptor));
  // Avoid either our own type name or reserved names. Note that not all names
  // are reserved - a field called to_string, write_to etc would still cause a problem.
  // There are various ways of ending up with naming collisions, but we try to avoid obvious
  // ones.
  if (property_name == descriptor->containing_type()->name()
      || property_name == "Types"
      || property_name == "Descriptor") {
    property_name += "_";
  }
  return property_name;
}

std::string GetOutputFile(
    const google::protobuf::FileDescriptor* descriptor,
    const std::string file_extension,
    const bool generate_directories,
    const std::string base_namespace,
    string* error) {
  string relative_filename = GetFileNameBase(descriptor) + file_extension;
  if (!generate_directories) {
    return relative_filename;
  }
  string ns = GetFileNamespace(descriptor);
  string namespace_suffix = ns;
  if (!base_namespace.empty()) {
    // Check that the base_namespace is either equal to or a leading part of
    // the file namespace. This isn't just a simple prefix; "Foo.B" shouldn't
    // be regarded as a prefix of "Foo.Bar". The simplest option is to add "."
    // to both.
    string extended_ns = ns + ".";
    if (extended_ns.find(base_namespace + ".") != 0) {
      *error = "Namespace " + ns + " is not a prefix namespace of base namespace " + base_namespace;
      return ""; // This will be ignored, because we've set an error.
    }
    namespace_suffix = ns.substr(base_namespace.length());
    if (namespace_suffix.find(".") == 0) {
      namespace_suffix = namespace_suffix.substr(1);
    }
  }

  string namespace_dir = StringReplace(namespace_suffix, ".", "/", true);
  if (!namespace_dir.empty()) {
    namespace_dir += "/";
  }
  return namespace_dir + relative_filename;
}

// TODO: c&p from Java protoc plugin
// For encodings with fixed sizes, returns that size in bytes.  Otherwise
// returns -1.
int GetFixedSize(FieldDescriptor::Type type) {
  switch (type) {
    case FieldDescriptor::TYPE_INT32   : return -1;
    case FieldDescriptor::TYPE_INT64   : return -1;
    case FieldDescriptor::TYPE_UINT32  : return -1;
    case FieldDescriptor::TYPE_UINT64  : return -1;
    case FieldDescriptor::TYPE_SINT32  : return -1;
    case FieldDescriptor::TYPE_SINT64  : return -1;
    case FieldDescriptor::TYPE_FIXED32 : return internal::WireFormatLite::kFixed32Size;
    case FieldDescriptor::TYPE_FIXED64 : return internal::WireFormatLite::kFixed64Size;
    case FieldDescriptor::TYPE_SFIXED32: return internal::WireFormatLite::kSFixed32Size;
    case FieldDescriptor::TYPE_SFIXED64: return internal::WireFormatLite::kSFixed64Size;
    case FieldDescriptor::TYPE_FLOAT   : return internal::WireFormatLite::kFloatSize;
    case FieldDescriptor::TYPE_DOUBLE  : return internal::WireFormatLite::kDoubleSize;

    case FieldDescriptor::TYPE_BOOL    : return internal::WireFormatLite::kBoolSize;
    case FieldDescriptor::TYPE_ENUM    : return -1;

    case FieldDescriptor::TYPE_STRING  : return -1;
    case FieldDescriptor::TYPE_BYTES   : return -1;
    case FieldDescriptor::TYPE_GROUP   : return -1;
    case FieldDescriptor::TYPE_MESSAGE : return -1;

    // No default because we want the compiler to complain if any new
    // types are added.
  }
  GOOGLE_LOG(FATAL) << "Can't get here.";
  return -1;
}

static const char base64_chars[] =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

std::string StringToBase64(const std::string& input) {
  std::string result;
  size_t remaining = input.size();
  const unsigned char *src = (const unsigned char*) input.c_str();
  while (remaining > 2) {
    result += base64_chars[src[0] >> 2];
    result += base64_chars[((src[0] & 0x3) << 4) | (src[1] >> 4)];
    result += base64_chars[((src[1] & 0xf) << 2) | (src[2] >> 6)];
    result += base64_chars[src[2] & 0x3f];
    remaining -= 3;
    src += 3;
  }
  switch (remaining) {
    case 2:
      result += base64_chars[src[0] >> 2];
      result += base64_chars[((src[0] & 0x3) << 4) | (src[1] >> 4)];
      result += base64_chars[(src[1] & 0xf) << 2];
      result += '=';
      src += 2;
      break;
    case 1:
      result += base64_chars[src[0] >> 2];
      result += base64_chars[((src[0] & 0x3) << 4)];
      result += '=';
      result += '=';
      src += 1;
      break;
  }
  return result;
}

std::string FileDescriptorToBase64(const FileDescriptor* descriptor) {
  std::string fdp_bytes;
  FileDescriptorProto fdp;
  descriptor->CopyTo(&fdp);
  fdp.SerializeToString(&fdp_bytes);
  return StringToBase64(fdp_bytes);
}

/// The following code is Copyright 2018, Zynga
std::string GetEventDataType(const FieldDescriptor* descriptor) {
  switch (descriptor->type()) {
        case FieldDescriptor::TYPE_ENUM:
         return "U32";
        break;
        case FieldDescriptor::TYPE_DOUBLE:
          return "R64";
        break;
        case FieldDescriptor::TYPE_FLOAT:
          return "R32";
        break;
        case FieldDescriptor::TYPE_INT64:
          return "I64";
        break;
        case FieldDescriptor::TYPE_UINT64:
          return "U64";
        break;
        case FieldDescriptor::TYPE_INT32:
          return "I32";
        break;
        case FieldDescriptor::TYPE_FIXED64:
          return "F64";
        break;
        case FieldDescriptor::TYPE_FIXED32:
          return "F32";
        break;
        case FieldDescriptor::TYPE_BOOL:
          return "BoolData";
        break;
        case FieldDescriptor::TYPE_STRING:
          return "StringData";
        break;
        case FieldDescriptor::TYPE_MESSAGE:
        case FieldDescriptor::TYPE_BYTES:
          return "ByteData";
        break;
        case FieldDescriptor::TYPE_UINT32:
          return "U32";
        break;
        case FieldDescriptor::TYPE_SFIXED32:
          return "SF32";
        break;
        case FieldDescriptor::TYPE_SFIXED64:
          return "SF64";
        break;
        case FieldDescriptor::TYPE_SINT32:
          return "SI32";
        break;
        case FieldDescriptor::TYPE_SINT64:
          return "SI64";
        break;
       default:
         GOOGLE_LOG(FATAL)<< "GetEventDataType Unknown field type." << descriptor->type();
         return "";
    }
}

std::string GetByteStringRead(const FieldDescriptor* descriptor) {
  switch (descriptor->type()) {
        case FieldDescriptor::TYPE_ENUM:
         return "ReadEnum";
        break;
        case FieldDescriptor::TYPE_DOUBLE:
          return "ReadDouble";
        break;
        case FieldDescriptor::TYPE_FLOAT:
          return "ReadFloat";
        break;
        case FieldDescriptor::TYPE_INT64:
          return "ReadInt64";
        break;
        case FieldDescriptor::TYPE_UINT64:
          return "ReadUInt64";
        break;
        case FieldDescriptor::TYPE_INT32:
          return "ReadInt32";
        break;
        case FieldDescriptor::TYPE_FIXED64:
          return "ReadFixed64";
        break;
        case FieldDescriptor::TYPE_FIXED32:
          return "ReadFixed32";
        break;
        case FieldDescriptor::TYPE_BOOL:
          return "ReadBool";
        break;
        case FieldDescriptor::TYPE_STRING:
          return "ReadString";
        break;
        case FieldDescriptor::TYPE_MESSAGE:
          return "ReadMessage";
        break;
        case FieldDescriptor::TYPE_BYTES:
          return "ReadBytes";
        break;
        case FieldDescriptor::TYPE_UINT32:
          return "ReadUInt32";
        break;
        case FieldDescriptor::TYPE_SFIXED32:
          return "ReadSFixed32";
        break;
        case FieldDescriptor::TYPE_SFIXED64:
          return "ReadSFixed64";
        break;
        case FieldDescriptor::TYPE_SINT32:
          return "ReadSInt32";
        break;
        case FieldDescriptor::TYPE_SINT64:
          return "ReadSInt64";
        break;
       default:
         GOOGLE_LOG(FATAL)<< "GetByteStringRead Unknown field type." << descriptor->type();
         return "";
    }
}

std::string GetByteStringWrite(const FieldDescriptor* descriptor) {
  switch (descriptor->type()) {
        case FieldDescriptor::TYPE_ENUM:
         return "WriteEnum";
        break;
        case FieldDescriptor::TYPE_DOUBLE:
          return "WriteDouble";
        break;
        case FieldDescriptor::TYPE_FLOAT:
          return "WriteFloat";
        break;
        case FieldDescriptor::TYPE_INT64:
          return "WriteInt64";
        break;
        case FieldDescriptor::TYPE_UINT64:
          return "WriteUInt64";
        break;
        case FieldDescriptor::TYPE_INT32:
          return "WriteInt32";
        break;
        case FieldDescriptor::TYPE_FIXED64:
          return "WriteFixed64";
        break;
        case FieldDescriptor::TYPE_FIXED32:
          return "WriteFixed32";
        break;
        case FieldDescriptor::TYPE_BOOL:
          return "WriteBool";
        break;
        case FieldDescriptor::TYPE_STRING:
          return "WriteString";
        break;
        case FieldDescriptor::TYPE_MESSAGE:
          return "WriteMessage";
        break;
        case FieldDescriptor::TYPE_BYTES:
          return "WriteBytes";
        break;
        case FieldDescriptor::TYPE_UINT32:
          return "WriteUInt32";
        break;
        case FieldDescriptor::TYPE_SFIXED32:
          return "WriteSFixed32";
        break;
        case FieldDescriptor::TYPE_SFIXED64:
          return "WriteSFixed64";
        break;
        case FieldDescriptor::TYPE_SINT32:
          return "WriteSInt32";
        break;
        case FieldDescriptor::TYPE_SINT64:
          return "WriteSInt64";
        break;
       default:
         GOOGLE_LOG(FATAL)<< "GetByteStringWrite Unknown field type." << descriptor->type();
         return "";
    }
}
///

FieldGeneratorBase* CreateFieldGenerator(const FieldDescriptor* descriptor,
                                         int fieldOrdinal,
                                         const Options* options) {
  switch (descriptor->type()) {
    case FieldDescriptor::TYPE_GROUP:
    case FieldDescriptor::TYPE_MESSAGE:
      if (descriptor->is_repeated()) {
        if (descriptor->is_map()) {
          return new MapFieldGenerator(descriptor, fieldOrdinal, options);
        } else {
          return new RepeatedMessageFieldGenerator(descriptor, fieldOrdinal, options);
        }
      } else {
        if (IsWrapperType(descriptor)) {
          if (descriptor->containing_oneof()) {
            return new WrapperOneofFieldGenerator(descriptor, fieldOrdinal, options);
          } else {
            return new WrapperFieldGenerator(descriptor, fieldOrdinal, options);
          }
        } else {
          if (descriptor->containing_oneof()) {
            return new MessageOneofFieldGenerator(descriptor, fieldOrdinal, options);
          } else {
            return new MessageFieldGenerator(descriptor, fieldOrdinal, options);
          }
        }
      }
    case FieldDescriptor::TYPE_ENUM:
      if (descriptor->is_repeated()) {
        return new RepeatedEnumFieldGenerator(descriptor, fieldOrdinal, options);
      } else {
        if (descriptor->containing_oneof()) {
          return new EnumOneofFieldGenerator(descriptor, fieldOrdinal, options);
        } else {
          return new EnumFieldGenerator(descriptor, fieldOrdinal, options);
        }
      }
    default:
      if (descriptor->is_repeated()) {
        return new RepeatedPrimitiveFieldGenerator(descriptor, fieldOrdinal, options);
      } else {
        if (descriptor->containing_oneof()) {
          return new PrimitiveOneofFieldGenerator(descriptor, fieldOrdinal, options);
        } else {
          return new PrimitiveFieldGenerator(descriptor, fieldOrdinal, options);
        }
      }
  }
}

}  // namespace csharp
}  // namespace compiler
}  // namespace protobuf
}  // namespace google
