using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// This is a specialized context object that is passeed down to the elements of a MapField.  It does the work
	/// of generating an UpdateMap event.
	/// </summary>
	public class MapEventContext : EventContext {
		private readonly EventContext _mapContext;
		private readonly MapKey _key;
		private readonly int _fieldNumber;

		/// <summary>
		/// Creates a MapContext Instance
		/// </summary>
		public MapEventContext(EventContext mapContext, MapKey key, int fieldNumber) {
			_mapContext = mapContext;
			_key = key;
			_fieldNumber = fieldNumber;
		}

		public override void AddEvent(EventData e) {
			var me = new MapEvent {
				MapAction = MapAction.UpdateMap,
				Key = _key,
				EventData = e
			};

			_mapContext.AddMapEvent(_fieldNumber, me);
		}
	}
}