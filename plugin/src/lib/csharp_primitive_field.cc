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

#include <sstream>

#include <google/protobuf/compiler/code_generator.h>
#include <google/protobuf/compiler/plugin.h>
#include <google/protobuf/descriptor.h>
#include <google/protobuf/descriptor.pb.h>
#include <google/protobuf/io/printer.h>
#include <google/protobuf/io/zero_copy_stream.h>
#include <google/protobuf/stubs/strutil.h>

#include "src/lib/csharp_doc_comment.h"
#include "src/lib/csharp_helpers.h"
#include "src/lib/csharp_options.h"
#include "src/lib/csharp_primitive_field.h"

using namespace google::protobuf;

namespace zynga {
namespace protobuf {
namespace compiler {
namespace csharp {

PrimitiveFieldGenerator::PrimitiveFieldGenerator(
    const FieldDescriptor* descriptor, int fieldOrdinal, const Options *options)
    : FieldGeneratorBase(descriptor, fieldOrdinal, options) {
  // TODO(jonskeet): Make this cleaner...
  is_value_type = descriptor->type() != FieldDescriptor::TYPE_STRING
      && descriptor->type() != FieldDescriptor::TYPE_BYTES;
  if (!is_value_type) {
    variables_["has_property_check"] = variables_["property_name"] + ".Length != 0";
    variables_["other_has_property_check"] = "other." + variables_["property_name"] + ".Length != 0";
  }
}

PrimitiveFieldGenerator::~PrimitiveFieldGenerator() {
}

void PrimitiveFieldGenerator::GenerateMembers(io::Printer* printer, bool isEventSourced) {
  // TODO(jonskeet): Work out whether we want to prevent the fields from ever being
  // null, or whether we just handle it, in the cases of bytes and string.
  // (Basically, should null-handling code be in the getter or the setter?)
  printer->Print(
    variables_,
    "private $type_name$ $name_def_message$;\n");
  WritePropertyDocComment(printer, descriptor_);
  AddPublicMemberAttributes(printer);
  printer->Print(
    variables_,
    "$access_level$ $type_name$ $property_name$ {\n"
    "  get { return $name$_; }\n"
    "  set {\n");
  if (isEventSourced) {
      switch (descriptor_->type()) {
        case FieldDescriptor::TYPE_ENUM:
        case FieldDescriptor::TYPE_DOUBLE:
        case FieldDescriptor::TYPE_FLOAT:
        case FieldDescriptor::TYPE_INT64:
        case FieldDescriptor::TYPE_UINT64:
        case FieldDescriptor::TYPE_INT32:
        case FieldDescriptor::TYPE_FIXED64:
        case FieldDescriptor::TYPE_FIXED32:
        case FieldDescriptor::TYPE_BOOL:
        case FieldDescriptor::TYPE_STRING:
        case FieldDescriptor::TYPE_BYTES:
        case FieldDescriptor::TYPE_UINT32:
        case FieldDescriptor::TYPE_SFIXED32:
        case FieldDescriptor::TYPE_SFIXED64:
        case FieldDescriptor::TYPE_SINT32:
        case FieldDescriptor::TYPE_SINT64:
        {
          if (is_value_type) { 
            printer->Print(
              variables_,
              "    AddEvent($number$, zpr.EventSource.EventAction.Set, value);\n");
          }
          else {
            printer->Print(
              variables_,
              "    AddEvent($number$, zpr.EventSource.EventAction.Set, pb::ProtoPreconditions.CheckNotNull(value, \"value\"));\n");
          }
        }
        
        break;
      default:
        GOOGLE_LOG(FATAL)<< "Unknown field type.";
      }
  }

  if (is_value_type) {
    printer->Print(
      variables_,
      "    $name$_ = value;\n");
  } else {
    printer->Print(
      variables_,
      "    $name$_ = pb::ProtoPreconditions.CheckNotNull(value, \"value\");\n");
  }
  printer->Print(
    "  }\n"
    "}\n");
}

void PrimitiveFieldGenerator::GenerateEventSource(io::Printer* printer) {
    std::map<string, string> vars;
    vars["name"] = variables_["name"];
    vars["data_value"] = GetEventDataType(descriptor_);
    vars["type_name"] = variables_["type_name"];

    if (descriptor_->type() == FieldDescriptor::TYPE_ENUM) {
      printer->Print(
        vars,
        "        $name$_ = ($type_name$)e.Data.$data_value$;\n");
    }
    else 
    {
      printer->Print(
        vars,
        "        $name$_ = e.Data.$data_value$;\n");
    }
    
}

void PrimitiveFieldGenerator::GenerateEventAdd(io::Printer* printer, bool isMap) {
  std::map<string, string> vars;
  vars["data_value"] = GetEventDataType(descriptor_);
  vars["type_name"] = variables_["type_name"];
  printer->Print(vars, "        return new zpr.EventSource.EventContent() { data_ = data, dataCase_ = zpr.EventSource.EventContent.DataOneofCase.$data_value$ };\n");
}


void PrimitiveFieldGenerator::GenerateEventAddEvent(io::Printer* printer) {
  printer->Print(
    "        e.Path.AddRange(this.Path.$field_name$Path._path);\n",
    "field_name", GetPropertyName(descriptor_));
}

void PrimitiveFieldGenerator::GenerateCheckSum(io::Printer* printer) {
  if (checksum_exclude()) return;

  if (descriptor_->type() == FieldDescriptor::TYPE_ENUM)
  {
    printer->Print(
        variables_,
        "if ($has_property_check$) inWriter.Write((int)$name$_);\n");
  }
  else if (descriptor_->type() == FieldDescriptor::TYPE_BYTES)
  {
    printer->Print(
        variables_,
        "if ($has_property_check$) inWriter.Write($name$_.ToByteArray());\n");
  }
  else
  {
    printer->Print(
        variables_,
        "if ($has_property_check$) inWriter.Write($property_name$);\n");
  }
}


void PrimitiveFieldGenerator::GenerateMergingCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($other_has_property_check$) {\n"
    "  $property_name$ = other.$property_name$;\n"
    "}\n");
}

void PrimitiveFieldGenerator::GenerateParsingCode(io::Printer* printer) {
  // Note: invoke the property setter rather than writing straight to the field,
  // so that we can normalize "null to empty" for strings and bytes.
  printer->Print(
    variables_,
    "$property_name$ = input.Read$capitalized_type_name$();\n");
}

void PrimitiveFieldGenerator::GenerateSerializationCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($has_property_check$) {\n"
    "  output.WriteRawTag($tag_bytes$);\n"
    "  output.Write$capitalized_type_name$($property_name$);\n"
    "}\n");
}

void PrimitiveFieldGenerator::GenerateSerializedSizeCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($has_property_check$) {\n");
  printer->Indent();
  int fixedSize = GetFixedSize(descriptor_->type());
  if (fixedSize == -1) {
    printer->Print(
      variables_,
      "size += $tag_size$ + pb::CodedOutputStream.Compute$capitalized_type_name$Size($property_name$);\n");
  } else {
    printer->Print(
      "size += $tag_size$ + $fixed_size$;\n",
      "fixed_size", SimpleItoa(fixedSize),
      "tag_size", variables_["tag_size"]);
  }
  printer->Outdent();
  printer->Print("}\n");
}

void PrimitiveFieldGenerator::WriteHash(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($has_property_check$) hash ^= $property_name$.GetHashCode();\n");
}
void PrimitiveFieldGenerator::WriteEquals(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($property_name$ != other.$property_name$) return false;\n");
}
void PrimitiveFieldGenerator::WriteToString(io::Printer* printer) {
  printer->Print(
    variables_,
    "PrintField(\"$descriptor_name$\", $has_property_check$, $property_name$, writer);\n");
}

void PrimitiveFieldGenerator::GenerateCloningCode(io::Printer* printer, bool isEventSourced) {
  printer->Print(variables_,
    "$name$_ = other.$name$_;\n");
}

void PrimitiveFieldGenerator::GenerateCodecCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "pb::FieldCodec.For$capitalized_type_name$($tag$)");
}

