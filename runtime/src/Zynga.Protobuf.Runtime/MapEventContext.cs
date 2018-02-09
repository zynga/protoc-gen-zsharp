using Google.Protobuf;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// This is a specialized context object that is passeed down to the elements of a MapField.  It does the work
	/// of generating an UpdateMap event.
	/// </summary>
	public class MapEventContext : EventContext {
		private readonly EventContext _mapContext;
		private readonly ByteString _key;
		private readonly int _fieldNumber;

		/// <summary>
		/// Creates a MapContext Instance
		/// </summary>
		public MapEventContext(EventContext mapContext, ByteString key, int fieldNumber) {
			_mapContext = mapContext;
			_key = key;
			_fieldNumber = fieldNumber;
		}

		private void AddUpdateEvent(EventData data) {
			var me = new MapEvent {
				MapAction = MapAction.UpdateMap,
				KeyValue = _key,
				EventData = data
			};

			_mapContext.AddMapEvent(_fieldNumber, me);
		}

		/// <inheritdoc />
		public override void AddSetEvent(EventPath path, EventContent content) {
			if (!_mapContext.EventsEnabled) return;
			var e = new EventData {
				Set = content
			};
			e.Path.AddRange(path.Path);

			AddUpdateEvent(e);
		}

		/// <inheritdoc />
		public override void AddMapEvent(EventPath path, MapEvent mapEvent) {
			if (!_mapContext.EventsEnabled) return;
			var e = new EventData {
				MapEvent = mapEvent
			};
			e.Path.AddRange(path.Path);

			AddUpdateEvent(e);
		}

		/// <inheritdoc />
		public override void AddListEvent(EventPath path, ListEvent listEvent) {
			if (!_mapContext.EventsEnabled) return;
			var e = new EventData {
				ListEvent = listEvent
			};
			e.Path.AddRange(path.Path);

			AddUpdateEvent(e);
		}
	}
}