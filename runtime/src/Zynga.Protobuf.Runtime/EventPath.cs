using System;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// Used for generating event paths
	/// </summary>
	public struct EventPath {
		/// <summary>
		/// Generates an empty EventPath
		/// </summary>
		public static EventPath Empty = new EventPath();

		/// <summary>
		/// The full path
		/// </summary>
		public int[] Path;

		/// <summary>
		/// Create an EventPath for the child of another message
		/// </summary>
		public EventPath(EventPath parent, int field) {
			if (parent.Path == null) {
				Path = new[] {field};
			}
			else {
				Path = new int[parent.Path.Length + 1];
				Array.Copy(parent.Path, Path, parent.Path.Length);
				Path[Path.Length - 1] = field;
			}
		}
	}
}