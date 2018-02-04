// Protocol Buffers - Google's data interchange format
// Copyright 2015 Google Inc.  All rights reserved.
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
#include "src/lib/csharp_map_field.h"

using namespace google::protobuf;

namespace zynga {
namespace protobuf {
namespace compiler {
namespace csharp {

MapFieldGenerator::MapFieldGenerator(const FieldDescriptor* descriptor,
                                     int fieldOrdinal,
                                     const Options* options)
    : FieldGeneratorBase(descriptor, fieldOrdinal, options) {
}

MapFieldGenerator::~MapFieldGenerator() {
}

void MapFieldGenerator::GenerateMembers(io::Printer* printer, bool isEventSourced) {   
  const FieldDescriptor* key_descriptor =
      descriptor_->message_type()->FindFieldByName("key");
  const FieldDescriptor* value_descriptor =
      descriptor_->message_type()->FindFieldByName("value");
  variables_["key_type_name"] = type_name(key_descriptor);
  variables_["value_type_name"] = type_name(value_descriptor);
  scoped_ptr<FieldGeneratorBase> key_generator(
      CreateFieldGenerator(key_descriptor, 1, this->options()));
  scoped_ptr<FieldGeneratorBase> value_generator(
      CreateFieldGenerator(value_descriptor, 2, this->options()));


  variables_["key_write_name"] = GetByteStringWrite(key_descriptor);
  variables_["value_write_name"] = GetByteStringWrite(value_descriptor);

  printer->Print(
    variables_,
    "private static readonly pbc::MapField<$key_type_name$, $value_type_name$>.Codec _map_$name$_codec\n"
    "    = new pbc::MapField<$key_type_name$, $value_type_name$>.Codec(");
  key_generator->GenerateCodecCode(printer);
  printer->Print(", ");
  value_generator->GenerateCodecCode(printer);
  printer->Print(
    variables_,
    ", $tag$);\n"
    "private readonly pbc::MapField<$key_type_name$, $value_type_name$> $name$_ = new pbc::MapField<$key_type_name$, $value_type_name$>();\n");
  WritePropertyDocComment(printer, descriptor_);

  if (isEventSourced) {
    printer->Print(
      variables_,
      "$access_level$ void Add$property_name$($key_type_name$ key, $value_type_name$ value) {\n"
      " var mapEvent = new zpr.EventSource.EventMap();\n"
      " using (var memStream = new MemoryStream()) {;\n"
      "   var dataStream = new CodedOutputStream(memStream);\n"
      "   dataStream.$key_write_name$(key);\n"
      "   dataStream.$value_write_name$(value);\n"
      "   dataStream.Flush();\n"
      "   mapEvent.Data = ByteString.CopyFrom(memStream.ToArray());\n"
      " }\n"
      " AddEvent($number$, zpr.EventSource.EventAction.AddMap, mapEvent);\n"
      " $name$_.Add(key, value);\n"
    "}\n");

    printer->Print(
      variables_,
      "$access_level$ void Remove$property_name$($key_type_name$ key) {\n"
      " var mapEvent = new zpr.EventSource.EventMap();\n"
      " using (var memStream = new MemoryStream()) {;\n"
      "   var dataStream = new CodedOutputStream(memStream);\n"
      "   dataStream.$key_write_name$(key);\n"
      "   dataStream.Flush();\n"
      "   mapEvent.Data = ByteString.CopyFrom(memStream.ToArray());\n"
      " }\n"
      " AddEvent($number$, zpr.EventSource.EventAction.RemoveMap, mapEvent);\n"
      " $name$_.Remove(key);\n"
    "}\n");

    printer->Print(
      variables_,
      "$access_level$ void Clear$property_name$() {\n"
      " AddEvent($number$, zpr.EventSource.EventAction.ClearMap, -1);\n"
      " $name$_.Clear();\n"
    "}\n");

    AddPublicMemberAttributes(printer);
    printer->Print(
      variables_,
      "#if !NET35\n"
      "$access_level$ IReadOnlyDictionary<$key_type_name$, $value_type_name$> $property_name$ {\n"
      "  get { return $name$_; }\n"
      "}\n"
      "#endif\n");
  } else {
    AddPublicMemberAttributes(printer);
    printer->Print(
      variables_,
      "$access_level$ pbc::MapField<$key_type_name$, $value_type_name$> $property_name$ {\n"
      "  get { return $name$_; }\n"
    "}\n");
  }
}

void MapFieldGenerator::GenerateEventSource(io::Printer* printer) {
    const FieldDescriptor* key_descriptor =
      descriptor_->message_type()->FindFieldByName("key");
    const FieldDescriptor* value_descriptor =
      descriptor_->message_type()->FindFieldByName("value");

    std::map<string, string> vars;
    vars["name"] = variables_["name"];
    vars["key_value"] = GetEventDataType(key_descriptor);
    vars["data_value"] = GetEventDataType(value_descriptor);
    vars["type_name"] = variables_["type_name"];
    vars["key_type_name"] = type_name(key_descriptor);
    vars["value_type_name"] = type_name(value_descriptor);
    vars["key_read_name"] = GetByteStringRead(key_descriptor);
    vars["value_read_name"] = GetByteStringRead(value_descriptor);

  printer->Print(
          vars,
          "        var dataStream = e.Data.MapData.Data.CreateCodedInput();\n");
  printer->Print("        if (e.Action == zpr.EventSource.EventAction.AddMap) {\n");
  

      // if we are a message type then we need to decode the message to its actual value
      if (key_descriptor->type() == FieldDescriptor::TYPE_MESSAGE) {
        printer->Print(
          vars,
          "         var realKey$name$ = new $key_type_name$();\n"
          "         dataStream.ReadMessage(realKey$name$);\n");
      } else {
        printer->Print(
          vars,
          "         var realKey$name$ = dataStream.$key_read_name$();\n");
      }

      if (value_descriptor->type() == FieldDescriptor::TYPE_MESSAGE) {
        printer->Print(
          vars,
          "         var realValue$name$ = new $value_type_name$();\n"
          "         dataStream.ReadMessage(realValue$name$);;\n");
      } else {
        printer->Print(
          vars,
          "         var realValue$name$  = dataStream.$value_read_name$();\n");
      }
      printer->Print(
          vars,
          "         $name$_.Add(realKey$name$, realValue$name$);\n");
      printer->Print("        } else if (e.Action == zpr.EventSource.EventAction.RemoveMap) {\n");
      if (key_descriptor->type() == FieldDescriptor::TYPE_MESSAGE) {
        printer->Print(
          vars,
          "         var realKey$name$ = new $key_type_name$();\n"
          "         dataStream.ReadMessage(realKey$name$);\n");
      } else {
        printer->Print(
          vars,
          "         var realKey$name$ = dataStream.$key_read_name$();\n");
      }
      printer->Print(
          vars,
          "         $name$_.Remove(realKey$name$);\n");
      printer->Print("        } else if (e.Action == zpr.EventSource.EventAction.ClearMap) {\n");
      printer->Print(
          vars,
          "         $name$_.Clear();\n");
      printer->Print("        }\n");
}

void MapFieldGenerator::GenerateEventAdd(io::Printer* printer, bool isMap) {
  const FieldDescriptor* key_descriptor =
      descriptor_->message_type()->FindFieldByName("key");
  const FieldDescriptor* value_descriptor =
      descriptor_->message_type()->FindFieldByName("value");

  std::map<string, string> vars;
  vars["name"] = variables_["name"];
  vars["key_value"] = GetEventDataType(key_descriptor);
  vars["data_value"] = GetEventDataType(value_descriptor);
  vars["type_name"] = variables_["type_name"];
  vars["key_type_name"] = type_name(key_descriptor);
  vars["value_type_name"] = type_name(value_descriptor);
  printer->Print(vars, "        return new zpr.EventSource.EventContent() { data_ = data, dataCase_ = zpr.EventSource.EventContent.DataOneofCase.MapData };\n");
}

void MapFieldGenerator::GenerateEventAddEvent(io::Printer* printer) {
  printer->Print(
      "        e.Path.AddRange(this.Path.$field_name$Path._path);\n",
      "field_name", GetPropertyName(descriptor_));
}

void MapFieldGenerator::GenerateCheckSum(io::Printer* printer) {
  if (checksum_exclude())
    return;

  const FieldDescriptor *key_descriptor =
      descriptor_->message_type()->FindFieldByName("key");
  const FieldDescriptor *value_descriptor =
      descriptor_->message_type()->FindFieldByName("value");

  // we need to iterate over the lists
  printer->Print(
      variables_,
      "foreach (var item in $name$_) {\n");

  if (key_descriptor->type() == FieldDescriptor::TYPE_MESSAGE)
  {
    printer->Print(
      variables_,
      "    item.Key.GetChecksum(inWriter);\n");
  }
  else
  {
    printer->Print(
      variables_,
      "    inWriter.Write(item.Key);\n");
  }

  if (value_descriptor->type() == FieldDescriptor::TYPE_MESSAGE)
  {
    printer->Print(
      variables_,
      "    item.Value.GetChecksum(inWriter);\n");
  }
  else
  {
    printer->Print(
      variables_,
      "    inWriter.Write(item.Value);\n");
  }

  printer->Print(
      variables_,
      "}\n");
}

void MapFieldGenerator::GenerateMergingCode(io::Printer* printer) {
  printer->Print(
      variables_,
      "$name$_.Add(other.$name$_);\n");
}

void MapFieldGenerator::GenerateParsingCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "$name$_.AddEntriesFrom(input, _map_$name$_codec);\n");
}

void MapFieldGenerator::GenerateSerializationCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "$name$_.WriteTo(output, _map_$name$_codec);\n");
}

void MapFieldGenerator::GenerateSerializedSizeCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "size += $name$_.CalculateSize(_map_$name$_codec);\n");
}

void MapFieldGenerator::WriteHash(io::Printer* printer) {
  printer->Print(
    variables_,
    "hash ^= $name$_.GetHashCode();\n");
}
void MapFieldGenerator::WriteEquals(io::Printer* printer) {
  printer->Print(
    variables_,
    "if (!$name$_.Equals(other.$name$_)) return false;\n");
}

void MapFieldGenerator::WriteToString(io::Printer* printer) {
    // TODO: If we ever actually use ToString, we'll need to impleme this...
}

void MapFieldGenerator::GenerateCloningCode(io::Printer* printer) {
  printer->Print(variables_,
    "$name$_ = other.$name$_.Clone();\n");
}

void MapFieldGenerator::GenerateFreezingCode(io::Printer* printer) {
}

}  // namespace csharp
}  // namespace compiler
}  // namespace protobuf
}  // namespace google
