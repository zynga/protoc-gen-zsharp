using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	public class EventMapField<TKey, TValue> : IDeepCloneable<MapField<TKey, TValue>>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IEquatable<MapField<TKey, TValue>>, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>> {
		private readonly MapField<TKey, TValue> _internal;
		private readonly bool _isMessageType;
		private EventContext _context;
		private int _fieldNumber;
		private readonly EventMapConverter<TKey, TValue> _converter;

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
				if (kv.Value is IDeepCloneable<TValue>) {
					InternalAdd(kv.Key, ((IDeepCloneable<TValue>) kv.Value).Clone());
				}
				else {
					InternalAdd(kv.Key, kv.Value);
				}
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
			throw new NotSupportedException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
			throw new NotSupportedException();
		}

		public void CopyTo(Array array, int index) {
			throw new NotSupportedException();
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
				foreach(var kv in _internal) {
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
			throw new NotSupportedException();
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
				KeyValue = _converter.GetKeyValue(key, default(TValue), true)
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
					var removePair = _converter.GetItem(e.KeyValue, true);
					InternalRemove(removePair.Key);
					return true;
				case MapAction.ReplaceMap:
					var replacePair = _converter.GetItem(e.KeyValue);
					InternalReplace(replacePair.Key, replacePair.Value);
					return true;
				case MapAction.ClearMap:
					InternalClear();
					return true;
				case MapAction.UpdateMap:
					var updatePair = _converter.GetItem(e.KeyValue, true);
					var registry = _internal[updatePair.Key] as EventRegistry;
					registry?.ApplyEvent(e.EventData, 0);
					return true;
				default:
					return false;
			}
		}

		private void InternalAdd(TKey key, TValue value) {
			_internal.Add(key, value);
			if (_isMessageType) SetParent(key, value);
		}

		private bool InternalRemove(TKey key) {
			if (!_isMessageType) {
				return _internal.Remove(key);
			}

			TValue value;
			if(_internal.TryGetValue(key, out value)) {
				ClearParent(value);
				_internal.Remove(key);
				return true;
			}

			return false;
		}

		private void InternalReplace(TKey key, TValue value) {
			if (_isMessageType) {
				TValue existingValue;
				if(_internal.TryGetValue(key, out existingValue)) {
					ClearParent(existingValue);
				}
				_internal[key] = value;
				SetParent(key, value);
			}
			else {
				_internal[key] = value;
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
			var registry = item as EventRegistry;
			registry?.ClearParent();
		}

		private void SetParent(TKey key, TValue value) {
			var registry = value as EventRegistry;
			var keyBytes = _converter.GetKeyValue(key, value, true);
			registry?.SetParent(new MapEventContext(_context, keyBytes, _fieldNumber), EventPath.Empty);
		}
	}
}