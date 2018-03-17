using System.Collections.Generic;
using Google.Protobuf;

namespace Zynga.Protobuf.Runtime {
	public abstract class EventMapConverter<TKey, TValue> {
		/// <summary>
		/// Returns EventContent for the specified data
		/// </summary>
		public abstract ByteString GetKeyValue(TKey key, TValue value, bool skipValue = false);

		/// <summary>
		/// Returns the data for the specified EventContent
		/// </summary>
		public abstract KeyValuePair<TKey, TValue> GetItem(ByteString data, bool skipValue = false);
	}
}