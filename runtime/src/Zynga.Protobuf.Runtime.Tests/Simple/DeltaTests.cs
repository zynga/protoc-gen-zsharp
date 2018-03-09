using Com.Zynga.Runtime.Protobuf;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Xunit;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class DeltaTests {
		private TestBlob populated() {
			var blob = new TestBlob();
			blob.Bar = new Bar();
			blob.Bar.Foo = new Foo();
			blob.Bar.Foo.Long = 10;
			blob.Bar.Foo.Str = "world";
			blob.Foo = new Foo();
			blob.Foo.Long = 12;
			blob.Foo.Str = "hello";

			blob.IntToString.Add(10, "world");
			blob.StringToFoo.Add("hello", new Foo {Long = 9, Str = "ha"});
			blob.StringToFoo["hello"].Str = "happy";
			blob.StringToFoo["hello"].Long = 10;

			blob.Foolist.Add(new Foo {Long = 123});
			blob.Foolist[0].Long = 321;
			blob.Foolist.Add(new Foo {Long = 1, Str = "la", Foo_ = new Foo()});
			blob.Foolist[1].Str = "lalala";
			blob.Foolist[1].Long = 2;

			blob.Ilist.Add(12);
			blob.Ilist.Add(51);

			blob.Maybefoo = new Foo {Long = 42, Str = "maybe_foo_who_knows"};

			blob.Timestamp = new Timestamp {
				Seconds = 1234,
				Nanos = 987
			};
			blob.Duration = new Duration {
				Seconds = 111,
				Nanos = 2
			};

			return blob;
		}

		[Fact]
		public void CalculateSizeOfZero() {
			var blob = new TestBlob();

			Assert.Equal(0, blob.CalculateSize());
		}

		[Fact]
		public void HaveOneForHashCode() {
			var blob = new TestBlob();
			Assert.Equal(1, blob.GetHashCode());
		}

		[Fact]
		public void PopulatedFooShouldHaveCorrectSize() {
			Assert.Equal(0, new Foo().CalculateSize());
			Assert.Equal(2, new Foo {Long = 1}.CalculateSize());
			Assert.Equal(4, new Foo {Str = "hi"}.CalculateSize());
		}

		[Fact]
		public void ListOfMessagesShouldHaveTheCorrectSize() {
			var blob = new TestBlob();
			blob.Foolist.Add(new Foo());
			blob.Foolist.Add(new Foo());
			Assert.Equal(4, blob.CalculateSize());
		}

		[Fact]
		public void ListOfIntsShouldHaveTheCorrectSize() {
			var blob = new TestBlob();
			blob.Ilist.Add(0);
			blob.Ilist.Add(1);
			blob.Ilist.Add(300);
			Assert.Equal(6, blob.CalculateSize());
		}

		[Fact]
		public void MapOfIntToStringShouldHaveTheCorrectSize() {
			var blob = new TestBlob();
			blob.IntToString.Add(0, "");
			blob.IntToString.Add(10, "");
			blob.IntToString.Add(1, "hi");
			blob.IntToString.Add(300, "hi");
			Assert.Equal(23, blob.CalculateSize());
		}

		[Fact]
		public void MapOfStringToFooShouldHaveTheCorrectSize() {
			var blob = new TestBlob();
			blob.StringToFoo.Add("", new Foo());
			blob.StringToFoo.Add("hi", new Foo());
			blob.StringToFoo.Add("bye", new Foo {Long = 300});
			Assert.Equal(24, blob.CalculateSize());
		}

		[Fact]
		public void OneOfFieldShouldSetProperly() {
			var blob = new TestBlob();
			blob.Maybefoo = new Foo();
			Assert.Equal(TestBlob.TestOneofCase.Maybefoo, blob.TestCase);

			blob.Maybeint = 2;
			Assert.Null(blob.Maybefoo);
			Assert.Equal(TestBlob.TestOneofCase.Maybeint, blob.TestCase);
		}

		[Fact]
		public void PopulatedDeltaShouldEqualCloned() {
			var blob = populated();
			var newBlob = blob.Clone();

			Assert.Equal(blob, newBlob);
		}

		[Fact]
		public void AppliedDeltasShouldEqual() {
			var blob = populated();
			var root = blob.PeekEvents();
			blob.ClearEvents();

			var newBlob = new TestBlob();
			newBlob.ApplyEvents(root);

			Assert.Equal(blob, newBlob);
		}

		[Fact]
		public void ShouldProduceDeltasWithTheCorrectPaths() {
			var emptyBlob = new TestBlob();
			var events = emptyBlob.PeekEvents();
			Assert.Equal(0, events.Events.Count);

			var blob = new TestBlob();
			blob.Bar = new Bar();
			blob.Bar.Foo = new Foo();
			Assert.Equal(2, blob.PeekEvents().Events.Count);
			blob.ClearEvents();

			blob.Bar.Foo.Long = 1;
			AssertDeltaPath(blob, new[] {1, 1, 1});

			blob.Bar.Foo.Str = "test";
			AssertDeltaPath(blob, new[] {1, 1, 2});

			blob.Foo = new Foo();
			AssertDeltaPath(blob, new[] {2});

			blob.Foo.Long = 4;
			AssertDeltaPath(blob, new[] {2, 1});

			blob.Foo.Str = "asdf";
			AssertDeltaPath(blob, new[] {2, 2});

			blob.IntToString.Add(0, "");
			AssertDeltaPath(blob, new[] {3});

			blob.IntToString.Add(1, "asdf");
			AssertDeltaPath(blob, new[] {3});

			blob.IntToString.Remove(0);
			AssertDeltaPath(blob, new[] {3});

			blob.IntToString.Remove(1);
			AssertDeltaPath(blob, new[] {3});

			blob.StringToFoo.Add("a", new Foo());
			AssertDeltaPath(blob, new[] {4});

			blob.StringToFoo.Add("b", new Foo {Long = 12});
			AssertDeltaPath(blob, new[] {4});

			blob.StringToFoo.Remove("a");
			AssertDeltaPath(blob, new[] {4});

			blob.StringToFoo.Remove("b");
			AssertDeltaPath(blob, new[] {4});

			blob.Ilist.Add(20);
			AssertDeltaPath(blob, new[] {5});

			blob.Slist.Add("as");
			AssertDeltaPath(blob, new[] {6});

			blob.Foolist.Add(new Foo());
			AssertDeltaPath(blob, new[] {7});
		}

		[Fact]
		public void ShouldNotGenerateDeltasForNoOpsOnMaps() {
			var blob = new TestBlob();
			blob.IntToString[0] = "a";
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.IntToString[0] = "a";
			AssertNotGenerated(blob);

			blob.IntToString[0] = "b";
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.IntToString[0] = "a";
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.IntToString.Remove(1);
			AssertNotGenerated(blob);

			blob.IntToString.Remove(0);
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.IntToString.Remove(0);
			AssertNotGenerated(blob);

			blob.StringToFoo["a"] = new Foo();
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.StringToFoo["a"] = new Foo {Long = 1};
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.StringToFoo["a"] = new Foo {Long = 1, Str = "test"};
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.StringToFoo["a"] = new Foo {Long = 1, Str = "test"};
			AssertNotGenerated(blob);

			blob.StringToFoo["a"] = new Foo();
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.StringToFoo.Remove("a");
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.StringToFoo.Remove("a");
			AssertNotGenerated(blob);

			blob.StringToFoo.Remove("b");
			AssertNotGenerated(blob);
		}

		[Fact]
		public void ShouldNotGenerateDeltasForNoOpsOnPrimitiveFields() {
			var blob = new TestBlob();
			blob.Foo = new Foo();
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Foo = new Foo();
			AssertNotGenerated(blob);

			blob.Foo.Long = 0L;
			AssertNotGenerated(blob);

			blob.Foo.Long = 1L;
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Foo.Long = 1L;
			AssertNotGenerated(blob);

			blob.Foo.Long = 0L;
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Foo.Str = "1 long please";
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Foo.Str = "1 long please";
			AssertNotGenerated(blob);

			blob.Foo.Str = "";
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Foo.Str = "";
			AssertNotGenerated(blob);
		}

		[Fact]
		public void ShouldNotGenerateDeltasForNoOpsOnOneOfs() {
			var blob = new TestBlob();

			blob.Maybestring = "hello";
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Maybestring = "hello";
			AssertNotGenerated(blob);

			blob.Maybefoo = new Foo {Long = 2L};
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Maybefoo = new Foo {Long = 2L};
			AssertNotGenerated(blob);

			blob.Maybefoo = new Foo {Long = 2L, Str = "hi"};
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Maybeint = 2;
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Maybeint = 2;
			AssertNotGenerated(blob);
		}

		[Fact]
		public void ShouldNotGenerateDeltasForNoOpsOnLists() {
			var blob = new TestBlob();

			blob.Ilist.Add(1);
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Ilist[0] = 1;
			AssertNotGenerated(blob);

			blob.Foolist.Add(new Foo { Long = 1 });
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Foolist[0] = new Foo { Long = 1 };
			AssertNotGenerated(blob);

			blob.Slist.Add("hello");
			AssertGenerated(blob);
			blob.ClearEvents();

			blob.Slist[0] = "hello";
			AssertNotGenerated(blob);
		}

		private RecursiveMap CreateRecursiveMap(int depth) {
			var map = new RecursiveMap();
			if (depth > 0) {
				map.Map[depth] = CreateRecursiveMap(depth - 1);
			}

			return map;
		}

		private void AddRecusiveMapFields(RecursiveMap map) {
			map.Primitives = new AllPrimitives();
			map.Primitives.A = 10;
			map.Primitives.B = 11;
			map.Primitives.C = 12;
			map.Primitives.D = 13;
			map.Primitives.E = 14;
			map.Primitives.F = 15;
			map.Primitives.G = 16.1;
			map.Primitives.H = 17.2F;
			map.Primitives.I = true;
			map.Primitives.J = "18";
			map.Primitives.K = ByteString.CopyFromUtf8("19");
			map.Primitives.L = 20;
			map.Primitives.M = 21;
			map.Primitives.N = -22;
			map.Primitives.O = -23;

			map.Bar = new Bar();
			map.Bar.Foo = new Foo();
			map.Enumero = Enumero.Halp;

			foreach (var m in map.Map) {
				AddRecusiveMapFields(m.Value);
			}
		}

		[Fact]
		public void ShouldGenerateValidDeltasForRecursiveMap() {
			var map = CreateRecursiveMap(10);
			AddRecusiveMapFields(map);
			var root = map.GenerateEvents();
			var newMap = new RecursiveMap();
			newMap.ApplyEvents(root);
			Assert.Equal(map, newMap);
		}

		private RecursiveList CeateRecusiveList(int depth) {
			var list = new RecursiveList();
			if (depth > 0) {
				list.List.Add(CeateRecusiveList(depth - 1));
			}

			return list;
		}

		private void AddRecusiveListFields(RecursiveList list) {
			list.Primitives = new AllPrimitives();
			list.Primitives.A = 10;
			list.Primitives.B = 11;
			list.Primitives.C = 12;
			list.Primitives.D = 13;
			list.Primitives.E = 14;
			list.Primitives.F = 15;
			list.Primitives.G = 16.1;
			list.Primitives.H = 17.2F;
			list.Primitives.I = true;
			list.Primitives.J = "18";
			list.Primitives.K = ByteString.CopyFromUtf8("19");
			list.Primitives.L = 20;
			list.Primitives.M = 21;
			list.Primitives.N = -22;
			list.Primitives.O = -23;

			list.Bar = new Bar();
			list.Bar.Foo = new Foo();
			list.Enumero = Enumero.Halp;

			foreach (var l in list.List) {
				AddRecusiveListFields(l);
			}
		}

		[Fact]
		public void ShouldGenerateValidDeltasForRecursiveList() {
			var list = CeateRecusiveList(10);
			AddRecusiveListFields(list);
			var root = list.GenerateEvents();
			var newList = new RecursiveList();
			newList.ApplyEvents(root);
			Assert.Equal(list, newList);
		}

		[Fact]
		public void EventsShouldBeClearedAfterGeneratingSnapshot() {
			var blob = populated();
			var snapshot = blob.GenerateSnapshot();
			var events = blob.GenerateEvents();

			var newBlob = new TestBlob();
			newBlob.ApplyEvents(snapshot);
			newBlob.ApplyEvents(events);

			Assert.Equal(blob, newBlob);
		}

		[Fact]
		public void ShouldNotGenerateEventsWhenDisabled() {
			var blob = new TestBlob();

			// disable events
			blob.EventsEnabled = false;

			blob.Bar = new Bar();
			blob.Bar.Foo = new Foo();
			blob.Bar.Foo.Long = 10;
			blob.Bar.Foo.Str = "world";
			blob.Foo = new Foo();
			blob.Foo.Long = 12;
			blob.Foo.Str = "hello";

			blob.IntToString.Add(10, "world");
			blob.StringToFoo.Add("hello", new Foo {Long = 9, Str = "ha"});
			blob.StringToFoo["hello"].Str = "happy";
			blob.StringToFoo["hello"].Long = 10;

			blob.Foolist.Add(new Foo {Long = 123});
			blob.Foolist[0].Long = 321;
			blob.Foolist.Add(new Foo {Long = 1, Str = "la", Foo_ = new Foo()});
			blob.Foolist[1].Str = "lalala";
			blob.Foolist[1].Long = 2;

			blob.Ilist.Add(12);
			blob.Ilist.Add(51);

			blob.Maybefoo = new Foo {Long = 42, Str = "maybe_foo_who_knows"};

			blob.Timestamp = new Timestamp {
				Seconds = 1234,
				Nanos = 987
			};
			blob.Duration = new Duration {
				Seconds = 111,
				Nanos = 2
			};

			AssertNotGenerated(blob);
		}

		[Fact]
		public void ChangesToComplexChildMessageShouldHaveCorrectPath() {
			var blob = populated();
			var childBlob = populated();
			blob.TestBlob_ = childBlob;

			var snapshot = blob.GenerateSnapshot();

			var blobA = new TestBlob();
			blobA.ApplyEvents(snapshot);

			Assert.Equal(blob, blobA);

			blob.TestBlob_.Bar = new Bar();
			blob.TestBlob_.Bar.Foo = new Foo();
			blob.TestBlob_.Bar.Foo.Long = 10;
			blob.TestBlob_.Bar.Foo.Str = "world";
			blob.TestBlob_.Foo = new Foo();
			blob.TestBlob_.Foo.Long = 12;
			blob.TestBlob_.Foo.Str = "hello";

			blob.TestBlob_.IntToString.Add(11, "world");
			blob.TestBlob_.StringToFoo.Add("helloa", new Foo {Long = 9, Str = "ha"});
			blob.TestBlob_.StringToFoo["helloa"].Str = "happy";
			blob.TestBlob_.StringToFoo["helloa"].Long = 10;

			blob.TestBlob_.Foolist.Add(new Foo {Long = 123});
			blob.TestBlob_.Foolist[2].Long = 321;
			blob.TestBlob_.Foolist.Add(new Foo {Long = 1, Str = "la", Foo_ = new Foo()});
			blob.TestBlob_.Foolist[3].Str = "lalala";
			blob.TestBlob_.Foolist[3].Long = 2;

			blob.TestBlob_.Ilist.Add(12);
			blob.TestBlob_.Ilist.Add(51);

			blob.TestBlob_.Maybefoo = new Foo {Long = 42, Str = "maybe_foo_who_knows"};

			blob.TestBlob_.Timestamp = new Timestamp {
				Seconds = 1235,
				Nanos = 987
			};
			blob.TestBlob_.Duration = new Duration {
				Seconds = 112,
				Nanos = 2
			};

			var events = blob.GenerateEvents();
			foreach (var e in events.Events) {
				// TestBlob test_blob = 16;
				// each event path should start with the number 16
				Assert.Equal(16, e.Path[0]);
			}

			var blobB = new TestBlob();
			blobB.ApplyEvents(snapshot);
			blobB.ApplyEvents(events);

			Assert.Equal(blob, blobB);
		}
	}
}