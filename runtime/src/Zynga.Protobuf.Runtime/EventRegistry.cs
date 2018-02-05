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
        public List<EventData> _root = new List<EventData>();
        protected int  _indexRemoveCount = 0;
        protected int  _lastIndexRemove = int.MaxValue;

        public void ApplyEvents(EventSourceRoot root) {
            _indexRemoveCount = 0;
            _lastIndexRemove = int.MaxValue;

            for(int index = 0; index < root.Events.Count; ++index) {
                var e = root.Events[index];
                var currentPathIndex = 0;

                ApplyEvent(e, currentPathIndex);
            }
        }

        public abstract bool ApplyEvent(EventData e, int pathIndex);
        public abstract void AddEvent<T>(int fieldNumber, EventAction action, T data);
        public abstract EventContent GetEventData<T>(int fieldNumber, EventAction action, T data);
        public abstract bool ApplySnapshot(EventSourceRoot root);
        public abstract EventSourceRoot GenerateSnapshot();

        public EventSourceRoot GenerateEvents() {
            var er = new EventSourceRoot();
            er.Events.AddRange(_root);
            return er;
        }

        public virtual void SetRoot(List<EventData> inRoot) {
            _root = inRoot;
        }

        public void Reset() {
            _root.Clear();
            _indexRemoveCount = 0;
            _lastIndexRemove = int.MaxValue;
        }

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
