using System;
using Zynga.Protobuf.Runtime.EventSource;
using Google.Protobuf; 

namespace Zynga.Protobuf.Runtime
{
    public static class EventRegistryExt
    {
        public static EventContent GetEventContent<T>(this EventRegistry reg, T data) 
        {
            // this is a horrble c# class and thankfully we only ever use it for maps 
            // and its because they suck! 
            switch (data.GetType())
            {
                case var x when (data is uint):
                    return new EventContent() { U32 = Convert.ToUInt32(data)};
                case var t when (data is int):
                    return new EventContent() { I32 = Convert.ToInt32(data)};
                case var t when (data is ulong):
                    return new EventContent() { F64 = Convert.ToUInt64(data)};
                case var t when (data is long):
                    return new EventContent() { Sf64 = Convert.ToInt64(data)};
                case var t when (data is double):
                    return new EventContent() { R64 = Convert.ToDouble(data)};
                case var t when (data is float):
                    return new EventContent() { R32 = Convert.ToSingle(data)};
                case var t when (data is bool):
                    return new EventContent() { BoolData = Convert.ToBoolean(data)};
                case var t when (data is string):
                    return new EventContent() { StringData = Convert.ToString(data)};
               // case var t when (data is ByteString):
                    //return new EventContent() { ByteData = ByteString.FromBase64(Convert.ToBase64String(data))};
                case var t when (data is IMessage): {
                    var ms = (IMessage) data;
                    return new EventContent() { ByteData = ms.ToByteString()};
                }
                break;
                default:
                    return null;
                    break;
            }
        }
    }
}