PrimitiveOneofFieldGenerator::PrimitiveOneofFieldGenerator(
    const FieldDescriptor* descriptor, int fieldOrdinal, const Options *options)
    : PrimitiveFieldGenerator(descriptor, fieldOrdinal, options) {
  SetCommonOneofFieldVariables(&variables_);
}

PrimitiveOneofFieldGenerator::~PrimitiveOneofFieldGenerator() {
}

void PrimitiveOneofFieldGenerator::GenerateMembers(io::Printer* printer, bool isEventSourced) {
  WritePropertyDocComment(printer, descriptor_);
  AddPublicMemberAttributes(printer);
  printer->Print(
    variables_,
    "$access_level$ $type_name$ $property_name$ {\n"
    "  get { return $has_property_check$ ? ($type_name$) $oneof_name$_ : $default_value$; }\n"
    "  set {\n");
    /// The following code is Copyright 2018, Zynga
    // We check and see if we are event sourced
    // if we are then we can add an event to this object
    if (isEventSourced) {
      switch (descriptor_->type()) {
        case FieldDescriptor::TYPE_ENUM:
        case FieldDescriptor::TYPE_DOUBLE:
        case FieldDescriptor::TYPE_FLOAT:
        case FieldDescriptor::TYPE_INT64:
        case FieldDescriptor::TYPE_UINT64:
        case FieldDescriptor::TYPE_INT32:
        case FieldDescriptor::TYPE_FIXED64:
        case FieldDescriptor::TYPE_FIXED32:
        case FieldDescriptor::TYPE_BOOL:
        case FieldDescriptor::TYPE_STRING:
        case FieldDescriptor::TYPE_BYTES:
        case FieldDescriptor::TYPE_UINT32:
        case FieldDescriptor::TYPE_SFIXED32:
        case FieldDescriptor::TYPE_SFIXED64:
        case FieldDescriptor::TYPE_SINT32:
        case FieldDescriptor::TYPE_SINT64: 
        {
          if (is_value_type) { 
            printer->Print(
              variables_,
              "    AddEvent($number$, zpr.EventSource.EventAction.Set, value);\n");
          }
          else {
            printer->Print(
              variables_,
              "    AddEvent($number$, zpr.EventSource.EventAction.Set, pb::ProtoPreconditions.CheckNotNull(value, \"value\"));\n");
          }
        }
        
        break;
      default:
        GOOGLE_LOG(FATAL)<< "Unknown field type.";
      }
    }
    ///
    if (is_value_type) {
      printer->Print(
        variables_,
        "    $oneof_name$_ = value;\n");
    } else {
      printer->Print(
        variables_,
        "    $oneof_name$_ = pb::ProtoPreconditions.CheckNotNull(value, \"value\");\n");
    }
    printer->Print(
      variables_,
      "    $oneof_name$Case_ = $oneof_property_name$OneofCase.$property_name$;\n"
      "  }\n"
      "}\n");
}

