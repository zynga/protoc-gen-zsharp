using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	/// <summary>
	/// Wraps a repeated field to add event sourcing support
	/// </summary>
	public class EventRepeatedField<T> : IEventSubscribable, IList<T>, IList, IDeepCloneable<RepeatedField<T>>, IEquatable<RepeatedField<T>>, IReadOnlyList<T> {
		private readonly RepeatedField<T> _internal;
		private readonly bool _isMessageType;
		private EventContext _context;
		private int _fieldNumber;
		private readonly EventDataConverter<T> _converter;
		private readonly List<EventListChange<T>> _itemsChanged = new List<EventListChange<T>>();

		public EventRepeatedField(EventDataConverter<T> converter, bool isMessageType = false) {
			_converter = converter;
			_internal = new RepeatedField<T>();
			_isMessageType = isMessageType;
		}

		public EventRepeatedField(EventDataConverter<T> converter, RepeatedField<T> repeatedField, bool isMessageType = false) {
			_converter = converter;
			_isMessageType = isMessageType;

			if (isMessageType) {
				_internal = new RepeatedField<T>();
				foreach (var v in repeatedField) {
					InternalAdd(v);
				}
			}
			else {
				_internal = repeatedField;
			}
		}

		/// <summary>
		/// Subcribe to changes of this message
		/// </summary>
		public event Action<EventRepeatedField<T>, IReadOnlyList<EventListChange<T>>> OnChanged;

		/// <inheritdoc />
		public void NotifySubscribers() {
			try {
				OnChanged?.Invoke(this, _itemsChanged);
			}
			finally {
				_itemsChanged.Clear();
			}
		}

		private void MarkDirtyAdd(int index, T item) {
			if (OnChanged != null && OnChanged.GetInvocationList().Length > 0) {
				_context.MarkDirty(this);
				_itemsChanged.Add(new EventListChange<T>(index, item, EventChangeType.Add));
			}
		}

		private void MarkDirtyRemove(int index, T item) {
			if (OnChanged != null && OnChanged.GetInvocationList().Length > 0) {
				_context.MarkDirty(this);
				_itemsChanged.Add(new EventListChange<T>(index, item, EventChangeType.Remove));
			}
		}

		private void MarkDirty() {
			if (OnChanged != null && OnChanged.GetInvocationList().Length > 0) {
				_context.MarkDirty(this);
			}
		}

		public RepeatedField<T> Clone() {
			return _internal.Clone();
		}

		public void AddEntriesFrom(CodedInputStream input, FieldCodec<T> codec) {
			var startIndex = Count;
			_internal.AddEntriesFrom(input, codec);
			if (_isMessageType) {
				for (int i = startIndex; i < _internal.Count; i++) {
					SetParent(i, _internal[i]);
				}
			}
		}

		public int CalculateSize(FieldCodec<T> codec) {
			return _internal.CalculateSize(codec);
		}

		public void WriteTo(CodedOutputStream output, FieldCodec<T> codec) {
			_internal.WriteTo(output, codec);
		}

		public void Add(T item) {
			InternalAdd(item);
#if !DISABLE_EVENTS
			AddListEvent(item);
#endif
		}

		public void Add(EventRepeatedField<T> other) {
			foreach (var v in other) {
				InternalAdd(v);
#if !DISABLE_EVENTS
				AddListEvent(v);
#endif
			}
		}

		public void Add(IList<T> other) {
			foreach (var v in other) {
				InternalAdd(v);
#if !DISABLE_EVENTS
				AddListEvent(v);
#endif
			}
		}

		public void Clear() {
			InternalClear();
#if !DISABLE_EVENTS
			ClearListEvent();
#endif
		}

		public bool Contains(T item) {
			return _internal.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			_internal.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {
			var result = InternalRemove(item);
#if !DISABLE_EVENTS
			if (result) {
				RemoveListEvent(item);
			}
#endif

			return result;
		}

		public int Count {
			get { return _internal.Count; }
		}

		public bool IsReadOnly {
			get { return _internal.IsReadOnly; }
		}

		public IEnumerator<T> GetEnumerator() {
			return _internal.GetEnumerator();
		}

		public override bool Equals(object obj) {
			var repeatedField = obj as RepeatedField<T>;
			if (repeatedField != null) {
				return _internal.Equals(repeatedField);
			}

			var eventField = obj as EventRepeatedField<T>;
			if (eventField != null) {
				return eventField.Equals(_internal);
			}

			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return (IEnumerator) _internal.GetEnumerator();
		}

		public override int GetHashCode() {
			return _internal.GetHashCode();
		}

		public bool Equals(RepeatedField<T> other) {
			return _internal.Equals(other);
		}

		public int IndexOf(T item) {
			return _internal.IndexOf(item);
		}

		public void Insert(int index, T item) {
			InternalInsert(index, item);
#if !DISABLE_EVENTS
			InsertListEvent(index, item);
#endif
		}

		public void RemoveAt(int index) {
			InternalRemoveAt(index);
#if !DISABLE_EVENTS
			RemoveAtListEvent(index);
#endif
		}

		public override string ToString() {
			return _internal.ToString();
		}

		public T this[int index] {
			get { return _internal[index]; }
			set {
#if !DISABLE_EVENTS
				var generateEvent = !value.Equals(_internal[index]);
#endif
				InternalReplaceAt(index, value);
#if !DISABLE_EVENTS
				if (generateEvent) {
					ReplaceListEvent(index, value);
				}
#endif
			}
		}

		bool IList.IsFixedSize {
			get { return false; }
		}

		void ICollection.CopyTo(Array array, int index) {
			_internal.CopyTo((T[]) array, index);
		}

		bool ICollection.IsSynchronized {
			get { return false; }
		}

		object ICollection.SyncRoot {
			get { return (object) this; }
		}

		object IList.this[int index] {
			get { return (object) this[index]; }
			set { this[index] = (T) value; }
		}

		int IList.Add(object value) {
			Add((T) value);
			return Count - 1;
		}

		bool IList.Contains(object value) {
			if (value is T)
				return Contains((T) value);
			return false;
		}

		int IList.IndexOf(object value) {
			if (!(value is T))
				return -1;
			return IndexOf((T) value);
		}

		void IList.Insert(int index, object value) {
			Insert(index, (T) value);
		}

		void IList.Remove(object value) {
			if (!(value is T))
				return;
			Remove((T) value);
		}

		private void AddListEvent(T item) {
			if (!_context.EventsEnabled) return;
			var listEvent = new ListEvent {
				ListAction = ListAction.AddList,
				Content = _converter.GetEventData(item)
			};
			_context.AddListEvent(_fieldNumber, listEvent);
		}

		private void RemoveListEvent(T item) {
			if (!_context.EventsEnabled) return;
			var listEvent = new ListEvent {
				ListAction = ListAction.RemoveList,
				Content = _converter.GetEventData(item)
			};
			_context.AddListEvent(_fieldNumber, listEvent);
		}

		private void RemoveAtListEvent(int index) {
			if (!_context.EventsEnabled) return;
			var listEvent = new ListEvent {
				ListAction = ListAction.RemoveAtList,
				Index = index
			};
			_context.AddListEvent(_fieldNumber, listEvent);
		}

		private void ReplaceListEvent(int index, T item) {
			if (!_context.EventsEnabled) return;
			var listEvent = new ListEvent {
				ListAction = ListAction.ReplaceList,
				Index = index,
				Content = _converter.GetEventData(item)
			};
			_context.AddListEvent(_fieldNumber, listEvent);
		}

		private void InsertListEvent(int index, T item) {
			if (!_context.EventsEnabled) return;
			var listEvent = new ListEvent {
				ListAction = ListAction.InsertList,
				Index = index,
				Content = _converter.GetEventData(item)
			};
			_context.AddListEvent(_fieldNumber, listEvent);
		}

		private void ClearListEvent() {
			if (!_context.EventsEnabled) return;
			var listEvent = new ListEvent {
				ListAction = ListAction.ClearList
			};
			_context.AddListEvent(_fieldNumber, listEvent);
		}

		public void SetContext(EventContext context, int fieldNumber) {
			_context = context;
			_fieldNumber = fieldNumber;
		}

		public bool ApplyEvent(ListEvent e) {
			switch (e.ListAction) {
				case ListAction.AddList:
					InternalAdd(_converter.GetItem(e.Content));
					return true;
				case ListAction.RemoveList:
					InternalRemove(_converter.GetItem(e.Content));
					return true;
				case ListAction.RemoveAtList:
					InternalRemoveAt(e.Index);
					return true;
				case ListAction.ReplaceList:
					InternalReplaceAt(e.Index, _converter.GetItem(e.Content));
					return true;
				case ListAction.InsertList:
					InternalInsert(e.Index, _converter.GetItem(e.Content));
					return true;
				case ListAction.ClearList:
					InternalClear();
					MarkDirty();
					return true;
				case ListAction.UpdateList:
					var registry = _internal[e.Index] as IEventRegistry;
					registry?.ApplyEvent(e.EventData, 0);
					MarkDirty();
					return true;
				default:
					return false;
			}
		}

		private bool InternalRemove(T item) {
			int destinationIndex = _internal.IndexOf(item);
			if (destinationIndex != -1) {
				InternalRemoveAt(destinationIndex);
				return true;
			}

			return false;
		}

		private void InternalRemoveAt(int index) {
			var removedItem = _internal[index];
			_internal.RemoveAt(index);
			MarkDirtyRemove(index, removedItem);
			if (_isMessageType) {
				ClearParent(removedItem);
				UpdateParents(index);
			}
		}

		private void InternalAdd(T item) {
			_internal.Add(item);
			MarkDirtyAdd(_internal.Count - 1, item);
			if (_isMessageType) SetParent(_internal.Count - 1, item);
		}

		private void InternalReplaceAt(int index, T item) {
			var removedItem = _internal[index];
			_internal[index] = item;
			MarkDirtyRemove(index, removedItem);
			MarkDirtyAdd(index, item);
			if (_isMessageType) {
				ClearParent(removedItem);
				SetParent(index, item);
			}
		}

		private void InternalInsert(int index, T item) {
			_internal.Insert(index, item);
			MarkDirtyAdd(index, item);
			if (_isMessageType) {
				SetParent(index, item);
				UpdateParents(index + 1);
			}
		}

		private void InternalClear() {
			if (_isMessageType) {
				while (_internal.Count > 0) {
					var removedItem = _internal[0];
					_internal.RemoveAt(0);
					if (_isMessageType) {
						ClearParent(removedItem);
					}
				}
			}
			else {
				_internal.Clear();
			}
		}

		private static void ClearParent(T item) {
			var registry = item as IEventRegistry;
			registry?.ClearParent();
		}

		private void SetParent(int index, T item) {
			var registry = item as IEventRegistry;
			registry?.SetParent(new ListEventContext(_context, index, _fieldNumber), EventContext.UnsetPath);
		}

		private void UpdateParents(int startIndex) {
			for (int i = startIndex; i < _internal.Count; i++) {
				var registry = _internal[i] as IEventRegistry;
				registry?.TryUpdateContextIndex(i);
			}
		}
	}
}