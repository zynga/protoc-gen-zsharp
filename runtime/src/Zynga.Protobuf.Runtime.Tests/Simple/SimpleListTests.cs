using Com.Zynga.Runtime.Protobuf;
using Xunit;
using Zynga.Protobuf.Runtime.EventSource;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class SimpleListTests {
		[Fact]
		public void AreTheSame() {
			var l1 = new SimpleList();
			var l2 = new SimpleList();

			Assert.Equal(l1, l2);
		}

		[Fact]
		public void ShouldGenerateAddEvent() {
			var list = new SimpleList();
			list.TestBar.Add("hello");

			Assert.Equal(1, list.TestBar.Count);

			var root = AssertGenerated(list);
			var e = root.Events[0];
			Assert.Equal(EventAction.AddList, e.Action);
			AssertPath(e, new[] {11});
		}

		[Fact]
		public void ShouldGenerateRemoveEvent() {
			var list = new SimpleList();
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
			var list = new SimpleList();
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
			var list = new SimpleList();
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
			var list = new SimpleList();
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
			var list = new SimpleList();
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
		public void ShouldApplyEventsDeterministically() {
			var list = new SimpleList();
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
			var newList = new SimpleList();
			newList.ApplyEvents(root);

			Assert.Equal(list, newList);
			Assert.Equal(3, newList.TestBar.Count);
			Assert.Equal("hello", newList.TestBar[0]);
			Assert.Equal("all", newList.TestBar[1]);
			Assert.Equal("worlds", newList.TestBar[2]);
		}
	}
}