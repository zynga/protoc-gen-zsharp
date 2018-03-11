using System;
using Google.Protobuf;
using Xunit;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public static class EventTestHelper {
		public static EventSourceRoot AssertGenerated(EventRegistry blob) {
			EventSourceRoot root = blob.PeekEvents();
			Assert.Single(root.Events);
			return root;
		}

		public static void AssertNotGenerated(EventRegistry blob) {
			EventSourceRoot root = blob.PeekEvents();
			Assert.Empty(root.Events);
		}

		public static void AssertPath(EventData eventData, int[] path) {
			Assert.Equal(path.Length, eventData.Path.Count);
			for (int i = 0; i < path.Length; i++) {
				Assert.Equal(path[i], eventData.Path[i]);
			}
		}

		public static void AssertDeltaPath(EventRegistry blob, int[] path) {
			EventSourceRoot root = AssertGenerated(blob);
			AssertPath(root.Events[0], path);
			blob.ClearEvents();
		}

		public static void AssertEventsStable<T>(T message) where T : EventRegistry, IMessage, new() {
			var newMessage = new T();
			var events = message.GenerateEvents();
			newMessage.ApplyEvents(events);
			Assert.Equal(message, newMessage);
		}

		public static void AssertEventsStableWithClone<T>(T message, Action makeChanges, bool expectChanges = true) where T : EventRegistry, IMessage, IDeepCloneable<T>, new() {
			if (message.HasEvents) {
				message.ClearEvents();
			}
			var newMessage = message.Clone();

			makeChanges();

			if (expectChanges) {
				Assert.True(message.HasEvents);
				Assert.True(message.PeekEvents().Events.Count > 0);
			}
			else {
				Assert.False(message.HasEvents);
				Assert.False(message.PeekEvents().Events.Count > 0);
			}

			var events = message.GenerateEvents();
			newMessage.ApplyEvents(events);
			Assert.Equal(message, newMessage);
		}

		public static void AssertEventsStableWithSnapshot<T>(T message, Action makeChanges, bool expectChanges = true) where T : EventRegistry, IMessage, IDeepCloneable<T>, new() {
			if (message.HasEvents) {
				message.ClearEvents();
			}

			var snapshot = message.GenerateSnapshot();
			var newMessage = new T();
			newMessage.ApplyEvents(snapshot);

			makeChanges();

			if (expectChanges) {
				Assert.True(message.HasEvents);
				Assert.True(message.PeekEvents().Events.Count > 0);
			}
			else {
				Assert.False(message.HasEvents);
				Assert.False(message.PeekEvents().Events.Count > 0);
			}
			var events = message.GenerateEvents();
			newMessage.ApplyEvents(events);
			Assert.Equal(message, newMessage);
		}
	}
}