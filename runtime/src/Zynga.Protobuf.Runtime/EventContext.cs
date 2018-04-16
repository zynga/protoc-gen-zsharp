using System;
using System.Collections.Generic;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// The event context does the work of managing events and the relative paths of parents and children
	/// </summary>
	public class EventContext {
		public const int UnsetPath = -1;
		private List<EventData> _events;
		private EventContext _parent;
		private bool _eventsEnabled = true;
		private List<IEventSubscribable> _dirty;
		private int _field = UnsetPath;

		/// <summary>
		/// Establishes a parent child relationship
		/// </summary>
		public void SetParent(EventContext parent, int field) {
			if (_parent != null) {
				throw new Exception("Message is already associated with another parent");
			}

			_parent = parent;
			_field = field;

			if (parent != null) {
				EventsEnabled = _parent.EventsEnabled;
			}

			// when a message is added to a parent, any delta events it currently has are no longer valid
			_events?.Clear();
		}

		/// <summary>
		/// If the associated message has a parent, this will be the field id of the message for the parent
		/// </summary>
		public int Field {
			get { return _field; }
		}

		/// <summary>
		/// The current parent message
		/// </summary>
		public EventContext Parent {
			get { return _parent; }
		}

		/// <summary>
		/// Removes the parent child relationship
		/// </summary>
		public void ClearParent() {
			_parent = null;
			_field = UnsetPath;
		}

		/// <summary>
		/// Clears any events that have been generated
		/// </summary>
		public void ClearEvents() {
			_events?.Clear();
		}

		/// <summary>
		/// Used to track changes when applying events
		/// </summary>
		public virtual void MarkDirty(IEventSubscribable subscribable) {
			if (_parent != null) {
				_parent.MarkDirty(subscribable);
			}
			else {
				// We can't use a HashSet to dedupe subscribers as the HashCode of the subscriber can change over
				// the course of replaying events.  Instead we use a list and do a reference check on each existing subscriber.
				bool found = false;
				if (_dirty != null) {
					for (int i = 0; i < _dirty.Count; i++) {
						if (ReferenceEquals(subscribable, _dirty[i])) {
							found = true;
							break;
						}
					}
				}

				if (!found) {
					if (_dirty == null) {
						_dirty = new List<IEventSubscribable>();
					}

					_dirty.Add(subscribable);
				}
			}
		}

		/// <summary>
		/// Notifies all subscribers of changes to messages
		/// </summary>
		public void NotifySubscribers() {
			try {
				if (_dirty != null) {
					foreach (var registry in _dirty) {
						registry.NotifySubscribers();
					}
				}
			}
			finally {
				_dirty?.Clear();
			}
		}

		/// <summary>
		/// The current events of the context object
		/// </summary>
		public List<EventData> Events {
			get { return _events; }
		}

		/// <summary>
		/// Used to enable disable the generation of events
		/// </summary>
		public bool EventsEnabled {
			get { return _eventsEnabled; }
			set {
				_eventsEnabled = value;
				if (!_eventsEnabled) {
					_events?.Clear();
				}
			}
		}

		private void AddPath(ICollection<int> eventPath, int field) {
			List<int> path = new List<int>();
			EventContext p = this;
			while (p != null && p.Field != UnsetPath) {
				path.Add(p.Field);
				p = p.Parent;
			}

			// add it to the repeated field in reverse
			for (int i = path.Count - 1; i >= 0; i--) {
				eventPath.Add(path[i]);
			}

			// add the field path
			eventPath.Add(field);
		}

		public virtual void AddEvent(EventData e) {
			if (_parent != null) {
				_parent.AddEvent(e);
			}
			else {
				if (_events == null) {
					_events = new List<EventData>();
				}

				_events.Add(e);
			}
		}

		/// <summary>
		/// Adds a set event relative to the context path
		/// </summary>
		public void AddSetEvent(int field, EventContent content) {
			if (!EventsEnabled) return;
			var e = new EventData {
				Set = content
			};
			AddPath(e.Path, field);
			AddEvent(e);
		}

		/// <summary>
		/// Adds a map event relative to the context path
		/// </summary>
		public void AddMapEvent(int field, MapEvent mapEvent) {
			if (!EventsEnabled) return;
			var e = new EventData {
				MapEvent = mapEvent
			};
			AddPath(e.Path, field);
			AddEvent(e);
		}

		/// <summary>
		/// Adds a list event relative to the context path
		/// </summary>
		public void AddListEvent(int field, ListEvent listEvent) {
			if (!EventsEnabled) return;
			var e = new EventData {
				ListEvent = listEvent
			};
			AddPath(e.Path, field);
			AddEvent(e);
		}

		/// <summary>
		/// Used for ListEventContext objects, which may have had their index updated by a replace or insert event
		/// </summary>
		/// <param name="index"></param>
		public void TryUpdateContextIndex(int index) {
			var listContext = _parent as ListEventContext;
			if (listContext != null) {
				listContext.Index = index;
			}
		}
	}
}