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
#include "src/lib/csharp_message_field.h"
#include "src/lib/csharp_options.h"
#include "src/lib/generated/event_plugin.pb.h"


using namespace google::protobuf;

namespace zynga {
namespace protobuf {
namespace compiler {
namespace csharp {

MessageFieldGenerator::MessageFieldGenerator(const FieldDescriptor* descriptor,
                                             int fieldOrdinal,
                                             const Options *options)
    : FieldGeneratorBase(descriptor, fieldOrdinal, options) {
  variables_["has_property_check"] = name() + "_ != null";
  variables_["has_not_property_check"] = name() + "_ == null";
}

MessageFieldGenerator::~MessageFieldGenerator() {

}

void MessageFieldGenerator::GenerateMembers(io::Printer* printer, bool isEventSourced) {
  bool isEnternalSourced = IsInternalEventSourced();

  printer->Print(
    variables_,
    "private $type_name$ $name$_;\n");
  WritePropertyDocComment(printer, descriptor_);
  AddPublicMemberAttributes(printer);
  printer->Print(
    variables_,
    "$access_level$ $type_name$ $property_name$ {\n"
    "  get { return $name$_; }\n"
    "  set {\n");
  
  if (isEventSourced) {
    if (isEnternalSourced) {
       printer->Print(
         variables_,
         "    if($name$_ != null) $name$_.ClearParent();\n"
         "    value.SetParent(Context, new EventPath(Context.Path, $number$));\n");
    }

    printer->Print(
       variables_,
       "    #if !DISABLE_EVENTS\n");
    printer->Print(variables_, "    if(value == null || !value.Equals($name$_)) {\n");
    printer->Print(
       variables_,
       "      Context.AddSetEvent($number$, new zpr.EventSource.EventContent { ByteData = value.ToByteString() });\n");
    printer->Print(variables_, "    }\n");
    printer->Print(
       variables_,
       "    #endif\n");
  }
  
  printer->Print(
    variables_,
    "    $name$_ = value;\n"
    "  }\n"
    "}\n");
}

void MessageFieldGenerator::GenerateEventSource(io::Printer* printer) {
  bool isEventSourced = IsInternalEventSourced();
  if (isEventSourced) {
    printer->Print(variables_,
      "        if (e.Path.Count - 1 != pathIndex) {\n"
      "          if ($name$_ == null) {\n"
      "            $name$_ = new $type_name$();\n"
      "            $name$_.SetParent(Context, new EventPath(Context.Path, $number$));\n"
      "          }\n"
      "          ($name$_ as zpr::IEventRegistry)?.ApplyEvent(e, pathIndex + 1);\n"
      "        } else {\n"
      "          $name$_  = $type_name$.Parser.ParseFrom(e.Set.ByteData);\n"
      "          $name$_.SetParent(Context, new EventPath(Context.Path, $number$));\n"
      "        }\n");
  }
  else {
    printer->Print(variables_, 
      "        $name$_  = $type_name$.Parser.ParseFrom(e.Set.ByteData);\n");
  }
}


void MessageFieldGenerator::GenerateEventAdd(io::Printer* printer, bool isMap) {
  std::map<string, string> vars;
  vars["data_value"] = GetEventDataType(descriptor_);
  vars["type_name"] = variables_["type_name"];
  vars["name"] = variables_["name"];

  // if (!isMap) {
  //   printer->Print(vars, "        if ($type_name$.IsEventSourced) return null;\n");
  // }

  printer->Print(vars, "        var byteData$name$ = (data as pb::IMessage)?.ToByteString();\n");
  printer->Print(vars, "        return new zpr.EventSource.EventContent() { $data_value$ = byteData$name$ };\n");
}

void MessageFieldGenerator::GenerateEventAddEvent(io::Printer* printer) {
  bool isEventSourced = IsInternalEventSourced();
  if (isEventSourced) {
    printer->Print(
      "        e.Path.AddRange(this.Path.$field_name$Path.Path._path);\n",
      "field_name", GetPropertyName(descriptor_));
  }
  else {
    printer->Print(
      "        e.Path.AddRange(this.Path.$field_name$Path._path);\n",
      "field_name", GetPropertyName(descriptor_));
  }
  
}

void MessageFieldGenerator::GenerateCheckSum(io::Printer* printer) {
  if (checksum_exclude()) return;
  printer->Print(
    variables_,
    "if ($has_property_check$) $property_name$.GetChecksum(inWriter);\n");
}


void MessageFieldGenerator::GenerateMergingCode(io::Printer* printer, bool isEventSourced) {
  bool isInternalEventSourced = IsInternalEventSourced();
  printer->Print(
    variables_,
    "if (other.$has_property_check$) {\n"
    "  if ($has_not_property_check$) {\n"
    "    $name$_ = new $type_name$();\n");
  if(isEventSourced && isInternalEventSourced) {
    printer->Print(variables_,
      "    $name$_.SetParent(Context, new EventPath(Context.Path, $number$));\n");
  }
  printer->Print(
    variables_,
    "  }\n"
    "  $property_name$.MergeFrom(other.$property_name$);\n"
    "}\n");
}

void MessageFieldGenerator::GenerateParsingCode(io::Printer* printer, bool isEventSourced) {
  bool isInternalEventSourced = IsInternalEventSourced();
  printer->Print(
    variables_,
    "if ($has_not_property_check$) {\n"
    "  $name$_ = new $type_name$();\n");

  if(isEventSourced && isInternalEventSourced) {
    printer->Print(variables_,
      "  $name$_.SetParent(Context, new EventPath(Context.Path, $number$));\n");
  }

  printer->Print(
    variables_,
    "}\n"
    // TODO(jonskeet): Do we really need merging behaviour like this?
    "input.ReadMessage($name$_);\n"); // No need to support TYPE_GROUP...
}


void MessageFieldGenerator::GenerateSerializationCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($has_property_check$) {\n"
    "  output.WriteRawTag($tag_bytes$);\n"
    "  output.WriteMessage($property_name$);\n"
    "}\n");
}

void MessageFieldGenerator::GenerateSerializedSizeCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($has_property_check$) {\n"
    "  size += $tag_size$ + pb::CodedOutputStream.ComputeMessageSize($property_name$);\n"
    "}\n");
}

void MessageFieldGenerator::WriteHash(io::Printer* printer) {
  printer->Print(
    variables_,
    "if ($has_property_check$) hash ^= $property_name$.GetHashCode();\n");
}
void MessageFieldGenerator::WriteEquals(io::Printer* printer) {
  printer->Print(
    variables_,
    "if (!object.Equals($property_name$, other.$property_name$)) return false;\n");
}
void MessageFieldGenerator::WriteToString(io::Printer* printer) {
  variables_["field_name"] = GetFieldName(descriptor_);
  printer->Print(
    variables_,
    "PrintField(\"$field_name$\", has$property_name$, $name$_, writer);\n");
}

void MessageFieldGenerator::GenerateCloningCode(io::Printer* printer, bool isEventSourced) {
  printer->Print(variables_,
    "$name$_ = other.$has_property_check$ ? other.$property_name$.Clone() : null;\n");
}

void MessageFieldGenerator::GenerateFreezingCode(io::Printer* printer) {
}

void MessageFieldGenerator::GenerateCodecCode(io::Printer* printer) {
  printer->Print(
    variables_,
    "pb::FieldCodec.ForMessage($tag$, $type_name$.Parser)");
}

// $AS TODO: Make this function a little less crazy!
bool MessageFieldGenerator::IsInternalEventSourced() {
  bool isInternalSourced = !IsGoogleMessage(descriptor_->message_type()) 
                            && HasFileEventSource(descriptor_->file());

  if (!isInternalSourced) {
    const MessageOptions& op = descriptor_->message_type()->options();
    isInternalSourced = op.HasExtension(com::zynga::runtime::protobuf::event_sourced); 

    // we check and see if you are a nested type of the field owner if so
    // then we can assume 
    const Descriptor* parentObject = descriptor_->containing_type();
    if (parentObject && parentObject->nested_type_count() != 0) {
      const MessageOptions& parentOptions = parentObject->options();
      if (parentOptions.HasExtension(com::zynga::runtime::protobuf::event_sourced) &&
          parentObject->FindNestedTypeByName(descriptor_->message_type()->name()) != NULL) {
        isInternalSourced = true; 
      }
    }
  }
  else {
    // we need to check and see if the field is from this current file and if its not
    // then we need to check its Sourced case. 
    if (descriptor_->message_type()->file() != descriptor_->file()) {
      isInternalSourced = !IsGoogleMessage(descriptor_->message_type()) 
                            && HasFileEventSource(descriptor_->message_type()->file());

      if (!isInternalSourced) {
        const MessageOptions& op = descriptor_->message_type()->options();
        isInternalSourced = op.HasExtension(com::zynga::runtime::protobuf::event_sourced); 
      }
    }
  }

  return isInternalSourced;
}

MessageOneofFieldGenerator::MessageOneofFieldGenerator(
    const FieldDescriptor* descriptor,
	  int fieldOrdinal,
    const Options *options)
    : MessageFieldGenerator(descriptor, fieldOrdinal, options) {
  SetCommonOneofFieldVariables(&variables_);
}

MessageOneofFieldGenerator::~MessageOneofFieldGenerator() {

}

void MessageOneofFieldGenerator::GenerateMembers(io::Printer* printer, bool isEventSourced) {
  bool isInternalSourced = IsInternalEventSourced();
  WritePropertyDocComment(printer, descriptor_);
  AddPublicMemberAttributes(printer);
  printer->Print(
    variables_,
    "$access_level$ $type_name$ $property_name$ {\n"
    "  get { return $has_property_check$ ? ($type_name$) $oneof_name$_ : null; }\n"
    "  set {\n");

    if (isEventSourced) {
      if (isInternalSourced) {
        printer->Print(
          variables_,
          "    if($oneof_name$Case_ == $oneof_property_name$OneofCase.$property_name$ && $oneof_name$_ != null) (($type_name$) $oneof_name$_).ClearParent();\n"
          "    value.SetParent(Context, new EventPath(Context.Path, $number$));\n");
      }
      // if (!value.Equals(test_)) {

      printer->Print(variables_, "    #if !DISABLE_EVENTS\n");
      printer->Print(variables_, "    if($oneof_name$Case_ != $oneof_property_name$OneofCase.$property_name$ || !value.Equals($oneof_name$_)) {\n");
      printer->Print(
              variables_,
              "      Context.AddSetEvent($number$, new zpr.EventSource.EventContent { ByteData = value.ToByteString() });\n");
      printer->Print(variables_, "    }\n");
      printer->Print(variables_,"    #endif\n");
    }

    printer->Print(
    variables_,
    "    $oneof_name$_ = value;\n"
    "    $oneof_name$Case_ = value == null ? $oneof_property_name$OneofCase.None : $oneof_property_name$OneofCase.$property_name$;\n"
    "  }\n"
    "}\n");
}

void MessageOneofFieldGenerator::GenerateEventSource(io::Printer* printer) {
  bool isEventSourced = IsInternalEventSourced();
  if (isEventSourced) {
    printer->Print(variables_,
      "        if (e.Path.Count - 1 != pathIndex) {\n"
      "          if ($oneof_name$_ == null) {\n"
      "            $oneof_name$_ = new $type_name$();\n"
      "            ($oneof_name$_ as zpr::IEventRegistry)?.SetParent(Context, new EventPath(Context.Path, $number$));\n"
      "          }\n"
      "          ($oneof_name$_ as zpr::IEventRegistry)?.ApplyEvent(e, pathIndex + 1);\n"
      "        } else {\n"
      "          $oneof_name$_   = $type_name$.Parser.ParseFrom(e.Set.ByteData);\n"
      "          ($oneof_name$_ as zpr::IEventRegistry)?.SetParent(Context, new EventPath(Context.Path, $number$));\n"
      "        }\n");
  }
  else {
    printer->Print(variables_,
    "        $oneof_name$_  = $type_name$.Parser.ParseFrom(e.Set.ByteData);\n");
  }
  printer->Print(
    variables_,
    "        $oneof_name$Case_ = $oneof_name$_ == null ? $oneof_property_name$OneofCase.None : $oneof_property_name$OneofCase.$property_name$;\n");
}

void MessageOneofFieldGenerator::GenerateEventAdd(io::Printer* printer, bool isMap) {
  std::map<string, string> vars;
  vars["data_value"] = GetEventDataType(descriptor_);
  vars["type_name"] = variables_["type_name"];
  vars["name"] = variables_["name"];

  printer->Print(vars, "        var byteData$name$ = (data as pb::IMessage)?.ToByteString();\n");
  printer->Print(vars, "        return new zpr.EventSource.EventContent() { $data_value$ = byteData$name$ };\n");
}

void MessageOneofFieldGenerator::GenerateEventAddEvent(io::Printer* printer) {
  bool isEventSourced = IsInternalEventSourced();
  if (isEventSourced) {
    printer->Print(
      "        e.Path.AddRange(this.Path.$field_name$Path.Path._path);\n",
      "field_name", GetPropertyName(descriptor_));
  }
  else {
    printer->Print(
      "        e.Path.AddRange(this.Path.$field_name$Path._path);\n",
      "field_name", GetPropertyName(descriptor_));
  }
}

void MessageOneofFieldGenerator::GenerateMergingCode(io::Printer* printer, bool isEventSourced) {
  bool isInternalEventSourced = IsInternalEventSourced();
  printer->Print(variables_, 
    "if ($property_name$ == null) {\n"
    "  $property_name$ = new $type_name$();\n");
  if(isEventSourced && isInternalEventSourced) {
    printer->Print(variables_,
      "  $property_name$.SetParent(Context, new EventPath(Context.Path, $number$));\n");
  }
  printer->Print(variables_,
    "}\n"
    "$property_name$.MergeFrom(other.$property_name$);\n");
}

void MessageOneofFieldGenerator::GenerateParsingCode(io::Printer* printer, bool isEventSourced) {
  // TODO(jonskeet): We may be able to do better than this
  printer->Print(
    variables_,
    "$type_name$ subBuilder = new $type_name$();\n"
    "if ($has_property_check$) {\n"
    "  subBuilder.MergeFrom($property_name$);\n"
    "}\n"
    "input.ReadMessage(subBuilder);\n" // No support of TYPE_GROUP
    "$property_name$ = subBuilder;\n");
}

void MessageOneofFieldGenerator::WriteToString(io::Printer* printer) {
  printer->Print(
    variables_,
    "PrintField(\"$descriptor_name$\", $has_property_check$, $oneof_name$_, writer);\n");
}

void MessageOneofFieldGenerator::GenerateCloningCode(io::Printer* printer, bool isEventSourced) {
  printer->Print(variables_,
    "$property_name$ = other.$property_name$.Clone();\n");
}

}  // namespace csharp
}  // namespace compiler
}  // namespace protobuf
}  // namespace google
