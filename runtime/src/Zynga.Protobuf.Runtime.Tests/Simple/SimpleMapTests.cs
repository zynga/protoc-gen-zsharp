using Com.Zynga.Runtime.Protobuf;
using Xunit;
using Zynga.Protobuf.Runtime.EventSource;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class SimpleMapTests {
		[Fact]
		public void AreTheSame() {
			var m1 = new SimpleMap();
			var m2 = new SimpleMap();

			Assert.Equal(m1, m2);
		}

		[Fact]
		public void ShouldGenerateAddEvent() {
			var map = new SimpleMap();
			map.TestFoo.Add(1, "hello");

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(EventAction.AddMap, e.Action);
			AssertPath(e, new[] {10});
		}
		
		[Fact]
		public void ShouldGenerateReplaceEvent() {
			var map = new SimpleMap();
			map.TestFoo[1] = "hello";

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(EventAction.ReplaceMap, e.Action);
			AssertPath(e, new[] {10});
		}
		
		[Fact]
		public void ShouldGenerateRemoveEvent() {
			var map = new SimpleMap();
			map.TestFoo[1] = "hello";
			map.Reset(); // throw away changes

			map.TestFoo.Remove(1);

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(EventAction.RemoveMap, e.Action);
			AssertPath(e, new[] {10});
		}
		
		[Fact]
		public void ShouldGenerateClearEvent() {
			var map = new SimpleMap();
			map.TestFoo[1] = "hello";
			map.Reset(); // throw away changes

			map.TestFoo.Clear();

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(EventAction.ClearMap, e.Action);
			AssertPath(e, new[] {10});
		}

		[Fact]
		public void ShouldApplyEventsDeterministically() {
			var map = new SimpleMap();
			map.TestFoo.Add(1, "hello");
			map.TestFoo[1] = "world";
			map.TestFoo.Add(2, "foo");
			map.TestFoo.Remove(2);
			map.TestFoo.Clear();
			map.TestFoo.Add(1, "hello");
			map.TestFoo[1] = "world";
			map.TestFoo.Add(2, "foo");
			map.TestFoo.Remove(2);

			var root = map.GenerateEvents();
			var newMap = new SimpleMap();
			newMap.ApplyEvents(root);

			Assert.Equal(map, newMap);
		}
	}
}