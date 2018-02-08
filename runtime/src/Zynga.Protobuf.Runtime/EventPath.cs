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

		private readonly List<int> _path = new List<int>();

		/// <summary>
		/// The full path
		/// </summary>
		public List<int> Path {
			get => _path;
		}

		/// <summary>
		/// Creates an empty EventPath
		/// </summary>
		public EventPath() { }

		/// <summary>
		/// Create an EventPath for the child of another message
		/// </summary>
		public EventPath(EventPath parent, int field) {
			_path.Clear();
			_path.AddRange(parent._path);
			_path.Add(field);
		}
	}
}