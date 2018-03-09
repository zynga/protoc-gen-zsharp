using System;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// Exception generated while applying an event
	/// </summary>
	public class ApplyEventException : Exception {
		/// <summary>
		/// Event being applied
		/// </summary>
		public EventData EventData { get; set; }

		public ApplyEventException(EventData e, Exception ex) : base("Failed to apply event: " + e, ex) {
			EventData = e;
		}
	}
}