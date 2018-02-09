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
	}
}
