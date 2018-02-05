using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	public abstract class EventDataConverter<T> {
		/// <summary>
		/// Returns EventContent for the specified data
		/// </summary>
		public abstract EventContent GetEventData(T data);
		
		/// <summary>
		/// Returns the data for the specified EventContent
		/// </summary>
		public abstract T GetItem(EventContent data);
	}
}