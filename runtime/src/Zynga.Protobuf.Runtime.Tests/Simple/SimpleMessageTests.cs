using Com.Zynga.Runtime.Protobuf;
using Xunit;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class SimpleMessageTests {
		[Fact]
		public void ReplacedMessageDoesntGenerateEvents() {
			var parent = new ParentMessage();
			var childA = new ChildMessage();
			parent.B = childA;
			var childB = new ChildMessage();
			parent.B = childB;

			parent.ClearEvents();

			// childA changes shouldn't generate events on the parent anymore
			childA.C = 100;
			AssertNotGenerated(parent);
		}

		[Fact]
		public void ShouldGenerateEventsWithObjectIntializer() {
			var a = new ChildMessage {D = new ChildChildMessage()};

			var b = new ChildMessage {
				D = new ChildChildMessage()
			};

			var parent = new ParentMessage {
				B = b
			};
			AssertGenerated(parent);
			parent.ClearEvents();

			parent.B.D.E = 10;
			AssertDeltaPath(parent, new int[]{2, 4, 5});
		}
	}
}
