using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// EventRegistry is extended by protobuf messages with the event_sourced option set. This interface is useful
	/// for methods that do not require the message Type.
	/// </summary>
	public interface IEventRegistry {
		/// <summary>
		/// Takes a set of events and applies them to the Message
		/// </summary>
		/// <param name="root"></param>
		void ApplyEvents(EventSourceRoot root);

		/// <summary>
		/// Applies a specific event with the current path index specified.
		/// This is for internal use only.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="pathIndex"></param>
		/// <returns>true if the event was applied</returns>
		bool ApplyEvent(EventData e, int pathIndex);

		/// <summary>
		/// Generates a snapshot of the state
		/// </summary>
		EventSourceRoot GenerateSnapshot();

		/// <summary>
		/// Returns the existing set of events, this does not clear the events associated with the messages
		/// </summary>
		/// <returns></returns>
		EventSourceRoot PeekEvents();

		/// <summary>
		/// Returns true if the object currently has Events
		/// </summary>
		bool HasEvents { get; }

		/// <summary>
		/// Returns the existing set of events and clears them
		/// </summary>
		/// <returns></returns>
		EventSourceRoot GenerateEvents();

		/// <summary>
		/// Clears the existing events that have been generated
		/// </summary>
		void ClearEvents();

		/// <summary>
		/// Used to establish a parent child relationship between a message and child message.
		/// This is for internal use only.
		/// </summary>
		void SetParent(EventContext parent, int field);

		/// <summary>
		/// Clears the existing parent, this is typically called when a child message is replaced or a message
		/// is removed from a list or a map. This is for internal use only.
		/// </summary>
		void ClearParent();

		/// <summary>
		/// Used for ListEventContext objects, which may have had their index updated by a replace or insert event
		/// </summary>
		/// <param name="index"></param>
		void TryUpdateContextIndex(int index);
	}
}