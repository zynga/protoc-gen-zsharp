using Xunit;
using Zynga.Protobuf.Runtime.EventSource;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public static class EventTestHelper {
		public static EventSourceRoot AssertGenerated(EventRegistry blob) {
			EventSourceRoot root = blob.GenerateEvents();
			Assert.Single(root.Events);
			return root;
		}

		public static void AssertNotGenerated(EventRegistry blob) {
			EventSourceRoot root = blob.GenerateEvents();
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
			blob.Reset();
		}
	}
}