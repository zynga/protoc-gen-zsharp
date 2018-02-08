using System.IO;
using Com.Zynga.Runtime.Protobuf;
using Xunit;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class BasicTests {
		[Fact]
		public void AreTheSame() {
			var e1 = new EventTest();
			var e2 = new EventTest();

			Assert.Equal(e1.GetHashCode(), e2.GetHashCode());
		}

		[Fact]
		public void OneEvent() {
			var e1 = new EventTest();
			e1.EventId = "Heool World";
			AssertGenerated(e1);
		}

		[Fact]
		public void NotSame() {
			var e1 = new EventTest();
			e1.EventId = "Heool World";

			var e2 = new EventTest();

			Assert.NotEqual(e1.GetHashCode(), e2.GetHashCode());
		}

		[Fact]
		public void SnapshotSame()
		{
		    var e1 = new EventTest();
		    e1.EventId = "Heool World";

		    var e2 = new EventTest();
		    e2.ApplyEvents(e1.GenerateSnapshot());

		    Assert.Equal(e1, e2);
		}

		[Fact]
		public void EventSame() {
			var e1 = new EventTest();
			e1.EventId = "Heool World";

			var e2 = new EventTest();
			e2.ApplyEvents(e1.GenerateEvents());

			Assert.Equal(e1.GetHashCode(), e2.GetHashCode());
		}

		[Fact]
		public void EventFieldSame() {
			var e1 = new EventTest();
			e1.EventId = "Heool World";

			var e2 = new EventTest();
			e2.ApplyEvents(e1.GenerateEvents());

			Assert.Equal(e1.EventId, e2.EventId);
		}

		[Fact]
		public void EventFieldNotSame() {
			var e1 = new EventTest();
			e1.EventId = "Heool World";

			var e2 = new EventTest();
			e2.ApplyEvents(e1.GenerateEvents());

			e1.EventId = "Foo World";


			Assert.NotEqual(e1.EventId, e2.EventId);
		}

		[Fact]
		public void AreTheChecksumSame() {
			var e1 = new EventTest();
			e1.EventId = "Hello World";
			var e2 = new EventTest();
			e2.EventId = "Hello World";

			var m1 = new MemoryStream();
			var m2 = new MemoryStream();

			var b1 = new BinaryWriter(m1);
			var b2 = new BinaryWriter(m2);

			e1.GetChecksum(b1);
			e2.GetChecksum(b2);

			Assert.Equal(m1.GetBuffer(), m2.GetBuffer());
		}
	}
}