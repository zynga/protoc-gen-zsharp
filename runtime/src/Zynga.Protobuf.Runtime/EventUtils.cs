using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zynga.Protobuf.Runtime
{
    public static class EventUtils
    {
        public static void GetChecksum(this Google.Protobuf.WellKnownTypes.Timestamp tmp, BinaryWriter inWriter)
        {
            using (var memStream = new MemoryStream())
            {
                var dataStream = new CodedOutputStream(memStream);
                tmp.WriteTo(dataStream);

                // this sucks because its going to do a copy :/
                inWriter.Write(memStream.ToArray());
            }
        }

        public static void GetChecksum(this Google.Protobuf.WellKnownTypes.Duration tmp, BinaryWriter inWriter)
        {
            using (var memStream = new MemoryStream())
            {
                var dataStream = new CodedOutputStream(memStream);
                tmp.WriteTo(dataStream);

                // this sucks because its going to do a copy :/
                inWriter.Write(memStream.ToArray());
            }
        }
    }
}
