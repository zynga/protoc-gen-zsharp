using Com.Zynga.Runtime.Protobuf.File;
using Xunit;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class ComplexTests {
		[Fact]
		public void DeeplyNestedMessagesShouldApplyEventsProperly() {
			var g = new MessageG();
			g.H.Add(1);
			AssertEventsStable(g);

			var f = new MessageF();
			f.G = g;
			f.G.H.Add(2);
			AssertEventsStable(f);

			var e = new MessageE();
			e.F = f;
			e.F.G.H.Add(3);
			AssertEventsStable(e);

			var d = new MessageD();
			d.E = e;
			d.E.F.G.H.Add(4);
			AssertEventsStable(d);

			var c = new MessageC();
			c.D["foo"] = d;
			c.D["foo"].E.F.G.H.Add(5);
			AssertEventsStable(c);

			var b = new MessageB();
			b.C["bar"] = c;
			b.C["bar"].D["foo"].E.F.G.H.Add(6);
			AssertEventsStable(b);

			var a = new MessageA();
			a.B = b;
			a.B.C["bar"].D["foo"].E.F.G.H.Add(7);
			AssertEventsStable(a);
		}

		[Fact]
		public void DeeplyNestedMessagesInRepeatedFieldShouldApplyEventsProperly() {
			var state = new MessageO {
				P = new MessageP {
					Q = new MessageQ()
				}
			};

			var s = new MessageR();
			s.R = "hello";

			state.P.Q.R.Add(s);

			AssertEventsStable(state);

			AssertEventsStableWithClone(state, () => { s.R = "World"; });

			var newState = new MessageO();
			newState.ApplyEvents(state.GenerateSnapshot());

			AssertEventsStableWithClone(newState, () => { newState.P.Q.R[0].R = "Worlds"; });
		}
	}
}
