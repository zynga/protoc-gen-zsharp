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
        protected int  _indexRemoveCount = 0;
        protected int  _lastIndexRemove = int.MaxValue;

        public abstract bool ApplyEvents(EventSourceRoot root, int startIndex = 0);
        public abstract void AddEvent<T>(int fieldNumber, EventAction action, T data);
        public abstract EventSourceRoot CollectEvents();

        protected void SafeRemoveCurrentIndex<T>(IList<T> inList, int currentIndexToRemove) {
            // this is the case where we are removing the last element in the list ? 
            if (_lastIndexRemove == currentIndexToRemove) {
                currentIndexToRemove -= 1;
            }
            else if (_lastIndexRemove < currentIndexToRemove) {
                currentIndexToRemove -= _indexRemoveCount;
            }

            if (currentIndexToRemove >= inList.Count) return;
      
            inList.RemoveAt(currentIndexToRemove);

            _lastIndexRemove = currentIndexToRemove;
            _indexRemoveCount++;
        } 
    }
}
