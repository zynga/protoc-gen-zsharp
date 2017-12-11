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
    public abstract class EventRegistry
    {
        protected EventSourceRoot _root = new EventSourceRoot();

        public abstract bool ApplyEvents(EventSourceRoot root, int startIndex = 0);
        public abstract void AddEvent<T>(int fieldNumber, EventAction action, T data);
        public abstract EventSourceRoot CollectEvents();
    }
}
