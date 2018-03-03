using Com.Zynga.Runtime.Protobuf;
using Xunit;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class UpgradeTests {
		[Fact]
		public void ShouldUpgradeSafely() {
			var message1 = new UpgradeMessage1();
			message1.LongA = 1;
			message1.StrA = "a";
			message1.EnumA = Enum1.NestedA1;
			message1.NestedA = new NestedMessage1 {IntA = 2};
			message1.ListA.Add(3);
			message1.MapA.Add(4, "b");
			message1.FloatA = 5;

			var events1 = message1.GenerateEvents();

			var message1New = new UpgradeMessage1();
			message1New.ApplyEvents(events1);
			Assert.Equal(message1, message1New);

			var message2 = new UpgradeMessage2();
			message2.ApplyEvents(events1);

			Assert.Equal(1, message2.LongB);
			Assert.Equal("a", message2.StrB);
			Assert.Equal(Enum1.NestedA1, message2.EnumB);
			Assert.Equal(new NestedMessage1 {IntA = 2}, message2.NestedB);
			Assert.Equal(3, message2.ListB[0]);
			Assert.Equal("b", message2.MapB[4]);
			Assert.Equal(5, message2.FloatB);

			var message3 = new UpgradeMessage3();
			message3.ApplyEvents(events1);

			message3.LongC = 6;
			message3.EnumB = Enum2.NestedC2;
			message3.NestedB.IntB = 7;
			message3.FloatC = 8;

			var events2 = message3.GenerateEvents();

			var message3New = new UpgradeMessage3();
			message3New.ApplyEvents(events1);
			message3New.ApplyEvents(events2);

			Assert.Equal(1, message3New.LongB);
			Assert.Equal("a", message3New.StrB);
			Assert.Equal(Enum2.NestedC2, message3New.EnumB);
			Assert.Equal(new NestedMessage2 {IntA = 2, IntB = 7}, message3New.NestedB);
			Assert.Equal(3, message3New.ListB[0]);
			Assert.Equal("b", message3New.MapB[4]);
			Assert.Equal(6, message3New.LongC);
			Assert.Equal(8, message3New.FloatC);

			var message4 = new UpgradeMessage4();
			message4.ApplyEvents(events1);
			message4.ApplyEvents(events2);

			Assert.Equal(1, message4.LongB);
			Assert.Equal(Enum2.NestedC2, message4.EnumB);
			Assert.Equal(new NestedMessage2 {IntA = 2, IntB = 7}, message4.NestedB);
			Assert.Equal(3, message4.ListB[0]);
			Assert.Equal("b", message4.MapB[4]);
			Assert.Equal(6, message4.LongC);
			Assert.Equal(8, message4.FloatC);
		}
	}
}