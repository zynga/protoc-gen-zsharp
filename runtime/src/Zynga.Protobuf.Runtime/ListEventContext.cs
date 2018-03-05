using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// This is a specialized context object that is passeed down to the elements of a RepeatedField.  It does the work
	/// of generating an UpdateList event.
	/// </summary>
	public class ListEventContext : EventContext {
		private readonly EventContext _listContext;
		private int _index;
		private int _fieldNumber;

		/// <summary>
		/// Creates a ListEventContext Instance
		/// </summary>
		public ListEventContext(EventContext listContext, int index, int fieldNumber) {
			_listContext = listContext;
			_index = index;
			_fieldNumber = fieldNumber;
		}

		/// <summary>
		/// The current index of the RepeatedField element
		/// </summary>
		public int Index {
			get { return _index; }
			set { _index = value; }
		}

		private void AddUpdateEvent(EventData data) {
			var le = new ListEvent {
				ListAction = ListAction.UpdateList,
				Index = _index,
				EventData = data
			};

			_listContext.AddListEvent(_fieldNumber, le);
		}

		/// <inheritdoc />
		public override void AddSetEvent(EventPath path, EventContent content) {
			if (!_listContext.EventsEnabled) return;
			var e = new EventData {
				Set = content
			};
			e.Path.AddRange(path.Path);

			AddUpdateEvent(e);
		}

		/// <inheritdoc />
		public override void AddMapEvent(EventPath path, MapEvent mapEvent) {
			if (!_listContext.EventsEnabled) return;
			var e = new EventData {
				MapEvent = mapEvent
			};
			e.Path.AddRange(path.Path);

			AddUpdateEvent(e);
		}

		/// <inheritdoc />
		public override void AddListEvent(EventPath path, ListEvent listEvent) {
			if (!_listContext.EventsEnabled) return;
			var e = new EventData {
				ListEvent = listEvent
			};
			e.Path.AddRange(path.Path);

			AddUpdateEvent(e);
		}
	}
}