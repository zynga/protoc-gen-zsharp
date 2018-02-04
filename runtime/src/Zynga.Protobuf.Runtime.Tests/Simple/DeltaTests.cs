using System;
using Com.Zynga.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Xunit;
using Zynga.Protobuf.Runtime.EventSource;

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

			blob.AddIntToString(10, "world");
			blob.AddStringToFoo("hello", new Foo {Long = 9, Str = "ha"});

			//blob.AddFoolist(new Foo{Long = 123});
			//blob.RemoveFoolist(0)  // This doesn't work atm
			blob.AddFoolist(new Foo {Long = 321});
			blob.AddFoolist(new Foo {Long = 1, Str = "la", Foo_ = new Foo()});

			blob.Addilist(12);
			blob.Addilist(51);

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
			blob.AddFoolist(new Foo());
			blob.AddFoolist(new Foo());
			Assert.Equal(4, blob.CalculateSize());
		}

		[Fact]
		public void ListOfIntsShouldHaveTheCorrectSize() {
			var blob = new TestBlob();
			blob.Addilist(0);
			blob.Addilist(1);
			blob.Addilist(300);
			Assert.Equal(6, blob.CalculateSize());
		}

		[Fact]
		public void MapOfIntToStringShouldHaveTheCorrectSize() {
			var blob = new TestBlob();
			blob.AddIntToString(0, "");
			blob.AddIntToString(10, "");
			blob.AddIntToString(1, "hi");
			blob.AddIntToString(300, "hi");
			Assert.Equal(23, blob.CalculateSize());
		}

		[Fact]
		public void MapOfStringToFooShouldHaveTheCorrectSize() {
			var blob = new TestBlob();
			blob.AddStringToFoo("", new Foo());
			blob.AddStringToFoo("hi", new Foo());
			blob.AddStringToFoo("bye", new Foo {Long = 300});
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

		// TODO: write paths test

		private static void AssertPath(EventData eventData, int[] path) {
			Assert.Equal(path.Length, eventData.Path.Count);
			for (int i = 0; i < path.Length; i++) {
				Assert.Equal(path[i], eventData.Path[i]);
			}
		}

		private static void AssertDeltaPath(TestBlob blob, int[] path) {
			EventSourceRoot root = blob.GenerateEvents();
			Assert.Equal(1, root.Events.Count);
			AssertPath(root.Events[0], path);
			blob.Reset();
		}

		[Fact]
		public void AppliedDeltasShouldEqual() {
			var blob = populated();
			var root = blob.GenerateEvents();
			blob.Reset();

			var newBlob = new TestBlob();
			newBlob.ApplyEvents(root);

			Assert.Equal(blob, newBlob);
		}

		[Fact]
		public void ShouldProduceDeltasWithTheCorrectPaths() {
			var emptyBlob = new TestBlob();
			var events = emptyBlob.GenerateEvents();
			Assert.Equal(0, events.Events.Count);

			var blob = new TestBlob();
			blob.Bar = new Bar();
			blob.Bar.Foo = new Foo();
			Assert.Equal(2, blob.GenerateEvents().Events.Count);
			blob.Reset();

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

			blob.AddIntToString(0, "");
			AssertDeltaPath(blob, new[] {3});

			blob.AddIntToString(1, "asdf");
			AssertDeltaPath(blob, new[] {3});

			blob.RemoveIntToString(0);
			AssertDeltaPath(blob, new[] {3});

			blob.RemoveIntToString(1);
			AssertDeltaPath(blob, new[] {3});

			blob.AddStringToFoo("a", new Foo());
			AssertDeltaPath(blob, new[] {4});

			blob.AddStringToFoo("b", new Foo {Long = 12});
			AssertDeltaPath(blob, new[] {4});

			blob.RemoveStringToFoo("a");
			AssertDeltaPath(blob, new[] {4});

			blob.RemoveStringToFoo("b");
			AssertDeltaPath(blob, new[] {4});

			blob.Addilist(20);
			AssertDeltaPath(blob, new[] {5});

			blob.Addslist("as");
			AssertDeltaPath(blob, new[] {6});

			blob.AddFoolist(new Foo());
			AssertDeltaPath(blob, new[] {7});
		}

		[Fact]
		public void ShouldProduceIdenticalTestBlobWithMergeFrom() {
			var blob = populated();

			var newBlob = new TestBlob();
			newBlob.MergeFrom(blob);
			Assert.Equal(blob, newBlob);
			Assert.Equal(newBlob, blob);

			// should clear any pending events after a merge
			blob.MergeFrom(newBlob);
			Assert.Equal(0, blob.GenerateEvents().Events.Count);
		}
	}
}