using Com.Zynga.Runtime.Protobuf;
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

			//blob.AddFoolist(new Foo{Long = 123});
			//blob.RemoveFoolist(0)  // This doesn't work atm
			blob.Foolist.Add(new Foo {Long = 321});
			blob.Foolist.Add(new Foo {Long = 1, Str = "la", Foo_ = new Foo()});

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
		public void ShouldProduceIdenticalTestBlobWithMergeFrom() {
			var blob = populated();

			var newBlob = new TestBlob();
			newBlob.MergeFrom(blob);
			Assert.Equal(blob, newBlob);
			Assert.Equal(newBlob, blob);
		}

		[Fact]
		public void ShouldNotGenerateDeltasForNoOpsOnMaps() {
			/*
			var blob = new TestBlob();
			blob.IntToString.Add(0, "a");
			AssertGenerated(blob);

			blob.IntToString.Add(0, "a");
			AssertNotGenerated(blob);
			*/
		}
	}
}