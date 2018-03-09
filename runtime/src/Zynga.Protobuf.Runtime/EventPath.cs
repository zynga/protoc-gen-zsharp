using System.Collections.Generic;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// Used for generating event paths
	/// </summary>
	public class EventPath {
		/// <summary>
		/// Generates an empty EventPath
		/// </summary>
		public static EventPath Empty => new EventPath();

		/// <summary>
		/// The full path
		/// </summary>
		public List<int> Path { get; }

		/// <summary>
		/// Creates an empty EventPath
		/// </summary>
		public EventPath() {
			Path = new List<int>();
		}

		/// <summary>
		/// Create an EventPath for the child of another message
		/// </summary>
		public EventPath(EventPath parent, int field) {
			Path = new List<int>(parent.Path.Count + 1);
			Path.AddRange(parent.Path);
			Path.Add(field);
		}
	}
}