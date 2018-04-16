using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// This is a specialized context object that is passeed down to the elements of a RepeatedField.  It does the work
	/// of generating an UpdateList event.
	/// </summary>
	public class ListEventContext : EventContext {
		private readonly EventContext _listContext;
		private int _index;
		private readonly int _fieldNumber;

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

		public override void AddEvent(EventData e) {
			var le = new ListEvent {
				ListAction = ListAction.UpdateList,
				Index = _index,
				EventData = e
			};

			_listContext.AddListEvent(_fieldNumber, le);
		}

		/// <inheritdoc />
		public override void MarkDirty(IEventSubscribable eventRegistry) {
			_listContext.MarkDirty(eventRegistry);
		}
	}
}