using System.Collections.Generic;
using Google.Protobuf.TestProtos;
using Xunit;
using static Zynga.Protobuf.Runtime.Tests.Simple.EventTestHelper;
using static Zynga.Protobuf.Runtime.Tests.Simple.AllTypesTestsHelper;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public class AllTypesTests {
		[Fact]
		public void ShouldGenerateValidStableEventsWithClone() {
			var allTypes = new TestAllTypes();
			AssertEventsStableWithClone(allTypes, () => { ApplyAllChanges(allTypes); });
		}

		[Fact]
		public void ShouldGenerateValidStableEventsToExistingWithClone() {
			var allTypes = new TestAllTypes();
			ApplyAllChanges(allTypes);

			var newAllTypes = allTypes.Clone();
			Assert.Equal(allTypes, newAllTypes);
			AssertEventsStableWithClone(newAllTypes, () => { ApplyAllChanges(newAllTypes, 1); });
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
		public void ShouldGenerateValidStableEventsToExistingWithSnapshot() {
			var allTypes = new TestAllTypes();
			ApplyAllChanges(allTypes);
			var snapshot = allTypes.GenerateSnapshot();

			var newAllTypes = new TestAllTypes();
			newAllTypes.ApplyEvents(snapshot);
			Assert.Equal(allTypes, newAllTypes);
			AssertEventsStableWithClone(newAllTypes, () => { ApplyAllChanges(newAllTypes, 1); });
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

		[Fact]
		public void ShouldGenerateManyValidEventsDeeplyNestedMapListToExistingWithSnapshot() {
			var allTypes = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1] = DeeplyNested();
			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			TestManyChangesWithSnapshot(allTypes, allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[0].AllTypes.AllTypes.AllTypes);

			var snapshot = allTypes.GenerateSnapshot();
			var newAllTypes = new TestAllTypes();
			newAllTypes.ApplyEvents(snapshot);
			Assert.Equal(allTypes, newAllTypes);

			allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			TestManyChangesWithSnapshot(allTypes, allTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[1].AllTypes.AllTypes.AllTypes);
		}

		[Fact]
		public void ShouldGenerateOnChangeEvents() {
			var allTypes = new TestAllTypes();
			ApplyAllChanges(allTypes);
			var snapshot = allTypes.GenerateSnapshot();

			var target = new TestAllTypes();
			target.ApplyEvents(snapshot);

			bool foreignChanged = false;
			bool nestedChanged = false;
			bool importChanged = false;
			bool publicImportChanged = false;

			target.SingleForeignMessage.OnChanged += f => { foreignChanged = true; };
			target.SingleNestedMessage.OnChanged += f => { nestedChanged = true; };
			target.SingleImportMessage.OnChanged += f => { importChanged = true; };
			target.SinglePublicImportMessage.OnChanged += f => { publicImportChanged = true; };

			allTypes.SingleForeignMessage.C = 100;
			var events = allTypes.GenerateEvents();

			target.ApplyEvents(events);

			Assert.True(foreignChanged);
			Assert.False(nestedChanged);
			Assert.False(importChanged);
			Assert.False(publicImportChanged);

			foreignChanged = false;
			nestedChanged = false;
			importChanged = false;
			publicImportChanged = false;

			allTypes.SingleNestedMessage.Bb = 100;
			events = allTypes.GenerateEvents();
			target.ApplyEvents(events);

			Assert.False(foreignChanged);
			Assert.True(nestedChanged);
			Assert.False(importChanged);
			Assert.False(publicImportChanged);

			bool repeatedBoolChanged = false;
			bool mapStringChanged = false;
			foreignChanged = false;
			nestedChanged = false;
			importChanged = false;
			publicImportChanged = false;

			allTypes.RepeatedBool.Add(false);
			allTypes.MapStringString["foo"] = "bar";
			events = allTypes.GenerateEvents();
			target.RepeatedBool.OnChanged += (f, changes) => {
				repeatedBoolChanged = true;
				Assert.Equal(1, changes.Count);
				Assert.Equal(false, changes[0].Value);
				Assert.Equal(1, changes[0].Index);  // initial RepeatedBool already has a single element
			};
			target.MapStringString.OnChanged += (f, changes) => {
				mapStringChanged = true;
				Assert.Equal(1, changes.Count);
			};

			target.ApplyEvents(events);

			Assert.False(foreignChanged);
			Assert.False(nestedChanged);
			Assert.False(importChanged);
			Assert.False(publicImportChanged);
			Assert.True(repeatedBoolChanged);
			Assert.True(mapStringChanged);
		}

		[Fact]
		public void ShouldGenerateOnChangeEventsDeeplyNested() {
			var root = DeeplyNested();
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1] = DeeplyNested();
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[3] = DeeplyNested();
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());

			var allTypes = root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[1].AllTypes.AllTypes.AllTypes;
			ApplyAllChanges(allTypes);
			var snapshot = root.GenerateSnapshot();

			var targetAllTypes = new TestAllTypes();
			targetAllTypes.ApplyEvents(snapshot);
			var target = targetAllTypes.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[1].AllTypes.AllTypes.AllTypes;

			bool foreignChanged = false;
			bool nestedChanged = false;
			bool importChanged = false;
			bool publicImportChanged = false;

			target.SingleForeignMessage.OnChanged += f => { foreignChanged = true; };
			target.SingleNestedMessage.OnChanged += f => { nestedChanged = true; };
			target.SingleImportMessage.OnChanged += f => { importChanged = true; };
			target.SinglePublicImportMessage.OnChanged += f => { publicImportChanged = true; };

			allTypes.SingleForeignMessage.C = 100;
			var events = root.GenerateEvents();
			targetAllTypes.ApplyEvents(events);

			Assert.True(foreignChanged);
			Assert.False(nestedChanged);
			Assert.False(importChanged);
			Assert.False(publicImportChanged);

			foreignChanged = false;
			nestedChanged = false;
			importChanged = false;
			publicImportChanged = false;

			allTypes.SingleNestedMessage.Bb = 100;
			events = root.GenerateEvents();
			targetAllTypes.ApplyEvents(events);

			Assert.False(foreignChanged);
			Assert.True(nestedChanged);
			Assert.False(importChanged);
			Assert.False(publicImportChanged);

			bool repeatedBoolChanged = false;
			bool mapStringChanged = false;
			foreignChanged = false;
			nestedChanged = false;
			importChanged = false;
			publicImportChanged = false;

			allTypes.RepeatedBool.Add(false);
			allTypes.MapStringString["foo"] = "bar";
			events = root.GenerateEvents();
			target.RepeatedBool.OnChanged += (f, changes) => {
				repeatedBoolChanged = true;
				Assert.Equal(1, changes.Count);
				Assert.Equal(false, changes[0].Value);
				Assert.Equal(1, changes[0].Index);  // initial RepeatedBool already has a single element
			};
			target.MapStringString.OnChanged += (f, changes) => {
				mapStringChanged = true;
				Assert.Equal(1, changes.Count);
			};

			targetAllTypes.ApplyEvents(events);

			Assert.False(foreignChanged);
			Assert.False(nestedChanged);
			Assert.False(importChanged);
			Assert.False(publicImportChanged);
			Assert.True(repeatedBoolChanged);
			Assert.True(mapStringChanged);
		}

		[Fact]
		public void ShouldGenerateOnChangeListEvents() {
			var allTypes = new TestAllTypes();
			ApplyAllChanges(allTypes);
			var snapshot = allTypes.GenerateSnapshot();

			var target = new TestAllTypes();
			target.ApplyEvents(snapshot);

			// verify clearing generates an event, but doesn't generate any add/remove events
			allTypes.RepeatedString.Clear();
			var events = allTypes.GenerateEvents();
			_validateListClearCalled = false;
			target.RepeatedString.OnChanged += ValidateListClear;
			target.ApplyEvents(events);
			target.RepeatedString.OnChanged -= ValidateListClear;
			Assert.True(_validateListClearCalled);

			// verify add, remove, remove at, and insert generate valid events
			allTypes.RepeatedString.Add("a"); // 1 event
			allTypes.RepeatedString.Add("b"); // 1 event
			allTypes.RepeatedString[0] = "c"; // 2 events
			allTypes.RepeatedString.RemoveAt(0); // 1 event   (3 removes on these two lines)
			allTypes.RepeatedString.Remove("b"); // 1 event
			// should be empty
			allTypes.RepeatedString.Add("e");  // 1 event
			allTypes.RepeatedString.Insert(0, "f"); // 1 event

			events = allTypes.GenerateEvents();
			bool onChangeCalled = false;
			target.RepeatedString.OnChanged += (list, changes) => {
				onChangeCalled = true;
				Assert.Equal(2, list.Count);
				Assert.Equal(8, changes.Count);
				Assert.Equal(0, changes[0].Index);
				Assert.Equal(1, changes[1].Index);
				Assert.Equal(0, changes[2].Index);
				Assert.Equal(0, changes[3].Index);
				Assert.Equal(0, changes[4].Index);
				Assert.Equal(0, changes[5].Index);
				Assert.Equal(0, changes[6].Index);
				Assert.Equal(0, changes[7].Index);
				Assert.Equal("a", changes[0].Value);
				Assert.Equal("b", changes[1].Value);
				Assert.Equal("a", changes[2].Value);
				Assert.Equal("c", changes[3].Value);
				Assert.Equal("c", changes[4].Value);
				Assert.Equal("b", changes[5].Value);
				Assert.Equal("e", changes[6].Value);
				Assert.Equal("f", changes[7].Value);
			};

			target.ApplyEvents(events);

			Assert.True(onChangeCalled);
		}

		private bool _validateListClearCalled;
		private void ValidateListClear(EventRepeatedField<string> list, IReadOnlyList<EventListChange<string>> changes) {
			_validateListClearCalled = true;
			Assert.Equal(0, list.Count);
			Assert.Equal(0, changes.Count);
		}

		[Fact]
		public void ShouldGenerateOnChangeMapEvents() {
			var allTypes = new TestAllTypes();
			ApplyAllChanges(allTypes);
			var snapshot = allTypes.GenerateSnapshot();

			var target = new TestAllTypes();
			target.ApplyEvents(snapshot);

			// verify clearing generates an event, but doesn't generate any add/remove events
			allTypes.MapStringString.Clear();
			var events = allTypes.GenerateEvents();
			_validateListClearCalled = false;
			target.MapStringString.OnChanged += ValidateMapClear;
			target.ApplyEvents(events);
			target.MapStringString.OnChanged -= ValidateMapClear;
			Assert.True(_validateMapClearCalled);

			// verify add, remove, remove at, and insert generate valid events
			allTypes.MapStringString.Add("a", "b"); // 1 event
			allTypes.MapStringString.Add("c", "d"); // 1 event
			allTypes.MapStringString["a"] = "e"; // 2 events
			allTypes.MapStringString.Remove("a"); // 1 event
			allTypes.MapStringString.Remove("c"); // 1 event
			allTypes.MapStringString.Remove("d"); // 0 events

			events = allTypes.GenerateEvents();
			bool onChangeCalled = false;
			target.MapStringString.OnChanged += (map, changes) => {
				onChangeCalled = true;
				Assert.Equal(0, map.Count);
				Assert.Equal(6, changes.Count);
				Assert.Equal("a", changes[0].Key);
				Assert.Equal("b", changes[0].Value);
				Assert.Equal("c", changes[1].Key);
				Assert.Equal("d", changes[1].Value);
				Assert.Equal("a", changes[2].Key);
				Assert.Equal("b", changes[2].Value);
				Assert.Equal("a", changes[3].Key);
				Assert.Equal("e", changes[3].Value);
				Assert.Equal("a", changes[4].Key);
				Assert.Equal("e", changes[4].Value);
				Assert.Equal("c", changes[5].Key);
				Assert.Equal("d", changes[5].Value);
			};

			target.ApplyEvents(events);

			Assert.True(onChangeCalled);
		}

		private bool _validateMapClearCalled;
		private void ValidateMapClear(EventMapField<string, string> map, IReadOnlyList<EventMapChange<string, string>> changes) {
			_validateMapClearCalled = true;
			Assert.Equal(0, map.Count);
			Assert.Equal(0, changes.Count);
		}

		[Fact]
		public void ShouldNotGenerateEventsWhenApplyingSnapshot() {
			var allTypes = new TestAllTypes();
			ApplyAllChanges(allTypes);
			var snapshot = allTypes.GenerateSnapshot();

			var target = new TestAllTypes();
			target.ApplyEvents(snapshot);

			Assert.False(target.HasEvents);
		}

		[Fact]
		public void ShouldGenerateEventsInSnapshotBasedInstance() {
			var allTypes = new TestAllTypes();
			ApplyAllChanges(allTypes);
			var snapshot = allTypes.GenerateSnapshot();

			var target = new TestAllTypes();
			target.ApplyEvents(snapshot);

			UpdateNestedMessages(target, 2);
			UpdateNestedMessages(target.OneofAllTypes, 5);
			UpdateNestedMessages(target.AllTypes, 6);

			// verify events are stable
			var targetEvents = target.GenerateEvents();
			var target2 = new TestAllTypes();
			target2.ApplyEvents(snapshot);
			target2.ApplyEvents(targetEvents);
			Assert.Equal(target, target2);
			Assert.NotEqual(allTypes, target2);

			// verify snapshot is stable
			var targetSnapshot = target.GenerateSnapshot();
			var target3 = new TestAllTypes();
			target3.ApplyEvents(targetSnapshot);
			Assert.Equal(target, target3);
			Assert.NotEqual(allTypes, target3);
		}

		[Fact]
		public void ShouldGenerateEventsInDeeplyNestedSnapshotBasedInstance() {
			var root = DeeplyNested();
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1] = DeeplyNested();
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[3] = DeeplyNested();
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());
			root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage.Add(DeeplyNested());

			var allTypes = root.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[1].AllTypes.AllTypes.AllTypes;
			ApplyAllChanges(allTypes);

			var snapshot = root.GenerateSnapshot();

			var target = new TestAllTypes();
			target.ApplyEvents(snapshot);

			var nested = target.AllTypes.AllTypes.AllTypes.MapInt32TestAllTypesMessage[1].AllTypes.AllTypes.AllTypes.RepeatedTestAllTypesMessage[1].AllTypes.AllTypes.AllTypes;

			UpdateNestedMessages(nested, 2);
			UpdateNestedMessages(nested.OneofAllTypes, 5);
			UpdateNestedMessages(nested.AllTypes, 6);

			// verify events are stable
			var targetEvents = target.GenerateEvents();
			var target2 = new TestAllTypes();
			target2.ApplyEvents(snapshot);
			target2.ApplyEvents(targetEvents);
			Assert.Equal(target, target2);
			Assert.NotEqual(allTypes, target2);

			// verify snapshot is stable
			var targetSnapshot = target.GenerateSnapshot();
			var target3 = new TestAllTypes();
			target3.ApplyEvents(targetSnapshot);
			Assert.Equal(target, target3);
			Assert.NotEqual(allTypes, target3);
		}
	}
}