void PrimitiveOneofFieldGenerator::GenerateEventSource(io::Printer* printer) {
    std::map<string, string> vars;
    vars["oneof_name"] = variables_["oneof_name"];
    vars["oneof_property_name"] = variables_["oneof_property_name"];
    vars["property_name"] = variables_["property_name"];
    vars["data_value"] = GetEventDataType(descriptor_);
    vars["type_name"] = variables_["type_name"];

    if (is_value_type) {
      printer->Print(
        vars,
        "        $oneof_name$_ = e.Data.$data_value$;\n");
    } else {
      printer->Print(
        vars,
        "        $oneof_name$_ = pb::ProtoPreconditions.CheckNotNull(e.Data.$data_value$, \"value\");\n");
    }

    printer->Print(
      variables_,
      "        $oneof_name$Case_ = $oneof_property_name$OneofCase.$property_name$;\n");

    
}

void PrimitiveOneofFieldGenerator::GenerateEventAdd(io::Printer* printer, bool isMap) {
  std::map<string, string> vars;
  vars["name"] = variables_["name"];
  vars["data_value"] = GetEventDataType(descriptor_);
  vars["type_name"] = variables_["type_name"];

  printer->Print(vars, "        return new zpr.EventSource.EventContent() { data_ = data, dataCase_ = zpr.EventSource.EventContent.DataOneofCase.$data_value$ };\n");
}

void PrimitiveOneofFieldGenerator::GenerateEventAddEvent(io::Printer* printer) {
  printer->Print(
    "        e.Path.AddRange(this.Path.$field_name$Path._path);\n",
    "field_name", GetPropertyName(descriptor_));
}

void PrimitiveOneofFieldGenerator::WriteToString(io::Printer* printer) {
  printer->Print(variables_,
    "PrintField(\"$descriptor_name$\", $has_property_check$, $oneof_name$_, writer);\n");
}

void PrimitiveOneofFieldGenerator::GenerateParsingCode(io::Printer* printer) {
    printer->Print(
      variables_,
      "$property_name$ = input.Read$capitalized_type_name$();\n");
}

void PrimitiveOneofFieldGenerator::GenerateCloningCode(io::Printer* printer, bool isEventSourced) {
  printer->Print(variables_,
    "$property_name$ = other.$property_name$;\n");
}

}  // namespace csharp
}  // namespace compiler
}  // namespace protobuf
}  // namespace google
