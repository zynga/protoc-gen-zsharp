using Com.Zynga.Runtime.Protobuf;
using Xunit;
using Zynga.Protobuf.Runtime.EventSource;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class SimpleMapTests {
		[Fact]
		public void AreTheSame() {
			var m1 = new SimpleLongToMessageDeltaMap();
			var m2 = new SimpleLongToMessageDeltaMap();

			Assert.Equal(m1, m2);
		}

		[Fact]
		public void ShouldGenerateAddEvent() {
			var map = new SimpleLongToMessageDeltaMap();
			map.TestFoo.Add(1, new SimpleMapDeltaMessage {H = "hello"});

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(MapAction.AddMap, e.MapEvent.MapAction);
			AssertPath(e, new[] {10});
		}

		[Fact]
		public void ShouldGenerateReplaceEvent() {
			var map = new SimpleLongToMessageDeltaMap();
			map.TestFoo[1] = new SimpleMapDeltaMessage {H = "hello"};

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(MapAction.ReplaceMap, e.MapEvent.MapAction);
			AssertPath(e, new[] {10});
		}

		[Fact]
		public void ShouldGenerateRemoveEvent() {
			var map = new SimpleLongToMessageDeltaMap();
			map.TestFoo[1] = new SimpleMapDeltaMessage {H = "hello"};
			map.ClearEvents(); // throw away changes

			map.TestFoo.Remove(1);

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(MapAction.RemoveMap, e.MapEvent.MapAction);
			AssertPath(e, new[] {10});
		}

		[Fact]
		public void ShouldGenerateClearEvent() {
			var map = new SimpleLongToMessageDeltaMap();
			map.TestFoo[1] = new SimpleMapDeltaMessage {H = "hello"};
			map.ClearEvents(); // throw away changes

			map.TestFoo.Clear();

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(MapAction.ClearMap, e.MapEvent.MapAction);
			AssertPath(e, new[] {10});
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyLongToMessageDeltaMap() {
			var map = new SimpleLongToMessageDeltaMap();
			map.TestFoo.Add(1, new SimpleMapDeltaMessage {H = "hello"});
			map.TestFoo.Add(2, new SimpleMapDeltaMessage {H = "world"});
			map.TestFoo.Add(3, new SimpleMapDeltaMessage {H = "bad"});
			map.TestFoo[2] = new SimpleMapDeltaMessage {H = "worlds"};
			map.TestFoo.Remove(3);
			map.TestFoo[4] = new SimpleMapDeltaMessage {H = "all"};
			map.TestFoo.Clear();
			map.TestFoo.Add(1, new SimpleMapDeltaMessage {H = "hello"});
			map.TestFoo.Add(2, new SimpleMapDeltaMessage {H = "world"});
			map.TestFoo.Add(3, new SimpleMapDeltaMessage {H = "bad"});
			map.TestFoo[2] = new SimpleMapDeltaMessage {H = "worlds"};
			map.TestFoo.Remove(3);
			map.TestFoo[4] = new SimpleMapDeltaMessage {H = "all"};

			var root = map.GenerateEvents();
			var newMap = new SimpleLongToMessageDeltaMap();
			newMap.ApplyEvents(root);

			Assert.Equal(map, newMap);
			Assert.Equal(3, map.TestFoo.Count);
			Assert.Equal(new SimpleMapDeltaMessage {H = "hello"}, map.TestFoo[1]);
			Assert.Equal(new SimpleMapDeltaMessage {H = "all"}, map.TestFoo[4]);
			Assert.Equal(new SimpleMapDeltaMessage {H = "worlds"}, map.TestFoo[2]);
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyStringToEnumDeltaMap() {
			var map = new SimpleStringToEnumDeltaMap();
			map.TestFoo.Add("1", SimpleMapEnum.A);
			map.TestFoo.Add("2", SimpleMapEnum.B);
			map.TestFoo.Add("3", SimpleMapEnum.C);
			map.TestFoo["2"] = SimpleMapEnum.D;
			map.TestFoo.Remove("3");
			map.TestFoo["4"] = SimpleMapEnum.E;
			map.TestFoo.Clear();
			map.TestFoo.Add("1", SimpleMapEnum.A);
			map.TestFoo.Add("2", SimpleMapEnum.B);
			map.TestFoo.Add("3", SimpleMapEnum.C);
			map.TestFoo["2"] = SimpleMapEnum.D;
			map.TestFoo.Remove("3");
			map.TestFoo["4"] = SimpleMapEnum.E;

			var root = map.GenerateEvents();
			var newMap = new SimpleStringToEnumDeltaMap();
			newMap.ApplyEvents(root);

			Assert.Equal(map, newMap);
			Assert.Equal(3, map.TestFoo.Count);
			Assert.Equal(SimpleMapEnum.A, map.TestFoo["1"]);
			Assert.Equal(SimpleMapEnum.E, map.TestFoo["4"]);
			Assert.Equal(SimpleMapEnum.D, map.TestFoo["2"]);
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyStringToStringDeltaMap() {
			var map = new SimpleStringToStringDeltaMap();
			map.TestFoo.Add("1", "a");
			map.TestFoo.Add("2", "b");
			map.TestFoo.Add("3", "c");
			map.TestFoo["2"] = "d";
			map.TestFoo.Remove("3");
			map.TestFoo["4"] = "e";
			map.TestFoo.Clear();
			map.TestFoo.Add("1", "a");
			map.TestFoo.Add("2", "b");
			map.TestFoo.Add("3", "c");
			map.TestFoo["2"] = "d";
			map.TestFoo.Remove("3");
			map.TestFoo["4"] = "e";

			var root = map.GenerateEvents();
			var newMap = new SimpleStringToStringDeltaMap();
			newMap.ApplyEvents(root);

			Assert.Equal(map, newMap);
			Assert.Equal(3, map.TestFoo.Count);
			Assert.Equal("a", map.TestFoo["1"]);
			Assert.Equal("e", map.TestFoo["4"]);
			Assert.Equal("d", map.TestFoo["2"]);
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyStringToLongDeltaMap() {
			var map = new SimpleStringToLongDeltaMap();
			map.TestFoo.Add("1", 1);
			map.TestFoo.Add("2", 2);
			map.TestFoo.Add("3", 3);
			map.TestFoo["2"] = 4;
			map.TestFoo.Remove("3");
			map.TestFoo["4"] = 5;
			map.TestFoo.Clear();
			map.TestFoo.Add("1", 1);
			map.TestFoo.Add("2", 2);
			map.TestFoo.Add("3", 3);
			map.TestFoo["2"] = 4;
			map.TestFoo.Remove("3");
			map.TestFoo["4"] = 5;

			var root = map.GenerateEvents();
			var newMap = new SimpleStringToLongDeltaMap();
			newMap.ApplyEvents(root);

			Assert.Equal(map, newMap);
			Assert.Equal(3, map.TestFoo.Count);
			Assert.Equal(1, map.TestFoo["1"]);
			Assert.Equal(5, map.TestFoo["4"]);
			Assert.Equal(4, map.TestFoo["2"]);
		}

		[Fact]
		public void ShouldGenerateDeltasForMessagesInMap() {
			var map = new SimpleLongToMessageDeltaMap();
			map.TestFoo.Add(1, new SimpleMapDeltaMessage {H = "hello"});
			map.ClearEvents();

			map.TestFoo[1].H = "world";

			var root = AssertGenerated(map);
			var e = root.Events[0];
			Assert.Equal(EventData.ActionOneofCase.MapEvent, e.ActionCase);
			AssertPath(e, new[] {10});
			var me = e.MapEvent;
			Assert.Equal(MapAction.UpdateMap, me.MapAction);

			Assert.Equal(EventData.ActionOneofCase.Set, me.EventData.ActionCase);
			AssertPath(me.EventData, new[] {1});
		}
	}
}