using Com.Zynga.Runtime.Protobuf;
using Xunit;
using Zynga.Protobuf.Runtime.EventSource;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class SimpleListTests {
		[Fact]
		public void AreTheSame() {
			var l1 = new SimpleDeltaStringList();
			var l2 = new SimpleDeltaStringList();

			Assert.Equal(l1, l2);
		}

		[Fact]
		public void ShouldGenerateAddEvent() {
			var list = new SimpleDeltaStringList();
			list.TestBar.Add("hello");

			Assert.Equal(1, list.TestBar.Count);

			var root = AssertGenerated(list);
			var e = root.Events[0];
			Assert.Equal(EventAction.AddList, e.Action);
			AssertPath(e, new[] {11});
		}

		[Fact]
		public void ShouldGenerateRemoveEvent() {
			var list = new SimpleDeltaStringList();
			list.TestBar.Add("hello");
			list.Reset(); // throw away changes

			list.TestBar.Remove("hello");
			Assert.Equal(0, list.TestBar.Count);

			var root = AssertGenerated(list);
			var e = root.Events[0];
			Assert.Equal(EventAction.RemoveList, e.Action);
			AssertPath(e, new[] {11});
		}

		[Fact]
		public void ShouldGenerateRemoveAtEvent() {
			var list = new SimpleDeltaStringList();
			list.TestBar.Add("hello");
			list.TestBar.Add("world");
			list.Reset(); // throw away changes

			list.TestBar.RemoveAt(1);
			Assert.Equal(1, list.TestBar.Count);

			var root = AssertGenerated(list);
			var e = root.Events[0];
			Assert.Equal(EventAction.RemoveAtList, e.Action);
			AssertPath(e, new[] {11});
			Assert.Equal(1, e.Field);
		}

		[Fact]
		public void ShouldGenerateReplaceEvent() {
			var list = new SimpleDeltaStringList();
			list.TestBar.Add("hello");
			list.TestBar.Add("world");
			list.Reset(); // throw away changes

			list.TestBar[1] = "worlds";
			Assert.Equal(2, list.TestBar.Count);

			var root = AssertGenerated(list);
			var e = root.Events[0];
			Assert.Equal(EventAction.ReplaceList, e.Action);
			AssertPath(e, new[] {11});
			Assert.Equal(1, e.Field);
		}

		[Fact]
		public void ShouldGenerateInsertEvent() {
			var list = new SimpleDeltaStringList();
			list.TestBar.Add("hello");
			list.TestBar.Add("world");
			list.Reset(); // throw away changes

			list.TestBar.Insert(1, "all");
			Assert.Equal(3, list.TestBar.Count);

			var root = AssertGenerated(list);
			var e = root.Events[0];
			Assert.Equal(EventAction.InsertList, e.Action);
			AssertPath(e, new[] {11});
			Assert.Equal(1, e.Field);
		}

		[Fact]
		public void ShouldGenerateClearEvent() {
			var list = new SimpleDeltaStringList();
			list.TestBar.Add("hello");
			list.TestBar.Add("world");
			list.Reset(); // throw away changes

			list.TestBar.Clear();
			Assert.Equal(0, list.TestBar.Count);

			var root = AssertGenerated(list);
			var e = root.Events[0];
			Assert.Equal(EventAction.ClearList, e.Action);
			AssertPath(e, new[] {11});
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyStringList() {
			var list = new SimpleDeltaStringList();
			list.TestBar.Add("hello");
			list.TestBar.Add("world");
			list.TestBar.Add("bad");
			list.TestBar.Add("bad_index");
			list.TestBar[1] = "worlds";
			list.TestBar.Insert(1, "all");
			list.TestBar.Remove("bad");
			list.TestBar.RemoveAt(3);
			list.TestBar.Clear();
			list.TestBar.Add("hello");
			list.TestBar.Add("world");
			list.TestBar.Add("bad");
			list.TestBar.Add("bad_index");
			list.TestBar[1] = "worlds";
			list.TestBar.Insert(1, "all");
			list.TestBar.Remove("bad");
			list.TestBar.RemoveAt(3);

			var root = list.GenerateEvents();
			var newList = new SimpleDeltaStringList();
			newList.ApplyEvents(root);

			Assert.Equal(list, newList);
			Assert.Equal(3, newList.TestBar.Count);
			Assert.Equal("hello", newList.TestBar[0]);
			Assert.Equal("all", newList.TestBar[1]);
			Assert.Equal("worlds", newList.TestBar[2]);
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyLongList() {
			var list = new SimpleDeltaLongList();
			list.TestBar.Add(1);
			list.TestBar.Add(2);
			list.TestBar.Add(3);
			list.TestBar.Add(4);
			list.TestBar[1] = 5;
			list.TestBar.Insert(1, 6);
			list.TestBar.Remove(4);
			list.TestBar.RemoveAt(3);
			list.TestBar.Clear();
			list.TestBar.Add(1);
			list.TestBar.Add(2);
			list.TestBar.Add(3);
			list.TestBar.Add(4);
			list.TestBar[1] = 5;
			list.TestBar.Insert(1, 6);
			list.TestBar.Remove(4);
			list.TestBar.RemoveAt(3);

			var root = list.GenerateEvents();
			var newList = new SimpleDeltaLongList();
			newList.ApplyEvents(root);

			Assert.Equal(list, newList);
			Assert.Equal(3, newList.TestBar.Count);
			Assert.Equal(1, newList.TestBar[0]);
			Assert.Equal(6, newList.TestBar[1]);
			Assert.Equal(5, newList.TestBar[2]);
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyEnumList() {
			var list = new SimpleDeltaEnumList();
			list.TestBar.Add(SimpleListEnum.A);
			list.TestBar.Add(SimpleListEnum.B);
			list.TestBar.Add(SimpleListEnum.C);
			list.TestBar.Add(SimpleListEnum.D);
			list.TestBar[1] = SimpleListEnum.E;
			list.TestBar.Insert(1, SimpleListEnum.F);
			list.TestBar.Remove(SimpleListEnum.D);
			list.TestBar.RemoveAt(3);
			list.TestBar.Clear();
			list.TestBar.Add(SimpleListEnum.A);
			list.TestBar.Add(SimpleListEnum.B);
			list.TestBar.Add(SimpleListEnum.C);
			list.TestBar.Add(SimpleListEnum.D);
			list.TestBar[1] = SimpleListEnum.E;
			list.TestBar.Insert(1, SimpleListEnum.F);
			list.TestBar.Remove(SimpleListEnum.D);
			list.TestBar.RemoveAt(3);

			var root = list.GenerateEvents();
			var newList = new SimpleDeltaEnumList();
			newList.ApplyEvents(root);

			Assert.Equal(list, newList);
			Assert.Equal(3, newList.TestBar.Count);
			Assert.Equal(SimpleListEnum.A, newList.TestBar[0]);
			Assert.Equal(SimpleListEnum.F, newList.TestBar[1]);
			Assert.Equal(SimpleListEnum.E, newList.TestBar[2]);
		}

		[Fact]
		public void ShouldApplyEventsDeterministicallyMessageList() {
			var list = new SimpleDeltaMessageList();
			list.TestBar.Add(new SimpleListDeltaMessage {H = "hello"});
			list.TestBar.Add(new SimpleListDeltaMessage {H = "world"});
			list.TestBar.Add(new SimpleListDeltaMessage {H = "bad"});
			list.TestBar.Add(new SimpleListDeltaMessage {H = "bad_index"});
			list.TestBar[1] = new SimpleListDeltaMessage {H = "worlds"};
			list.TestBar.Insert(1, new SimpleListDeltaMessage {H = "all"});
			list.TestBar.Remove(new SimpleListDeltaMessage {H = "bad"});
			list.TestBar.RemoveAt(3);
			list.TestBar.Clear();
			list.TestBar.Add(new SimpleListDeltaMessage {H = "hello"});
			list.TestBar.Add(new SimpleListDeltaMessage {H = "world"});
			list.TestBar.Add(new SimpleListDeltaMessage {H = "bad"});
			list.TestBar.Add(new SimpleListDeltaMessage {H = "bad_index"});
			list.TestBar[1] = new SimpleListDeltaMessage {H = "worlds"};
			list.TestBar.Insert(1, new SimpleListDeltaMessage {H = "all"});
			list.TestBar.Remove(new SimpleListDeltaMessage {H = "bad"});
			list.TestBar.RemoveAt(3);

			var root = list.GenerateEvents();
			var newList = new SimpleDeltaMessageList();
			newList.ApplyEvents(root);

			Assert.Equal(list, newList);
			Assert.Equal(3, newList.TestBar.Count);
			Assert.Equal(new SimpleListDeltaMessage {H = "hello"}, newList.TestBar[0]);
			Assert.Equal(new SimpleListDeltaMessage {H = "all"}, newList.TestBar[1]);
			Assert.Equal(new SimpleListDeltaMessage {H = "worlds"}, newList.TestBar[2]);
		}
	}
}