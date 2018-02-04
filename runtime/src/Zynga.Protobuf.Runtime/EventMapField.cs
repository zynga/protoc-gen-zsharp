using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime {
	public class EventMapField<TKey, TValue> : IDeepCloneable<MapField<TKey, TValue>>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IEquatable<MapField<TKey, TValue>>, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>> {
		private readonly MapField<TKey, TValue> _internal = new MapField<TKey, TValue>();
		private readonly List<EventData> _root;
		private readonly EventPath _path;
		private readonly EventMapConverter<TKey, TValue> _converter;

		public EventMapField(List<EventData> root, EventPath path, EventMapConverter<TKey, TValue> converter) {
			_root = root;
			_path = path;
			_converter = converter;
		}

		public MapField<TKey, TValue> Clone() {
			return _internal.Clone();
		}

		public void Add(TKey key, TValue value) {
			_internal.Add(key, value);
			AddMapEvent(key, value);
		}

		public bool ContainsKey(TKey key) {
			return _internal.ContainsKey(key);
		}

		public bool Remove(TKey key) {
			var result = _internal.Remove(key);
			if (result) {
				RemoveMapEvent(key);
			}

			return result;
		}

		public bool TryGetValue(TKey key, out TValue value) {
			return _internal.TryGetValue(key, out value);
		}

		public TValue this[TKey key] {
			get { return _internal[key]; }
			set {
				_internal[key] = value;
				ReplaceMapEvent(key, value);
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
			_internal.Clear();
			ClearMapEvent();
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
			var newEvent = new EventData {
				Action = EventAction.AddMap,
				Data = _converter.GetEventData(key, value)
			};
			newEvent.Path.AddRange(_path._path);
			_root.Add(newEvent);
		}

		private void RemoveMapEvent(TKey key) {
			var newEvent = new EventData {
				Action = EventAction.RemoveMap,
				Data = _converter.GetEventData(key, default(TValue))
			};
			newEvent.Path.AddRange(_path._path);
			_root.Add(newEvent);
		}

		private void ReplaceMapEvent(TKey key, TValue value) {
			var newEvent = new EventData {
				Action = EventAction.ReplaceMap,
				Data = _converter.GetEventData(key, value)
			};
			newEvent.Path.AddRange(_path._path);
			_root.Add(newEvent);
		}

		private void ClearMapEvent() {
			var newEvent = new EventData {
				Action = EventAction.ClearMap
			};
			newEvent.Path.AddRange(_path._path);
			_root.Add(newEvent);
		}

		public bool ApplyEvent(EventData e) {
			switch (e.Action) {
				case EventAction.AddMap:
					var addPair = _converter.GetItem(e.Data);
					_internal.Add(addPair.Key, addPair.Value);
					return true;
				case EventAction.RemoveMap:
					var removePair = _converter.GetItem(e.Data);
					_internal.Remove(removePair.Key);
					return true;
				case EventAction.ReplaceMap:
					var replacePair = _converter.GetItem(e.Data);
					_internal[replacePair.Key] = replacePair.Value;
					return true;
				case EventAction.ClearMap:
					_internal.Clear();
					return true;
				default:
					return false;
			}
		}
	}
}