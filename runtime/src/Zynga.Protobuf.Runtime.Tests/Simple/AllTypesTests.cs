using Xunit;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;
using static Zynga.Protobuf.Runtime.Tests.Simple.AllTypesTestsHelper;
using Google.Protobuf.TestProtos;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class AllTypesTests {
		[Fact]
		public void ShouldGenerateValidStableEventsWithClone() {
			var allTypes = new TestAllTypes();
			AssertEventsStableWithClone(allTypes, () => { ApplyAllChanges(allTypes); });
		}

		[Fact]
		public void ShouldGenerateValidStableEventsSingleStepWithClone() {
			var allTypes = new TestAllTypes();
			// single
			AssertEventsStableWithClone(allTypes, () => { SetSingular(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { SetEnums(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { SetNestedMessages(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { UpdateNestedMessages(allTypes); });

			// lists
			AssertEventsStableWithClone(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { RemoveRepeated(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { RemoveAtRepeated(allTypes, 0); });
			AssertEventsStableWithClone(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { ReplaceRepeated(allTypes, 0); }, false);
			AssertEventsStableWithClone(allTypes, () => { ReplaceRepeated(allTypes, 0, 1); });
			AssertEventsStableWithClone(allTypes, () => { InsertRepeated(allTypes, 0, 2); });
			AssertEventsStableWithClone(allTypes, () => { ClearRepeated(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { UpdateRepeated(allTypes, 0, 1); });

			// maps
			AssertEventsStableWithClone(allTypes, () => { AddMap(allTypes, 1); });
			AssertEventsStableWithClone(allTypes, () => { RemoveMap(allTypes, 1); });
			AssertEventsStableWithClone(allTypes, () => { AddMap(allTypes, 1); });
			AssertEventsStableWithClone(allTypes, () => { ReplaceMap(allTypes, 1); }, false);
			AssertEventsStableWithClone(allTypes, () => { ReplaceMap(allTypes, 1, 1); });
			AssertEventsStableWithClone(allTypes, () => { ClearMap(allTypes); });
			AssertEventsStableWithClone(allTypes, () => { AddMap(allTypes, 1); });
			AssertEventsStableWithClone(allTypes, () => { UpdateMap(allTypes, 1, 1); });

			// oneof
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofUint32 = UintValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofNestedMessage = NestedMessageValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofString = StringValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofBytes = ByteValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofForeignMessage = ForeignMessageValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofForeignMessageNoEvents = ForeignMessageNoEventsValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofAllTypes = TestAllTypesValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.OneofAllTypesNoEvents = TestAllTypesNoEventsValue(0); });

			// nested types
			AssertEventsStableWithClone(allTypes, () => { allTypes.AllTypes = TestAllTypesValue(0); });
			AssertEventsStableWithClone(allTypes, () => { allTypes.AllTypesNoEvents = TestAllTypesNoEventsValue(0); });
		}

		[Fact]
		public void ShouldGenerateValidStableEventsWithSnapshot() {
			var allTypes = new TestAllTypes();
			AssertEventsStableWithSnapshot(allTypes, () => {
				ApplyAllChanges(allTypes);
			});
		}

		[Fact]
		public void ShouldGenerateValidStableEventsSingleStepWithSnapshot() {
			var allTypes = new TestAllTypes();
			// single
			AssertEventsStableWithSnapshot(allTypes, () => { SetSingular(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { SetEnums(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { SetNestedMessages(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { UpdateNestedMessages(allTypes); });

			// lists
			AssertEventsStableWithSnapshot(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { RemoveRepeated(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { RemoveAtRepeated(allTypes, 0); });
			AssertEventsStableWithSnapshot(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { ReplaceRepeated(allTypes, 0); }, false);
			AssertEventsStableWithSnapshot(allTypes, () => { ReplaceRepeated(allTypes, 0, 1); });
			AssertEventsStableWithSnapshot(allTypes, () => { InsertRepeated(allTypes, 0, 2); });
			AssertEventsStableWithSnapshot(allTypes, () => { ClearRepeated(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { AddRepeated(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { UpdateRepeated(allTypes, 0, 1); });

			// maps
			AssertEventsStableWithSnapshot(allTypes, () => { AddMap(allTypes, 1); });
			AssertEventsStableWithSnapshot(allTypes, () => { RemoveMap(allTypes, 1); });
			AssertEventsStableWithSnapshot(allTypes, () => { AddMap(allTypes, 1); });
			AssertEventsStableWithSnapshot(allTypes, () => { ReplaceMap(allTypes, 1); }, false);
			AssertEventsStableWithSnapshot(allTypes, () => { ReplaceMap(allTypes, 1, 1); });
			AssertEventsStableWithSnapshot(allTypes, () => { ClearMap(allTypes); });
			AssertEventsStableWithSnapshot(allTypes, () => { AddMap(allTypes, 1); });
			AssertEventsStableWithSnapshot(allTypes, () => { UpdateMap(allTypes, 1, 1); });

			// oneof
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofUint32 = UintValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofNestedMessage = NestedMessageValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofString = StringValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofBytes = ByteValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofForeignMessage = ForeignMessageValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofForeignMessageNoEvents = ForeignMessageNoEventsValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofAllTypes = TestAllTypesValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.OneofAllTypesNoEvents = TestAllTypesNoEventsValue(0); });

			// nested types
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.AllTypes = TestAllTypesValue(0); });
			AssertEventsStableWithSnapshot(allTypes, () => { allTypes.AllTypesNoEvents = TestAllTypesNoEventsValue(0); });
		}

		[Fact]
		public void ShouldBeAbleToMakeChangesToNestedTypeInNonEventType() {
			var allTypesNoEvent = new TestAllTypesNoEvents();
			var allTypes = new TestAllTypes();
			allTypesNoEvent.AllTypes = allTypes;

			ApplyAllChanges(allTypes);
		}

		[Fact]
		public void ShouldBeAbleToSetAllNonEventFields() {
			var allTypesNoEvent = new TestAllTypesNoEvents();
			ApplyAllChanges(allTypesNoEvent);
		}

		[Fact]
		public void NonEventChildMessageShouldNotGenerateEvents() {
			var allTypes = new TestAllTypes();
			allTypes.AllTypesNoEvents = new TestAllTypesNoEvents();
			AssertGenerated(allTypes);
			allTypes.ClearEvents();

			ApplyAllChanges(allTypes.AllTypesNoEvents);
			AssertNotGenerated(allTypes);
		}

		private void TestManyChangesWithClone(TestAllTypes root, TestAllTypes allTypes) {
			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { SetSingular(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { SetEnums(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { SetNestedMessages(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { UpdateNestedMessages(allTypes, i1); });

				AssertEventsStableWithClone(root, () => { ClearRepeated(allTypes); });
				AssertEventsStableWithClone(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { RemoveRepeated(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { RemoveAtRepeated(allTypes, 0); });
				AssertEventsStableWithClone(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { ReplaceRepeated(allTypes, 0, i1); }, false);
				AssertEventsStableWithClone(root, () => { ReplaceRepeated(allTypes, 0, i1 + 1); });
				AssertEventsStableWithClone(root, () => { InsertRepeated(allTypes, 0, i1 + 2); });
				AssertEventsStableWithClone(root, () => { ClearRepeated(allTypes); });
				AssertEventsStableWithClone(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { UpdateRepeated(allTypes, 0, i1 + 1); });

				AssertEventsStableWithClone(root, () => { ClearMap(allTypes); });
				AssertEventsStableWithClone(root, () => { AddMap(allTypes, i1, i1); });
				AssertEventsStableWithClone(root, () => { RemoveMap(allTypes, i1); });
				AssertEventsStableWithClone(root, () => { AddMap(allTypes, i1, i1); });
				AssertEventsStableWithClone(root, () => { ReplaceMap(allTypes, i1, i1); }, false);
				AssertEventsStableWithClone(root, () => { ReplaceMap(allTypes, i1, i1 + 1); });
				AssertEventsStableWithClone(root, () => { ClearMap(allTypes); });
				AssertEventsStableWithClone(root, () => { AddMap(allTypes, i1, i1); });
				AssertEventsStableWithClone(root, () => { UpdateMap(allTypes, i1, i1 + 1); });

				AssertEventsStableWithClone(root, () => { allTypes.OneofUint32 = UintValue(i1); });
				AssertEventsStableWithClone(root, () => { allTypes.OneofNestedMessage = NestedMessageValue(i1); });
				AssertEventsStableWithClone(root, () => { allTypes.OneofString = StringValue(i1); });
				AssertEventsStableWithClone(root, () => { allTypes.OneofBytes = ByteValue(i1); });
				AssertEventsStableWithClone(root, () => { allTypes.OneofForeignMessage = ForeignMessageValue(i1); });
				AssertEventsStableWithClone(root, () => { allTypes.OneofForeignMessageNoEvents = ForeignMessageNoEventsValue(i1); });
				AssertEventsStableWithClone(root, () => { allTypes.OneofAllTypes = TestAllTypesValue(i1); });
				AssertEventsStableWithClone(root, () => { allTypes.OneofAllTypesNoEvents = TestAllTypesNoEventsValue(i1); });

				AssertEventsStableWithClone(root, () => { allTypes.AllTypes = TestAllTypesValue(i1 * 2); });
				AssertEventsStableWithClone(root, () => { SetSingular(allTypes.AllTypes, i1 * 2 + 1); });
				AssertEventsStableWithClone(root, () => { allTypes.AllTypesNoEvents = TestAllTypesNoEventsValue(i1); });
			}

			// list only changes
			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { AddRepeated(allTypes, i1); });
			}

			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { UpdateRepeated(allTypes, i1, i1 + 1); });
			}

			// map only changes
			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { ReplaceMap(allTypes, i1, i1); });
			}

			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { UpdateMap(allTypes, i1, i1 + 1); });
			}
		}

		private void TestManyChangesWithSnapshot(TestAllTypes root, TestAllTypes allTypes) {
			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithSnapshot(root, () => { SetSingular(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { SetEnums(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { SetNestedMessages(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { UpdateNestedMessages(allTypes, i1); });

				AssertEventsStableWithSnapshot(root, () => { ClearRepeated(allTypes); });
				AssertEventsStableWithSnapshot(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { RemoveRepeated(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { RemoveAtRepeated(allTypes, 0); });
				AssertEventsStableWithSnapshot(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { ReplaceRepeated(allTypes, 0, i1); }, false);
				AssertEventsStableWithSnapshot(root, () => { ReplaceRepeated(allTypes, 0, i1 + 1); });
				AssertEventsStableWithSnapshot(root, () => { InsertRepeated(allTypes, 0, i1 + 2); });
				AssertEventsStableWithSnapshot(root, () => { ClearRepeated(allTypes); });
				AssertEventsStableWithSnapshot(root, () => { AddRepeated(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { UpdateRepeated(allTypes, 0, i1 + 1); });

				AssertEventsStableWithSnapshot(root, () => { ClearMap(allTypes); });
				AssertEventsStableWithSnapshot(root, () => { AddMap(allTypes, i1, i1); });
				AssertEventsStableWithSnapshot(root, () => { RemoveMap(allTypes, i1); });
				AssertEventsStableWithSnapshot(root, () => { AddMap(allTypes, i1, i1); });
				AssertEventsStableWithSnapshot(root, () => { ReplaceMap(allTypes, i1, i1); }, false);
				AssertEventsStableWithSnapshot(root, () => { ReplaceMap(allTypes, i1, i1 + 1); });
				AssertEventsStableWithSnapshot(root, () => { ClearMap(allTypes); });
				AssertEventsStableWithSnapshot(root, () => { AddMap(allTypes, i1, i1); });
				AssertEventsStableWithSnapshot(root, () => { UpdateMap(allTypes, i1, i1 + 1); });

				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofUint32 = UintValue(i1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofNestedMessage = NestedMessageValue(i1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofString = StringValue(i1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofBytes = ByteValue(i1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofForeignMessage = ForeignMessageValue(i1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofForeignMessageNoEvents = ForeignMessageNoEventsValue(i1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofAllTypes = TestAllTypesValue(i1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.OneofAllTypesNoEvents = TestAllTypesNoEventsValue(i1); });

				AssertEventsStableWithSnapshot(root, () => { allTypes.AllTypes = TestAllTypesValue(i1 * 2); });
				AssertEventsStableWithSnapshot(root, () => { SetSingular(allTypes.AllTypes, i1 * 2 + 1); });
				AssertEventsStableWithSnapshot(root, () => { allTypes.AllTypesNoEvents = TestAllTypesNoEventsValue(i1); });
			}

			// list only changes
			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { AddRepeated(allTypes, i1); });
			}

			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { UpdateRepeated(allTypes, i1, i1 + 1); });
			}

			// map only changes
			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { ReplaceMap(allTypes, i1, i1); });
			}

			for (int i = 0; i < 10; i++) {
				var i1 = i;
				AssertEventsStableWithClone(root, () => { UpdateMap(allTypes, i1, i1 + 1); });
			}

			// just some basic sanity that these methods don't throw exceptions
			Assert.True(root.CalculateSize() > 0);
			Assert.True(root.GetHashCode() != 1);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsWithClone() {
			var allTypes = new TestAllTypes();
			TestManyChangesWithClone(allTypes, allTypes);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsWithSnapshot() {
			var allTypes = new TestAllTypes();
			TestManyChangesWithSnapshot(allTypes, allTypes);
		}

		private static TestAllTypes DeeplyNested() {
			var allTypes = new TestAllTypes();
			allTypes.AllTypes = new TestAllTypes();
			allTypes.AllTypes.AllTypes = new TestAllTypes();
			allTypes.AllTypes.AllTypes.AllTypes = new TestAllTypes();
			return allTypes;
		}

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedWithClone() {
			var allTypes = DeeplyNested();
			TestManyChangesWithClone(allTypes, allTypes.AllTypes.AllTypes.AllTypes);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedWithSnapshot() {
			var allTypes = DeeplyNested();
			TestManyChangesWithSnapshot(allTypes, allTypes.AllTypes.AllTypes.AllTypes);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedMapWithClone() {
			var allTypes = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1] = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[2] = DeeplyNested();
			TestManyChangesWithClone(allTypes, allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[2].AllTypes.AllTypes.AllTypes);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedMapWithSnapshot() {
			var allTypes = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1] = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[2] = DeeplyNested();
			TestManyChangesWithSnapshot(allTypes, allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[2].AllTypes.AllTypes.AllTypes);
		}


		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedListWithClone() {
			var allTypes = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			allTypes.AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			TestManyChangesWithClone(allTypes, allTypes.AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedListWithSnapshot() {
			var allTypes = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			allTypes.AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			TestManyChangesWithSnapshot(allTypes, allTypes.AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedMapListWithClone() {
			var allTypes = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1] = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			TestManyChangesWithClone(allTypes, allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes);
		}

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedMapListWithSnapshot() {
			var allTypes = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1] = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			TestManyChangesWithSnapshot(allTypes, allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes);
		}

	}
}