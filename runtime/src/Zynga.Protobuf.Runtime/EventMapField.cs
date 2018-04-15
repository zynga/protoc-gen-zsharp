using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	public class EventMapField<TKey, TValue> : IEventSubscribable, IDeepCloneable<MapField<TKey, TValue>>, IDictionary<TKey, TValue>, IEquatable<MapField<TKey, TValue>>, IDictionary, IReadOnlyDictionary<TKey, TValue> {
		private readonly MapField<TKey, TValue> _internal;
		private readonly bool _isMessageType;
		private EventContext _context;
		private int _fieldNumber;
		private readonly EventMapConverter<TKey, TValue> _converter;
		private readonly List<EventMapChange<TKey, TValue>> _itemsChanged = new List<EventMapChange<TKey, TValue>>();

		public EventMapField(EventMapConverter<TKey, TValue> converter, bool isMessageType = false) {
			_converter = converter;
			_internal = new MapField<TKey, TValue>();
			_isMessageType = isMessageType;
		}

		public EventMapField(EventMapConverter<TKey, TValue> converter, MapField<TKey, TValue> mapField, bool isMessageType = false) {
			_converter = converter;
			_isMessageType = isMessageType;

			if (_isMessageType) {
				_internal = new MapField<TKey, TValue>();
				foreach (var kv in mapField) {
					InternalAdd(kv.Key, kv.Value);
				}
			}
			else {
				_internal = mapField;
			}
		}

		/// <summary>
		/// Subcribe to changes of this message
		/// </summary>
		public event Action<EventMapField<TKey, TValue>, IReadOnlyList<EventMapChange<TKey, TValue>>> OnChanged;

		/// <inheritdoc />
		public void NotifySubscribers() {
			try {
				OnChanged?.Invoke(this, _itemsChanged);
			}
			finally {
				_itemsChanged.Clear();
			}
		}

		private void MarkDirtyAdd(TKey key, TValue value) {
			if (OnChanged != null && OnChanged.GetInvocationList().Length > 0) {
				_context.MarkDirty(this);
				_itemsChanged.Add(new EventMapChange<TKey, TValue>(key, value, EventChangeType.Add));
			}
		}

		private void MarkDirtyRemove(TKey key, TValue value) {
			if (OnChanged != null && OnChanged.GetInvocationList().Length > 0) {
				_context.MarkDirty(this);
				_itemsChanged.Add(new EventMapChange<TKey, TValue>(key, value, EventChangeType.Remove));
			}
		}

		private void MarkDirty() {
			if (OnChanged != null && OnChanged.GetInvocationList().Length > 0) {
				_context.MarkDirty(this);
			}
		}

		public MapField<TKey, TValue> Clone() {
			return _internal.Clone();
		}

		public void Add(TKey key, TValue value) {
			InternalAdd(key, value);
#if !DISABLE_EVENTS
			AddMapEvent(key, value);
#endif
		}

		public void Add(EventMapField<TKey, TValue> other) {
			foreach (var kv in other) {
				InternalAdd(kv.Key, kv.Value);
#if !DISABLE_EVENTS
				AddMapEvent(kv.Key, kv.Value);
#endif
			}
		}

		public bool ContainsKey(TKey key) {
			return _internal.ContainsKey(key);
		}

		public bool Remove(TKey key) {
			var result = InternalRemove(key);
#if !DISABLE_EVENTS
			if (result) {
				RemoveMapEvent(key);
			}
#endif

			return result;
		}

		public bool TryGetValue(TKey key, out TValue value) {
			return _internal.TryGetValue(key, out value);
		}

		public TValue this[TKey key] {
			get { return _internal[key]; }
			set {
#if !DISABLE_EVENTS
				var generateEvent = !_internal.ContainsKey(key) || !_internal[key].Equals(value);
#endif
				InternalReplace(key, value);
#if !DISABLE_EVENTS
				if (generateEvent) {
					ReplaceMapEvent(key, value);
				}
#endif
			}
		}

		public ICollection<TKey> Keys {
			get { return _internal.Keys; }
		}

		public ICollection<TValue> Values {
			get { return _internal.Values; }
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return _internal.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return (IEnumerator) this.GetEnumerator();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
			this.Add(item.Key, item.Value);
		}

		public void Clear() {
			InternalClear();
#if !DISABLE_EVENTS
			ClearMapEvent();
#endif
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
			TValue y;
			if (this.TryGetValue(item.Key, out y))
				return EqualityComparer<TValue>.Default.Equals(item.Value, y);
			return false;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			_internal.ToList().CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
			if ((object) item.Key == null)
				throw new ArgumentException("Key is null", nameof(item));
			TValue node;
			if (!TryGetValue(item.Key, out node) || !EqualityComparer<TValue>.Default.Equals(item.Value, node))
				return false;
			return Remove(item.Key);
		}

		public void CopyTo(Array array, int index) {
			((ICollection) this.Select<KeyValuePair<TKey, TValue>, DictionaryEntry>((Func<KeyValuePair<TKey, TValue>, DictionaryEntry>) (pair => new DictionaryEntry((object) pair.Key, (object) pair.Value))).ToList<DictionaryEntry>()).CopyTo(array, index);
		}

		/// <summary>Gets the number of elements contained in the map.</summary>
		public int Count {
			get { return _internal.Count; }
		}

		/// <summary>Gets a value indicating whether the map is read-only.</summary>
		public bool IsReadOnly {
			get { return false; }
		}

		public override bool Equals(object other) {
			var mapField = other as MapField<TKey, TValue>;
			if (mapField != null) {
				return _internal.Equals(mapField);
			}

			var eventMap = other as EventMapField<TKey, TValue>;
			if (eventMap != null) {
				return eventMap.Equals(_internal);
			}

			return false;
		}

		public override int GetHashCode() {
			return _internal.GetHashCode();
		}

		public bool Equals(MapField<TKey, TValue> other) {
			return _internal.Equals(other);
		}

		public void AddEntriesFrom(CodedInputStream input, MapField<TKey, TValue>.Codec codec) {
			_internal.AddEntriesFrom(input, codec);
			if (_isMessageType) {
				foreach (var kv in _internal) {
					SetParent(kv.Key, kv.Value);
				}
			}
		}

		public void WriteTo(CodedOutputStream output, MapField<TKey, TValue>.Codec codec) {
			_internal.WriteTo(output, codec);
		}

		public int CalculateSize(MapField<TKey, TValue>.Codec codec) {
			return _internal.CalculateSize(codec);
		}

		public override string ToString() {
			return _internal.ToString();
		}

		void IDictionary.Add(object key, object value) {
			this.Add((TKey) key, (TValue) value);
		}

		bool IDictionary.Contains(object key) {
			return _internal.ContainsKey((TKey) key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator() {
			return (IDictionaryEnumerator) new DictionaryEnumerator(this.GetEnumerator());
		}

		void IDictionary.Remove(object key) {
			ProtoPreconditions.CheckNotNull<object>(key, nameof(key));
			if (!(key is TKey))
				return;
			this.Remove((TKey) key);
		}

		bool IDictionary.IsFixedSize {
			get { return false; }
		}

		ICollection IDictionary.Keys {
			get { return (ICollection) this.Keys; }
		}

		ICollection IDictionary.Values {
			get { return (ICollection) this.Values; }
		}

		bool ICollection.IsSynchronized {
			get { return false; }
		}

		object ICollection.SyncRoot {
			get { return (object) this; }
		}

		object IDictionary.this[object key] {
			get {
				ProtoPreconditions.CheckNotNull<object>(key, nameof(key));
				if (!(key is TKey))
					return (object) null;
				TValue obj;
				this.TryGetValue((TKey) key, out obj);
				return (object) obj;
			}
			set { this[(TKey) key] = (TValue) value; }
		}

		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys {
			get { return (IEnumerable<TKey>) this.Keys; }
		}

		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values {
			get { return (IEnumerable<TValue>) this.Values; }
		}

		private void AddMapEvent(TKey key, TValue value) {
			if (!_context.EventsEnabled) return;
			var mapEvent = new MapEvent {
				MapAction = MapAction.AddMap,
				KeyValue = _converter.GetKeyValue(key, value)
			};
			_context.AddMapEvent(_fieldNumber, mapEvent);
		}

		private void RemoveMapEvent(TKey key) {
			if (!_context.EventsEnabled) return;
			var mapEvent = new MapEvent {
				MapAction = MapAction.RemoveMap,
				Key = _converter.GetMapKey(key)
			};
			_context.AddMapEvent(_fieldNumber, mapEvent);
		}

		private void ReplaceMapEvent(TKey key, TValue value) {
			if (!_context.EventsEnabled) return;
			var mapEvent = new MapEvent {
				MapAction = MapAction.ReplaceMap,
				KeyValue = _converter.GetKeyValue(key, value)
			};
			_context.AddMapEvent(_fieldNumber, mapEvent);
		}

		private void ClearMapEvent() {
			if (!_context.EventsEnabled) return;
			var mapEvent = new MapEvent {
				MapAction = MapAction.ClearMap
			};
			_context.AddMapEvent(_fieldNumber, mapEvent);
		}

		public void SetContext(EventContext context, int fieldNumber) {
			_context = context;
			_fieldNumber = fieldNumber;
		}

		public bool ApplyEvent(MapEvent e) {
			switch (e.MapAction) {
				case MapAction.AddMap:
					var addPair = _converter.GetItem(e.KeyValue);
					InternalAdd(addPair.Key, addPair.Value);
					return true;
				case MapAction.RemoveMap:
					var removeKey = _converter.GetKey(e.Key);
					InternalRemove(removeKey);
					return true;
				case MapAction.ReplaceMap:
					var replacePair = _converter.GetItem(e.KeyValue);
					InternalReplace(replacePair.Key, replacePair.Value);
					return true;
				case MapAction.ClearMap:
					InternalClear();
					MarkDirty();
					return true;
				case MapAction.UpdateMap:
					var updateKey = _converter.GetKey(e.Key);
					var registry = _internal[updateKey] as IEventRegistry;
					registry?.ApplyEvent(e.EventData, 0);
					MarkDirty();
					return true;
				default:
					return false;
			}
		}

		private void InternalAdd(TKey key, TValue value) {
			_internal.Add(key, value);
			MarkDirtyAdd(key, value);
			if (_isMessageType) SetParent(key, value);
		}

		private bool InternalRemove(TKey key) {
			TValue value;
			if (_internal.TryGetValue(key, out value)) {
				if (_isMessageType) {
					ClearParent(value);
				}

				_internal.Remove(key);
				MarkDirtyRemove(key, value);
				return true;
			}

			return false;
		}

		private void InternalReplace(TKey key, TValue value) {
			TValue existingValue;
			if (_internal.TryGetValue(key, out existingValue)) {
				MarkDirtyRemove(key, existingValue);
				if (_isMessageType) {
					ClearParent(existingValue);
				}
			}

			_internal[key] = value;
			MarkDirtyAdd(key, value);
			if (_isMessageType) {
				SetParent(key, value);
			}
		}

		private void InternalClear() {
			if (_isMessageType) {
				foreach (var kv in _internal) {
					ClearParent(kv.Value);
				}
			}

			_internal.Clear();
		}

		private static void ClearParent(TValue item) {
			var registry = item as IEventRegistry;
			registry?.ClearParent();
		}

		private void SetParent(TKey key, TValue value) {
			var registry = value as IEventRegistry;
			var mapKey = _converter.GetMapKey(key);
			registry?.SetParent(new MapEventContext(_context, mapKey, _fieldNumber), EventPath.Empty);
		}

		private class DictionaryEnumerator : IDictionaryEnumerator {
			private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

			internal DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator) {
				this._enumerator = enumerator;
			}

			public bool MoveNext() {
				return this._enumerator.MoveNext();
			}

			public void Reset() {
				this._enumerator.Reset();
			}

			public object Current {
				get { return (object) this.Entry; }
			}

			public DictionaryEntry Entry {
				get { return new DictionaryEntry(this.Key, this.Value); }
			}

			public object Key {
				get { return (object) this._enumerator.Current.Key; }
			}

			public object Value {
				get { return (object) this._enumerator.Current.Value; }
			}
		}
	}
}