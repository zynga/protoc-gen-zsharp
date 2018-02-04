using System.Collections.Generic;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	public abstract class EventMapConverter<TKey, TValue> {
		/// <summary>
		/// Returns EventContent for the specified data
		/// </summary>
		public abstract EventContent GetEventData(TKey key, TValue value, bool skipValue = false);
		
		/// <summary>
		/// Returns the data for the specified EventContent
		/// </summary>
		public abstract KeyValuePair<TKey, TValue> GetItem(EventData data);
	}
}