namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// Type of change event
	/// </summary>
	public enum EventChangeType {
		/// <summary>
		/// Item added
		/// </summary>
		Add,

		/// <summary>
		/// Item removed
		/// </summary>
		Remove
	}

	/// <summary>
	/// Stores data on a map field change
	/// </summary>
	public struct EventMapChange<TKey, TValue> {
		/// <summary>
		/// The key that changed
		/// </summary>
		public readonly TKey Key;

		/// <summary>
		/// The value that changed
		/// </summary>
		public readonly TValue Value;

		/// <summary>
		/// The type of change
		/// </summary>
		public readonly EventChangeType ChangeType;

		/// <summary>
		/// Default constructor
		/// </summary>
		public EventMapChange(TKey key, TValue value, EventChangeType changeType) {
			Key = key;
			Value = value;
			ChangeType = changeType;
		}
	}

	/// <summary>
	/// Stores data on a repeated field change
	/// </summary>
	public struct EventListChange<T> {
		/// <summary>
		/// The index that changed
		/// </summary>
		public readonly int Index;

		/// <summary>
		/// The item that changed
		/// </summary>
		public readonly T Value;

		/// <summary>
		/// The type of change
		/// </summary>
		public readonly EventChangeType ChangeType;

		/// <summary>
		/// Default constructor
		/// </summary>
		public EventListChange(int index, T value, EventChangeType changeType) {
			Index = index;
			Value = value;
			ChangeType = changeType;
		}
	}
}