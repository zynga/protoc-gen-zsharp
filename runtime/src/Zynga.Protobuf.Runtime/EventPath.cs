using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime
{
    public class EventPath {
        
        public static EventPath Empty => new EventPath();
        
        public List<int> _path = new List<int>();

        public EventPath() {
            
        }
        
        public EventPath(EventPath parent, int field) {
            _path.Clear();
            _path.AddRange(parent._path);
            _path.Add(field);
        }
    }
}