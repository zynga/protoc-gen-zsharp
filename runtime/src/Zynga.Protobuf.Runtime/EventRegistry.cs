﻿using System;
using Google.Protobuf;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// The EventRegistry is extended by protobuf messages with the event_sourced option set
	/// </summary>
	public abstract class EventRegistry<T> : IEventRegistry, IEventSubscribable where T : IMessage<T> {
		protected readonly EventContext Context = new EventContext();

		/// <summary>
		/// Subcribe to changes to this message caused by applying events
		/// </summary>
		public event Action<T> OnChanged;

		/// <summary>
		/// Takes a set of events and applies them to the Message
		/// </summary>
		/// <param name="root"></param>
		public void ApplyEvents(EventSourceRoot root) {
			foreach (var e in root.Events) {
				try {
					ApplyEvent(e, 0);
				}
				catch (Exception ex) {
					throw new ApplyEventException(e, ex);
				}
			}

			Context.NotifySubscribers();
		}

		/// <inheritdoc />
		public void NotifySubscribers() {
			OnChanged?.Invoke(Message);
		}

		/// <summary>
		/// Mark the message dirty
		/// </summary>
		protected void MarkDirty() {
			if (OnChanged != null && OnChanged.GetInvocationList().Length > 0) {
				Context.MarkDirty(this);
			}
		}

		/// <summary>
		/// Returns the current message associated with the EventRegistry
		/// </summary>
		protected abstract T Message { get; }

		/// <summary>
		/// Applies a specific event with the current path index specified.
		/// This is for internal use only.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="pathIndex"></param>
		/// <returns>true if the event was applied</returns>
		public abstract bool ApplyEvent(EventData e, int pathIndex);

		/// <summary>
		/// Generates a snapshot of the state
		/// </summary>
		public abstract EventSourceRoot GenerateSnapshot();

		/// <summary>
		/// Returns the existing set of events, this does not clear the events associated with the messages
		/// </summary>
		/// <returns></returns>
		public EventSourceRoot PeekEvents() {
			var er = new EventSourceRoot();
			er.Events.AddRange(Context.Events);
			return er;
		}

		/// <summary>
		/// Returns true if the object currently has Events
		/// </summary>
		public bool HasEvents {
			get { return Context.Events.Count > 0; }
		}

		/// <summary>
		/// Returns the existing set of events and clears them
		/// </summary>
		/// <returns></returns>
		public EventSourceRoot GenerateEvents() {
			var er = PeekEvents();
			ClearEvents();
			return er;
		}

		/// <summary>
		/// Clears the existing events that have been generated
		/// </summary>
		public void ClearEvents() {
			Context.ClearEvents();
		}

		/// <summary>
		/// Used to enable disable the generation of events
		/// </summary>
		public bool EventsEnabled {
			get { return Context.EventsEnabled; }
			set { Context.EventsEnabled = value; }
		}

		/// <summary>
		/// Used to establish a parent child relationship between a message and child message.
		/// This is for internal use only.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="path"></param>
		public virtual void SetParent(EventContext parent, EventPath path) {
			Context.SetParent(parent, path);
		}

		/// <summary>
		/// Clears the existing parent, this is typically called when a child message is replaced or a message
		/// is removed from a list or a map. This is for internal use only.
		/// </summary>
		public virtual void ClearParent() {
			Context.ClearParent();
		}

		/// <summary>
		/// Used for ListEventContext objects, which may have had their index updated by a replace or insert event
		/// </summary>
		/// <param name="index"></param>
		public void TryUpdateContextIndex(int index) {
			var listContext = Context as ListEventContext;
			if (listContext != null) {
				listContext.Index = index;
			}
		}
	}
}