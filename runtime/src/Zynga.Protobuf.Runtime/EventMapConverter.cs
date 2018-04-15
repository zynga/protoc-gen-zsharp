using System.Collections.Generic;
using Google.Protobuf;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	public abstract class EventMapConverter<TKey, TValue> {
		public abstract MapKey GetMapKey(TKey key);

		public abstract TKey GetKey(MapKey key);

		/// <summary>
		/// Returns EventContent for the specified data
		/// </summary>
		public abstract ByteString GetKeyValue(TKey key, TValue value);

		/// <summary>
		/// Returns the data for the specified EventContent
		/// </summary>
		public abstract KeyValuePair<TKey, TValue> GetItem(ByteString data);
	}
}