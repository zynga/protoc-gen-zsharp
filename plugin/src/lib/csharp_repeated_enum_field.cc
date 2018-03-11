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
#include <google/protobuf/wire_format.h>

#include "src/lib/csharp_doc_comment.h"
#include "src/lib/csharp_helpers.h"
#include "src/lib/csharp_repeated_enum_field.h"

using namespace google::protobuf;

namespace zynga {
namespace protobuf {
namespace compiler {
namespace csharp {

RepeatedEnumFieldGenerator::RepeatedEnumFieldGenerator(
    const FieldDescriptor* descriptor, int fieldOrdinal, const Options *options)
    : FieldGeneratorBase(descriptor, fieldOrdinal, options) {
}

RepeatedEnumFieldGenerator::~RepeatedEnumFieldGenerator() {

}

void RepeatedEnumFieldGenerator::GenerateMembers(io::Printer* printer, bool isEventSourced) {
  printer->Print(
    variables_,
    "private static readonly pb::FieldCodec<$type_name$> _repeated_$name$_codec\n"
    "    = pb::FieldCodec.ForEnum($tag$, x => (int) x, x => ($type_name$) x);\n");

  // The following code is Copyright 2018, Zynga
  if(isEventSourced) {
    variables_["data_value"] = GetEventDataType(descriptor_);
    printer->Print(
      variables_,
      "public class $property_name$DataConverter: EventDataConverter<$type_name$> {\n"
      "  public override zpr.EventSource.EventContent GetEventData($type_name$ data) {\n"
      "    return new zpr.EventSource.EventContent() { $data_value$ = Convert.ToUInt32(data) };\n"
      "  }\n"
      "  public override $type_name$ GetItem(zpr.EventSource.EventContent data) {\n"
      "    return ($type_name$) data.U32;\n"
      "  }\n"
      "}\n"
      "private static $property_name$DataConverter $name$DataConverter = new $property_name$DataConverter();\n"
      );

    printer->Print(
      variables_,
      "private readonly EventRepeatedField<$type_name$> $name$_ = new EventRepeatedField<$type_name$>($name$DataConverter);\n");
  }
  else {
    printer->Print(
      variables_,
      "private readonly pbc::RepeatedField<$type_name$> $name$_ = new pbc::RepeatedField<$type_name$>();\n");
  }

  WritePropertyDocComment(printer, descriptor_);
  AddPublicMemberAttributes(printer);

  if(isEventSourced) {
    printer->Print(
      variables_,
      "$access_level$ EventRepeatedField<$type_name$> $property_name$ {\n"
      "  get { return $name$_; }\n"
    "}\n");
  }
  else {
    printer->Print(
      variables_,
      "$access_level$ pbc::RepeatedField<$type_name$> $property_name$ {\n"
      "  get { return $name$_; }\n"
    "}\n");
  }
}

void RepeatedEnumFieldGenerator::GenerateEventSource(io::Printer* printer) {
    printer->Print(
          variables_,
          "        $name$_.ApplyEvent(e.ListEvent);\n");
}

void RepeatedEnumFieldGenerator::GenerateEventAdd(io::Printer* printer, bool isMap) {
  std::map<string, string> vars;
  vars["data_value"] = GetEventDataType(descriptor_);
  vars["type_name"] = variables_["type_name"];
  printer->Print(vars, "        return new zpr.EventSource.EventContent() { $data_value$ = Convert.ToUInt32(data) };\n");

}

void RepeatedEnumFieldGenerator::GenerateEventAddEvent(io::Printer* printer) {
  printer->Print(
    "        e.Path.AddRange(this.Path.$field_name$Path._path);\n",
    "field_name", GetPropertyName(descriptor_));
}

void RepeatedEnumFieldGenerator::GenerateCheckSum(io::Printer* printer) {
  if (checksum_exclude()) return;

  // we need to iterate over the lists
  printer->Print(
      variables_,
      "foreach (var item in $name$_) {\n"
      "    inWriter.Write((int)item);\n"
      "}\n");
}

void RepeatedEnumFieldGenerator::GenerateMergingCode(io::Printer* printer, bool isEventSourced) {
  printer->Print(
    variables_,
    "$name$_.Add(other.$name$_);\n");
}

void RepeatedEnumFieldGenerator::GenerateParsingCode(io::Printer* printer, bool isEventSourced) {
  printer->Print(
    variables_,
    "$name$_.AddEntriesFrom(input, _repeated_$name$_codec);\n");
}

void RepeatedEnumFieldGenerator::GenerateSerializationCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "$name$_.WriteTo(output, _repeated_$name$_codec);\n");
}

void RepeatedEnumFieldGenerator::GenerateSerializedSizeCode(io::Printer* printer) {  
  printer->Print(
    variables_,
    "size += $name$_.CalculateSize(_repeated_$name$_codec);\n");
}

void RepeatedEnumFieldGenerator::WriteHash(io::Printer* printer) {
  printer->Print(
    variables_,
    "hash ^= $name$_.GetHashCode();\n");
}

void RepeatedEnumFieldGenerator::WriteEquals(io::Printer* printer) {
  printer->Print(
    variables_,
    "if(!$name$_.Equals(other.$name$_)) return false;\n");
}

void RepeatedEnumFieldGenerator::WriteToString(io::Printer* printer) {
  printer->Print(variables_,
    "PrintField(\"$descriptor_name$\", $name$_, writer);\n");
}

void RepeatedEnumFieldGenerator::GenerateCloningCode(io::Printer* printer, bool isEventSourced) {
  if(isEventSourced) {
    printer->Print(variables_,
      "$name$_ = new EventRepeatedField<$type_name$>($name$DataConverter, other.$property_name$.Clone());\n"
      "$name$_.SetContext(Context, $number$);\n");
  }
  else {
    printer->Print(variables_,
      "$name$_ = other.$name$_.Clone();\n");
  }
}

void RepeatedEnumFieldGenerator::GenerateFreezingCode(io::Printer* printer) {
}

}  // namespace csharp
}  // namespace compiler
}  // namespace protobuf
}  // namespace google
