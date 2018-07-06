using Com.Zynga.Runtime.Protobuf;
using Google.Protobuf;
using Xunit;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class StructTests {
		[Fact]
		public void CanSerializeTestStruct() {
			var a = new TestStruct();
			a.Long = 10;

			var serialized = a.ToByteString();
			Assert.True(serialized.Length > 0);

			var b = new TestStruct();
			b.MergeFrom(serialized);

			Assert.Equal(a, b);
		}

		[Fact]
		public void CanSerializeTestMessageNestedStruct() {
			var message = new TestMessageNestedStruct();
			message.A = new TestStruct {Long = 10};

			var serialized = message.ToByteString();
			Assert.True(serialized.Length > 0);

			var b = new TestMessageNestedStruct();
			b.MergeFrom(serialized);

			Assert.Equal(message, b);
		}
	}
}