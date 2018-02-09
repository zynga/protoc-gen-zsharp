using System;
using System.Collections.Generic;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// The event context does the work of managing events and the relative paths of parents and children
	/// </summary>
	public class EventContext {
		private readonly List<EventData> _events = new List<EventData>();
		private EventContext _parent;
		private EventPath _path = EventPath.Empty;
		private bool _eventsEnabled = true;

		/// <summary>
		/// Establishes a parent child relationship
		/// </summary>
		public void SetParent(EventContext parent, EventPath path) {
			if (_parent != null) {
				throw new Exception("Message is already associated with another parent");
			}
			
			_parent = parent;
			_path = path;
			// when a message is added to a parent, any delta events it currently has are no longer valid
			_events.Clear();
		}

		/// <summary>
		/// Removes the parent child relationship
		/// </summary>
		public void ClearParent() {
			_parent = null;
			_path = EventPath.Empty;
		}

		/// <summary>
		/// Clears any events that have been generated
		/// </summary>
		public void ClearEvents() {
			_events.Clear();
		}

		/// <summary>
		/// The current path of the context object
		/// </summary>
		public EventPath Path {
			get => _path;
		}

		/// <summary>
		/// The current events of the context object
		/// </summary>
		public List<EventData> Events {
			get => _events;
		}

		/// <summary>
		/// Used to enable disable the generation of events
		/// </summary>
		public bool EventsEnabled {
			get => _eventsEnabled;
			set {
				_eventsEnabled = value;
				if (!_eventsEnabled) {
					_events.Clear();
				}
			}
		}

		/// <summary>
		/// Adds a set event relative to the context path
		/// </summary>
		public virtual void AddSetEvent(int field, EventContent content) {
			if (!EventsEnabled) return;
			AddSetEvent(new EventPath(_path, field), content);
		}

		/// <summary>
		/// Adds a set event for the specified path
		/// </summary>
		public virtual void AddSetEvent(EventPath path, EventContent content) {
			if (!EventsEnabled) return;
			if (_parent != null) {
				_parent.AddSetEvent(path, content);
			}
			else {
				var e = new EventData {
					Set = content
				};
				e.Path.AddRange(path.Path);
				_events.Add(e);
			}
		}

		/// <summary>
		/// Adds a map event relative to the context path
		/// </summary>
		public virtual void AddMapEvent(int field, MapEvent mapEvent) {
			if (!EventsEnabled) return;
			AddMapEvent(new EventPath(_path, field), mapEvent);
		}

		/// <summary>
		/// Adds a map event for the specified path
		/// </summary>
		public virtual void AddMapEvent(EventPath path, MapEvent mapEvent) {
			if (!EventsEnabled) return;
			if (_parent != null) {
				_parent.AddMapEvent(path, mapEvent);
			}
			else {
				var e = new EventData {
					MapEvent = mapEvent
				};
				e.Path.AddRange(path.Path);
				_events.Add(e);
			}
		}

		/// <summary>
		/// Adds a list event relative to the context path
		/// </summary>
		public virtual void AddListEvent(int field, ListEvent listEvent) {
			if (!EventsEnabled) return;
			AddListEvent(new EventPath(_path, field), listEvent);
		}

		/// <summary>
		/// Adds a list event for the specified path
		/// </summary>
		public virtual void AddListEvent(EventPath path, ListEvent listEvent) {
			if (!EventsEnabled) return;
			if (_parent != null) {
				_parent.AddListEvent(path, listEvent);
			}
			else {
				var e = new EventData {
					ListEvent = listEvent
				};
				e.Path.AddRange(path.Path);
				_events.Add(e);
			}
		}
	}
}