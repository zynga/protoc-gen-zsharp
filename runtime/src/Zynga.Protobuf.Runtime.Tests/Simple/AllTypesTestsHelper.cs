using Google.Protobuf;
using Google.Protobuf.TestProtos;

namespace Zynga.Protobuf.Runtime.Tests.Simple {
	public static class AllTypesTestsHelper {
		public const int OffsetBy = 20;

		public static int IntValue(int offsetIndex) {
			return 1 + offsetIndex * OffsetBy;
		}

		public static uint UintValue(int offsetIndex) {
			return (uint) (1 + offsetIndex * OffsetBy);
		}

		public static float FloatValue(int offsetIndex) {
			return (float) (1 + 0.1 + offsetIndex * OffsetBy);
		}

		public static bool BoolValue(int offsetIndex) {
			return offsetIndex % 2 == 0;
		}

		public static string StringValue(int offsetIndex) {
			return "string-" + 14 + (1 + offsetIndex * OffsetBy);
		}

		public static ByteString ByteValue(int offsetIndex) {
			return ByteString.CopyFromUtf8("string-" + 1 + offsetIndex * OffsetBy);
		}

		public static TestAllTypes.Types.NestedEnum NestedEnumValue(int offsetIndex) {
			return (TestAllTypes.Types.NestedEnum) 1 + offsetIndex % 3;
		}

		public static TestAllTypesNoEvents.Types.NestedEnumNoEvents NestedEnumNoEventsValue(int offsetIndex) {
			return (TestAllTypesNoEvents.Types.NestedEnumNoEvents) 1 + offsetIndex % 3;
		}

		public static ForeignEnum ForeignEnumValue(int offsetIndex) {
			return (ForeignEnum) 4 + offsetIndex % 3;
		}

		public static ImportEnum ImportEnumValue(int offsetIndex) {
			return (ImportEnum) 7 + offsetIndex % 3;
		}

		public static MapEnum MapEnumValue(int offsetIndex) {
			return (MapEnum) 0 + offsetIndex % 3;
		}

		public static TestAllTypes.Types.NestedMessage NestedMessageValue(int offsetIndex) {
			return new TestAllTypes.Types.NestedMessage {Bb = IntValue(offsetIndex)};
		}

		public static TestAllTypesNoEvents.Types.NestedMessageNoEvents NestedMessageNoEventsValue(int offsetIndex) {
			return new TestAllTypesNoEvents.Types.NestedMessageNoEvents {Bb = IntValue(offsetIndex)};
		}

		public static ForeignMessage ForeignMessageValue(int offsetIndex) {
			return new ForeignMessage {C = IntValue(offsetIndex)};
		}

		public static ForeignMessageNoEvents ForeignMessageNoEventsValue(int offsetIndex) {
			return new ForeignMessageNoEvents {C = IntValue(offsetIndex)};
		}

		public static ImportMessage ImportMessageValue(int offsetIndex) {
			return new ImportMessage {D = IntValue(offsetIndex)};
		}

		public static ImportMessageNoEvents ImportMessageNoEventsValue(int offsetIndex) {
			return new ImportMessageNoEvents {D = IntValue(offsetIndex)};
		}

		public static PublicImportMessage PublicImportMessageValue(int offsetIndex) {
			return new PublicImportMessage {E = IntValue(offsetIndex)};
		}

		public static PublicImportMessageNoEvents PublicImportMessageNoEventsValue(int offsetIndex) {
			return new PublicImportMessageNoEvents {E = IntValue(offsetIndex)};
		}

		public static TestAllTypes TestAllTypesValue(int offsetIndex) {
			var message = new TestAllTypes();
			SetSingular(message, offsetIndex);
			return message;
		}

		public static TestAllTypesNoEvents TestAllTypesNoEventsValue(int offsetIndex) {
			var message = new TestAllTypesNoEvents();
			SetSingular(message, offsetIndex);
			return message;
		}

		public static void SetSingular(TestAllTypes allTypes, int offsetIndex = 0) {
			allTypes.SingleInt32 = IntValue(offsetIndex);
			allTypes.SingleInt64 = IntValue(offsetIndex);
			allTypes.SingleUint32 = UintValue(offsetIndex);
			allTypes.SingleUint64 = UintValue(offsetIndex);
			allTypes.SingleSint32 = IntValue(offsetIndex);
			allTypes.SingleSint64 = IntValue(offsetIndex);
			allTypes.SingleFixed32 = UintValue(offsetIndex);
			allTypes.SingleFixed64 = UintValue(offsetIndex);
			allTypes.SingleSfixed32 = IntValue(offsetIndex);
			allTypes.SingleSfixed64 = IntValue(offsetIndex);
			allTypes.SingleFloat = FloatValue(offsetIndex);
			allTypes.SingleDouble = FloatValue(offsetIndex);
			allTypes.SingleBool = BoolValue(offsetIndex);
			allTypes.SingleString = StringValue(offsetIndex);
			allTypes.SingleBytes = ByteValue(offsetIndex);
		}

		public static void SetSingular(TestAllTypesNoEvents allTypes, int offsetIndex = 0) {
			allTypes.SingleInt32 = IntValue(offsetIndex);
			allTypes.SingleInt64 = IntValue(offsetIndex);
			allTypes.SingleUint32 = UintValue(offsetIndex);
			allTypes.SingleUint64 = UintValue(offsetIndex);
			allTypes.SingleSint32 = IntValue(offsetIndex);
			allTypes.SingleSint64 = IntValue(offsetIndex);
			allTypes.SingleFixed32 = UintValue(offsetIndex);
			allTypes.SingleFixed64 = UintValue(offsetIndex);
			allTypes.SingleSfixed32 = IntValue(offsetIndex);
			allTypes.SingleSfixed64 = IntValue(offsetIndex);
			allTypes.SingleFloat = FloatValue(offsetIndex);
			allTypes.SingleDouble = FloatValue(offsetIndex);
			allTypes.SingleBool = BoolValue(offsetIndex);
			allTypes.SingleString = StringValue(offsetIndex);
			allTypes.SingleBytes = ByteValue(offsetIndex);
		}

		public static void SetEnums(TestAllTypes allTypes, int offsetIndex = 0) {
			allTypes.SingleNestedEnum = NestedEnumValue(offsetIndex);
			allTypes.SingleForeignEnum = ForeignEnumValue(offsetIndex);
			allTypes.SingleImportEnum = ImportEnumValue(offsetIndex);
		}

		public static void SetEnums(TestAllTypesNoEvents allTypes, int offsetIndex = 0) {
			allTypes.SingleNestedEnum = NestedEnumNoEventsValue(offsetIndex);
			allTypes.SingleForeignEnum = ForeignEnumValue(offsetIndex);
			allTypes.SingleImportEnum = ImportEnumValue(offsetIndex);
		}

		public static void SetNestedMessages(TestAllTypes allTypes, int offsetIndex = 0) {
			allTypes.SingleNestedMessage = NestedMessageValue(offsetIndex);
			allTypes.SingleForeignMessage = ForeignMessageValue(offsetIndex);
			allTypes.SingleImportMessage = ImportMessageValue(offsetIndex);
			allTypes.SinglePublicImportMessage = PublicImportMessageValue(offsetIndex);
		}

		public static void SetNestedMessages(TestAllTypesNoEvents allTypes, int offsetIndex = 0) {
			allTypes.SingleNestedMessage = NestedMessageNoEventsValue(offsetIndex);
			allTypes.SingleForeignMessage = ForeignMessageNoEventsValue(offsetIndex);
			allTypes.SingleImportMessage = ImportMessageNoEventsValue(offsetIndex);
			allTypes.SinglePublicImportMessage = PublicImportMessageNoEventsValue(offsetIndex);
		}

		public static void UpdateNestedMessages(TestAllTypes allTypes, int offsetIndex = 0) {
			int offset = offsetIndex * 20;
			allTypes.SingleNestedMessage.Bb = 1 + offset;
			allTypes.SingleForeignMessage.C = 2 + offset;
			allTypes.SingleImportMessage.D = 3 + offset;
			allTypes.SinglePublicImportMessage.E = 4 + offset;
		}

		public static void UpdateNestedMessages(TestAllTypesNoEvents allTypes, int offsetIndex = 0) {
			int offset = offsetIndex * 20;
			allTypes.SingleNestedMessage.Bb = 1 + offset;
			allTypes.SingleForeignMessage.C = 2 + offset;
			allTypes.SingleImportMessage.D = 3 + offset;
			allTypes.SinglePublicImportMessage.E = 4 + offset;
		}

		public static void AddRepeated(TestAllTypes allTypes, int offsetIndex = 0) {
			allTypes.RepeatedInt32.Add(IntValue(offsetIndex));
			allTypes.RepeatedInt64.Add(IntValue(offsetIndex));
			allTypes.RepeatedUint32.Add(UintValue(offsetIndex));
			allTypes.RepeatedUint64.Add(UintValue(offsetIndex));
			allTypes.RepeatedSint32.Add(IntValue(offsetIndex));
			allTypes.RepeatedSint64.Add(IntValue(offsetIndex));
			allTypes.RepeatedFixed32.Add(UintValue(offsetIndex));
			allTypes.RepeatedFixed64.Add(UintValue(offsetIndex));
			allTypes.RepeatedSfixed32.Add(IntValue(offsetIndex));
			allTypes.RepeatedSfixed64.Add(IntValue(offsetIndex));
			allTypes.RepeatedFloat.Add(FloatValue(offsetIndex));
			allTypes.RepeatedDouble.Add(FloatValue(offsetIndex));
			allTypes.RepeatedBool.Add(BoolValue(offsetIndex));
			allTypes.RepeatedString.Add(StringValue(offsetIndex));
			allTypes.RepeatedBytes.Add(ByteValue(offsetIndex));
			allTypes.RepeatedNestedMessage.Add(NestedMessageValue(offsetIndex));
			allTypes.RepeatedForeignMessage.Add(ForeignMessageValue(offsetIndex));
			allTypes.RepeatedImportMessage.Add(ImportMessageValue(offsetIndex));
			allTypes.RepeatedImportNoEvents.Add(ImportMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedTestAllTypesMessage.Add(TestAllTypesValue(offsetIndex));
			allTypes.RepeatedTestAllTypesNoEventsMessage.Add(TestAllTypesNoEventsValue(offsetIndex));
			allTypes.RepeatedNestedEnum.Add(NestedEnumValue(offsetIndex));
			allTypes.RepeatedForeignEnum.Add(ForeignEnumValue(offsetIndex));
			allTypes.RepeatedImportEnum.Add(ImportEnumValue(offsetIndex));
		}

		public static void AddRepeated(TestAllTypesNoEvents allTypes, int offsetIndex = 0) {
			allTypes.RepeatedInt32.Add(IntValue(offsetIndex));
			allTypes.RepeatedInt64.Add(IntValue(offsetIndex));
			allTypes.RepeatedUint32.Add(UintValue(offsetIndex));
			allTypes.RepeatedUint64.Add(UintValue(offsetIndex));
			allTypes.RepeatedSint32.Add(IntValue(offsetIndex));
			allTypes.RepeatedSint64.Add(IntValue(offsetIndex));
			allTypes.RepeatedFixed32.Add(UintValue(offsetIndex));
			allTypes.RepeatedFixed64.Add(UintValue(offsetIndex));
			allTypes.RepeatedSfixed32.Add(IntValue(offsetIndex));
			allTypes.RepeatedSfixed64.Add(IntValue(offsetIndex));
			allTypes.RepeatedFloat.Add(FloatValue(offsetIndex));
			allTypes.RepeatedDouble.Add(FloatValue(offsetIndex));
			allTypes.RepeatedBool.Add(BoolValue(offsetIndex));
			allTypes.RepeatedString.Add(StringValue(offsetIndex));
			allTypes.RepeatedBytes.Add(ByteValue(offsetIndex));
			allTypes.RepeatedNestedMessage.Add(NestedMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedForeignMessage.Add(ForeignMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedImportMessage.Add(ImportMessageValue(offsetIndex));
			allTypes.RepeatedImportNoEvents.Add(ImportMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedTestAllTypesMessage.Add(TestAllTypesValue(offsetIndex));
			allTypes.RepeatedTestAllTypesNoEventsMessage.Add(TestAllTypesNoEventsValue(offsetIndex));
			allTypes.RepeatedNestedEnum.Add(NestedEnumNoEventsValue(offsetIndex));
			allTypes.RepeatedForeignEnum.Add(ForeignEnumValue(offsetIndex));
			allTypes.RepeatedImportEnum.Add(ImportEnumValue(offsetIndex));
		}

		public static void RemoveRepeated(TestAllTypes allTypes, int offsetIndex = 0) {
			allTypes.RepeatedInt32.Remove(IntValue(offsetIndex));
			allTypes.RepeatedInt64.Remove(IntValue(offsetIndex));
			allTypes.RepeatedUint32.Remove(UintValue(offsetIndex));
			allTypes.RepeatedUint64.Remove(UintValue(offsetIndex));
			allTypes.RepeatedSint32.Remove(IntValue(offsetIndex));
			allTypes.RepeatedSint64.Remove(IntValue(offsetIndex));
			allTypes.RepeatedFixed32.Remove(UintValue(offsetIndex));
			allTypes.RepeatedFixed64.Remove(UintValue(offsetIndex));
			allTypes.RepeatedSfixed32.Remove(IntValue(offsetIndex));
			allTypes.RepeatedSfixed64.Remove(IntValue(offsetIndex));
			allTypes.RepeatedFloat.Remove(FloatValue(offsetIndex));
			allTypes.RepeatedDouble.Remove(FloatValue(offsetIndex));
			allTypes.RepeatedBool.Remove(BoolValue(offsetIndex));
			allTypes.RepeatedString.Remove(StringValue(offsetIndex));
			allTypes.RepeatedBytes.Remove(ByteValue(offsetIndex));
			allTypes.RepeatedNestedMessage.Remove(NestedMessageValue(offsetIndex));
			allTypes.RepeatedForeignMessage.Remove(ForeignMessageValue(offsetIndex));
			allTypes.RepeatedImportMessage.Remove(ImportMessageValue(offsetIndex));
			allTypes.RepeatedImportNoEvents.Remove(ImportMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedTestAllTypesMessage.Remove(TestAllTypesValue(offsetIndex));
			allTypes.RepeatedTestAllTypesNoEventsMessage.Remove(TestAllTypesNoEventsValue(offsetIndex));
			allTypes.RepeatedNestedEnum.Remove(NestedEnumValue(offsetIndex));
			allTypes.RepeatedForeignEnum.Remove(ForeignEnumValue(offsetIndex));
			allTypes.RepeatedImportEnum.Remove(ImportEnumValue(offsetIndex));
		}

		public static void RemoveRepeated(TestAllTypesNoEvents allTypes, int offsetIndex = 0) {
			allTypes.RepeatedInt32.Remove(IntValue(offsetIndex));
			allTypes.RepeatedInt64.Remove(IntValue(offsetIndex));
			allTypes.RepeatedUint32.Remove(UintValue(offsetIndex));
			allTypes.RepeatedUint64.Remove(UintValue(offsetIndex));
			allTypes.RepeatedSint32.Remove(IntValue(offsetIndex));
			allTypes.RepeatedSint64.Remove(IntValue(offsetIndex));
			allTypes.RepeatedFixed32.Remove(UintValue(offsetIndex));
			allTypes.RepeatedFixed64.Remove(UintValue(offsetIndex));
			allTypes.RepeatedSfixed32.Remove(IntValue(offsetIndex));
			allTypes.RepeatedSfixed64.Remove(IntValue(offsetIndex));
			allTypes.RepeatedFloat.Remove(FloatValue(offsetIndex));
			allTypes.RepeatedDouble.Remove(FloatValue(offsetIndex));
			allTypes.RepeatedBool.Remove(BoolValue(offsetIndex));
			allTypes.RepeatedString.Remove(StringValue(offsetIndex));
			allTypes.RepeatedBytes.Remove(ByteValue(offsetIndex));
			allTypes.RepeatedNestedMessage.Remove(NestedMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedForeignMessage.Remove(ForeignMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedImportMessage.Remove(ImportMessageValue(offsetIndex));
			allTypes.RepeatedImportNoEvents.Remove(ImportMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedTestAllTypesMessage.Remove(TestAllTypesValue(offsetIndex));
			allTypes.RepeatedTestAllTypesNoEventsMessage.Remove(TestAllTypesNoEventsValue(offsetIndex));
			allTypes.RepeatedNestedEnum.Remove(NestedEnumNoEventsValue(offsetIndex));
			allTypes.RepeatedForeignEnum.Remove(ForeignEnumValue(offsetIndex));
			allTypes.RepeatedImportEnum.Remove(ImportEnumValue(offsetIndex));
		}

		public static void RemoveAtRepeated(TestAllTypes allTypes, int index) {
			allTypes.RepeatedInt32.RemoveAt(index);
			allTypes.RepeatedInt64.RemoveAt(index);
			allTypes.RepeatedUint32.RemoveAt(index);
			allTypes.RepeatedUint64.RemoveAt(index);
			allTypes.RepeatedSint32.RemoveAt(index);
			allTypes.RepeatedSint64.RemoveAt(index);
			allTypes.RepeatedFixed32.RemoveAt(index);
			allTypes.RepeatedFixed64.RemoveAt(index);
			allTypes.RepeatedSfixed32.RemoveAt(index);
			allTypes.RepeatedSfixed64.RemoveAt(index);
			allTypes.RepeatedFloat.RemoveAt(index);
			allTypes.RepeatedDouble.RemoveAt(index);
			allTypes.RepeatedBool.RemoveAt(index);
			allTypes.RepeatedString.RemoveAt(index);
			allTypes.RepeatedBytes.RemoveAt(index);
			allTypes.RepeatedNestedMessage.RemoveAt(index);
			allTypes.RepeatedForeignMessage.RemoveAt(index);
			allTypes.RepeatedImportMessage.RemoveAt(index);
			allTypes.RepeatedImportNoEvents.RemoveAt(index);
			allTypes.RepeatedTestAllTypesMessage.RemoveAt(index);
			allTypes.RepeatedTestAllTypesNoEventsMessage.RemoveAt(index);
			allTypes.RepeatedNestedEnum.RemoveAt(index);
			allTypes.RepeatedForeignEnum.RemoveAt(index);
			allTypes.RepeatedImportEnum.RemoveAt(index);
		}

		public static void RemoveAtRepeated(TestAllTypesNoEvents allTypes, int index) {
			allTypes.RepeatedInt32.RemoveAt(index);
			allTypes.RepeatedInt64.RemoveAt(index);
			allTypes.RepeatedUint32.RemoveAt(index);
			allTypes.RepeatedUint64.RemoveAt(index);
			allTypes.RepeatedSint32.RemoveAt(index);
			allTypes.RepeatedSint64.RemoveAt(index);
			allTypes.RepeatedFixed32.RemoveAt(index);
			allTypes.RepeatedFixed64.RemoveAt(index);
			allTypes.RepeatedSfixed32.RemoveAt(index);
			allTypes.RepeatedSfixed64.RemoveAt(index);
			allTypes.RepeatedFloat.RemoveAt(index);
			allTypes.RepeatedDouble.RemoveAt(index);
			allTypes.RepeatedBool.RemoveAt(index);
			allTypes.RepeatedString.RemoveAt(index);
			allTypes.RepeatedBytes.RemoveAt(index);
			allTypes.RepeatedNestedMessage.RemoveAt(index);
			allTypes.RepeatedForeignMessage.RemoveAt(index);
			allTypes.RepeatedImportMessage.RemoveAt(index);
			allTypes.RepeatedImportNoEvents.RemoveAt(index);
			allTypes.RepeatedTestAllTypesMessage.RemoveAt(index);
			allTypes.RepeatedTestAllTypesNoEventsMessage.RemoveAt(index);
			allTypes.RepeatedNestedEnum.RemoveAt(index);
			allTypes.RepeatedForeignEnum.RemoveAt(index);
			allTypes.RepeatedImportEnum.RemoveAt(index);
		}

		public static void ReplaceRepeated(TestAllTypes allTypes, int index, int offsetIndex = 0) {
			allTypes.RepeatedInt32[index] = IntValue(offsetIndex);
			allTypes.RepeatedInt64[index] = IntValue(offsetIndex);
			allTypes.RepeatedUint32[index] = UintValue(offsetIndex);
			allTypes.RepeatedUint64[index] = UintValue(offsetIndex);
			allTypes.RepeatedSint32[index] = IntValue(offsetIndex);
			allTypes.RepeatedSint64[index] = IntValue(offsetIndex);
			allTypes.RepeatedFixed32[index] = UintValue(offsetIndex);
			allTypes.RepeatedFixed64[index] = UintValue(offsetIndex);
			allTypes.RepeatedSfixed32[index] = IntValue(offsetIndex);
			allTypes.RepeatedSfixed64[index] = IntValue(offsetIndex);
			allTypes.RepeatedFloat[index] = FloatValue(offsetIndex);
			allTypes.RepeatedDouble[index] = FloatValue(offsetIndex);
			allTypes.RepeatedBool[index] = BoolValue(offsetIndex);
			allTypes.RepeatedString[index] = StringValue(offsetIndex);
			allTypes.RepeatedBytes[index] = ByteValue(offsetIndex);
			allTypes.RepeatedNestedMessage[index] = NestedMessageValue(offsetIndex);
			allTypes.RepeatedForeignMessage[index] = ForeignMessageValue(offsetIndex);
			allTypes.RepeatedImportMessage[index] = ImportMessageValue(offsetIndex);
			allTypes.RepeatedImportNoEvents[index] = ImportMessageNoEventsValue(offsetIndex);
			allTypes.RepeatedTestAllTypesMessage[index] = TestAllTypesValue(offsetIndex);
			allTypes.RepeatedTestAllTypesNoEventsMessage[index] = TestAllTypesNoEventsValue(offsetIndex);
			allTypes.RepeatedNestedEnum[index] = NestedEnumValue(offsetIndex);
			allTypes.RepeatedForeignEnum[index] = ForeignEnumValue(offsetIndex);
			allTypes.RepeatedImportEnum[index] = ImportEnumValue(offsetIndex);
		}

		public static void ReplaceRepeated(TestAllTypesNoEvents allTypes, int index, int offsetIndex = 0) {
			allTypes.RepeatedInt32[index] = IntValue(offsetIndex);
			allTypes.RepeatedInt64[index] = IntValue(offsetIndex);
			allTypes.RepeatedUint32[index] = UintValue(offsetIndex);
			allTypes.RepeatedUint64[index] = UintValue(offsetIndex);
			allTypes.RepeatedSint32[index] = IntValue(offsetIndex);
			allTypes.RepeatedSint64[index] = IntValue(offsetIndex);
			allTypes.RepeatedFixed32[index] = UintValue(offsetIndex);
			allTypes.RepeatedFixed64[index] = UintValue(offsetIndex);
			allTypes.RepeatedSfixed32[index] = IntValue(offsetIndex);
			allTypes.RepeatedSfixed64[index] = IntValue(offsetIndex);
			allTypes.RepeatedFloat[index] = FloatValue(offsetIndex);
			allTypes.RepeatedDouble[index] = FloatValue(offsetIndex);
			allTypes.RepeatedBool[index] = BoolValue(offsetIndex);
			allTypes.RepeatedString[index] = StringValue(offsetIndex);
			allTypes.RepeatedBytes[index] = ByteValue(offsetIndex);
			allTypes.RepeatedNestedMessage[index] = NestedMessageNoEventsValue(offsetIndex);
			allTypes.RepeatedForeignMessage[index] = ForeignMessageNoEventsValue(offsetIndex);
			allTypes.RepeatedImportMessage[index] = ImportMessageValue(offsetIndex);
			allTypes.RepeatedImportNoEvents[index] = ImportMessageNoEventsValue(offsetIndex);
			allTypes.RepeatedTestAllTypesMessage[index] = TestAllTypesValue(offsetIndex);
			allTypes.RepeatedTestAllTypesNoEventsMessage[index] = TestAllTypesNoEventsValue(offsetIndex);
			allTypes.RepeatedNestedEnum[index] = NestedEnumNoEventsValue(offsetIndex);
			allTypes.RepeatedForeignEnum[index] = ForeignEnumValue(offsetIndex);
			allTypes.RepeatedImportEnum[index] = ImportEnumValue(offsetIndex);
		}

		public static void InsertRepeated(TestAllTypes allTypes, int index, int offsetIndex = 0) {
			allTypes.RepeatedInt32.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedInt64.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedUint32.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedUint64.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedSint32.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedSint64.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedFixed32.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedFixed64.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedSfixed32.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedSfixed64.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedFloat.Insert(index, FloatValue(offsetIndex));
			allTypes.RepeatedDouble.Insert(index, FloatValue(offsetIndex));
			allTypes.RepeatedBool.Insert(index, BoolValue(offsetIndex));
			allTypes.RepeatedString.Insert(index, StringValue(offsetIndex));
			allTypes.RepeatedBytes.Insert(index, ByteValue(offsetIndex));
			allTypes.RepeatedNestedMessage.Insert(index, NestedMessageValue(offsetIndex));
			allTypes.RepeatedForeignMessage.Insert(index, ForeignMessageValue(offsetIndex));
			allTypes.RepeatedImportMessage.Insert(index, ImportMessageValue(offsetIndex));
			allTypes.RepeatedImportNoEvents.Insert(index, ImportMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedTestAllTypesMessage.Insert(index, TestAllTypesValue(offsetIndex));
			allTypes.RepeatedTestAllTypesNoEventsMessage.Insert(index, TestAllTypesNoEventsValue(offsetIndex));
			allTypes.RepeatedNestedEnum.Insert(index, NestedEnumValue(offsetIndex));
			allTypes.RepeatedForeignEnum.Insert(index, ForeignEnumValue(offsetIndex));
			allTypes.RepeatedImportEnum.Insert(index, ImportEnumValue(offsetIndex));
		}

		public static void InsertRepeated(TestAllTypesNoEvents allTypes, int index, int offsetIndex = 0) {
			allTypes.RepeatedInt32.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedInt64.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedUint32.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedUint64.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedSint32.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedSint64.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedFixed32.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedFixed64.Insert(index, UintValue(offsetIndex));
			allTypes.RepeatedSfixed32.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedSfixed64.Insert(index, IntValue(offsetIndex));
			allTypes.RepeatedFloat.Insert(index, FloatValue(offsetIndex));
			allTypes.RepeatedDouble.Insert(index, FloatValue(offsetIndex));
			allTypes.RepeatedBool.Insert(index, BoolValue(offsetIndex));
			allTypes.RepeatedString.Insert(index, StringValue(offsetIndex));
			allTypes.RepeatedBytes.Insert(index, ByteValue(offsetIndex));
			allTypes.RepeatedNestedMessage.Insert(index, NestedMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedForeignMessage.Insert(index, ForeignMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedImportMessage.Insert(index, ImportMessageValue(offsetIndex));
			allTypes.RepeatedImportNoEvents.Insert(index, ImportMessageNoEventsValue(offsetIndex));
			allTypes.RepeatedTestAllTypesMessage.Insert(index, TestAllTypesValue(offsetIndex));
			allTypes.RepeatedTestAllTypesNoEventsMessage.Insert(index, TestAllTypesNoEventsValue(offsetIndex));
			allTypes.RepeatedNestedEnum.Insert(index, NestedEnumNoEventsValue(offsetIndex));
			allTypes.RepeatedForeignEnum.Insert(index, ForeignEnumValue(offsetIndex));
			allTypes.RepeatedImportEnum.Insert(index, ImportEnumValue(offsetIndex));
		}

		public static void ClearRepeated(TestAllTypes allTypes) {
			allTypes.RepeatedInt32.Clear();
			allTypes.RepeatedInt64.Clear();
			allTypes.RepeatedUint32.Clear();
			allTypes.RepeatedUint64.Clear();
			allTypes.RepeatedSint32.Clear();
			allTypes.RepeatedSint64.Clear();
			allTypes.RepeatedFixed32.Clear();
			allTypes.RepeatedFixed64.Clear();
			allTypes.RepeatedSfixed32.Clear();
			allTypes.RepeatedSfixed64.Clear();
			allTypes.RepeatedFloat.Clear();
			allTypes.RepeatedDouble.Clear();
			allTypes.RepeatedBool.Clear();
			allTypes.RepeatedString.Clear();
			allTypes.RepeatedBytes.Clear();
			allTypes.RepeatedNestedMessage.Clear();
			allTypes.RepeatedForeignMessage.Clear();
			allTypes.RepeatedImportMessage.Clear();
			allTypes.RepeatedImportNoEvents.Clear();
			allTypes.RepeatedTestAllTypesMessage.Clear();
			allTypes.RepeatedTestAllTypesNoEventsMessage.Clear();
			allTypes.RepeatedNestedEnum.Clear();
			allTypes.RepeatedForeignEnum.Clear();
			allTypes.RepeatedImportEnum.Clear();
		}

		public static void ClearRepeated(TestAllTypesNoEvents allTypes) {
			allTypes.RepeatedInt32.Clear();
			allTypes.RepeatedInt64.Clear();
			allTypes.RepeatedUint32.Clear();
			allTypes.RepeatedUint64.Clear();
			allTypes.RepeatedSint32.Clear();
			allTypes.RepeatedSint64.Clear();
			allTypes.RepeatedFixed32.Clear();
			allTypes.RepeatedFixed64.Clear();
			allTypes.RepeatedSfixed32.Clear();
			allTypes.RepeatedSfixed64.Clear();
			allTypes.RepeatedFloat.Clear();
			allTypes.RepeatedDouble.Clear();
			allTypes.RepeatedBool.Clear();
			allTypes.RepeatedString.Clear();
			allTypes.RepeatedBytes.Clear();
			allTypes.RepeatedNestedMessage.Clear();
			allTypes.RepeatedForeignMessage.Clear();
			allTypes.RepeatedImportMessage.Clear();
			allTypes.RepeatedImportNoEvents.Clear();
			allTypes.RepeatedTestAllTypesMessage.Clear();
			allTypes.RepeatedTestAllTypesNoEventsMessage.Clear();
			allTypes.RepeatedNestedEnum.Clear();
			allTypes.RepeatedForeignEnum.Clear();
			allTypes.RepeatedImportEnum.Clear();
		}

		public static void UpdateRepeated(TestAllTypes allTypes, int index, int offsetIndex = 0) {
			allTypes.RepeatedNestedMessage[index].Bb = IntValue(offsetIndex);
			allTypes.RepeatedForeignMessage[index].C = IntValue(offsetIndex);
			allTypes.RepeatedImportMessage[index].D = IntValue(offsetIndex);
			//allTypes.RepeatedImportNoEvents[index].D = IntValue(offsetIndex);
			SetSingular(allTypes.RepeatedTestAllTypesMessage[index], offsetIndex);
			//SetSingular(allTypes.RepeatedTestAllTypesNoEventsMessage[index], offsetIndex);
		}

		public static void UpdateRepeated(TestAllTypesNoEvents allTypes, int index, int offsetIndex = 0) {
			allTypes.RepeatedNestedMessage[index].Bb = IntValue(offsetIndex);
			allTypes.RepeatedForeignMessage[index].C = IntValue(offsetIndex);
			allTypes.RepeatedImportMessage[index].D = IntValue(offsetIndex);
			//allTypes.RepeatedImportNoEvents[index].D = IntValue(offsetIndex);
			SetSingular(allTypes.RepeatedTestAllTypesMessage[index], offsetIndex);
			//SetSingular(allTypes.RepeatedTestAllTypesNoEventsMessage[index], offsetIndex);
		}

		public static void AddMap(TestAllTypes allTypes, int keyIndex, int offsetIndex = 0) {
			allTypes.MapInt32Int32.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapInt64Int64.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapUint32Uint32.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapUint64Uint64.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapSint32Sint32.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapSint64Sint64.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapFixed32Fixed32.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapFixed64Fixed64.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapSfixed32Sfixed32.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapSfixed64Sfixed64.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapInt32Float.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapInt32Double.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapBoolBool.Add(BoolValue(keyIndex), BoolValue(offsetIndex));
			allTypes.MapStringString.Add(StringValue(keyIndex), StringValue(offsetIndex));
			allTypes.MapInt32Bytes.Add(IntValue(keyIndex), ByteValue(offsetIndex));
			allTypes.MapInt32Enum.Add(IntValue(keyIndex), MapEnumValue(offsetIndex));
			allTypes.MapInt32ForeignMessage.Add(IntValue(keyIndex), ForeignMessageValue(offsetIndex));
			allTypes.MapInt32ForeignNoEventsMessage.Add(IntValue(keyIndex), ForeignMessageNoEventsValue(offsetIndex));
			allTypes.MapInt32TestAllTypesMessage.Add(IntValue(keyIndex), TestAllTypesValue(offsetIndex));
			allTypes.MapInt32TestAllTypesNoEventsMessage.Add(IntValue(keyIndex), TestAllTypesNoEventsValue(offsetIndex));
		}

		public static void AddMap(TestAllTypesNoEvents allTypes, int keyIndex, int offsetIndex = 0) {
			allTypes.MapInt32Int32.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapInt64Int64.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapUint32Uint32.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapUint64Uint64.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapSint32Sint32.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapSint64Sint64.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapFixed32Fixed32.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapFixed64Fixed64.Add(UintValue(keyIndex), UintValue(offsetIndex));
			allTypes.MapSfixed32Sfixed32.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapSfixed64Sfixed64.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapInt32Float.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapInt32Double.Add(IntValue(keyIndex), IntValue(offsetIndex));
			allTypes.MapBoolBool.Add(BoolValue(keyIndex), BoolValue(offsetIndex));
			allTypes.MapStringString.Add(StringValue(keyIndex), StringValue(offsetIndex));
			allTypes.MapInt32Bytes.Add(IntValue(keyIndex), ByteValue(offsetIndex));
			allTypes.MapInt32Enum.Add(IntValue(keyIndex), MapEnumValue(offsetIndex));
			allTypes.MapInt32ForeignMessage.Add(IntValue(keyIndex), ForeignMessageValue(offsetIndex));
			allTypes.MapInt32ForeignNoEventsMessage.Add(IntValue(keyIndex), ForeignMessageNoEventsValue(offsetIndex));
			allTypes.MapInt32TestAllTypesMessage.Add(IntValue(keyIndex), TestAllTypesValue(offsetIndex));
			allTypes.MapInt32TestAllTypesNoEventsMessage.Add(IntValue(keyIndex), TestAllTypesNoEventsValue(offsetIndex));
		}

		public static void RemoveMap(TestAllTypes allTypes, int keyIndex) {
			allTypes.MapInt32Int32.Remove(IntValue(keyIndex));
			allTypes.MapInt64Int64.Remove(IntValue(keyIndex));
			allTypes.MapUint32Uint32.Remove(UintValue(keyIndex));
			allTypes.MapUint64Uint64.Remove(UintValue(keyIndex));
			allTypes.MapSint32Sint32.Remove(IntValue(keyIndex));
			allTypes.MapSint64Sint64.Remove(IntValue(keyIndex));
			allTypes.MapFixed32Fixed32.Remove(UintValue(keyIndex));
			allTypes.MapFixed64Fixed64.Remove(UintValue(keyIndex));
			allTypes.MapSfixed32Sfixed32.Remove(IntValue(keyIndex));
			allTypes.MapSfixed64Sfixed64.Remove(IntValue(keyIndex));
			allTypes.MapInt32Float.Remove(IntValue(keyIndex));
			allTypes.MapInt32Double.Remove(IntValue(keyIndex));
			allTypes.MapBoolBool.Remove(BoolValue(keyIndex));
			allTypes.MapStringString.Remove(StringValue(keyIndex));
			allTypes.MapInt32Bytes.Remove(IntValue(keyIndex));
			allTypes.MapInt32Enum.Remove(IntValue(keyIndex));
			allTypes.MapInt32ForeignMessage.Remove(IntValue(keyIndex));
			allTypes.MapInt32ForeignNoEventsMessage.Remove(IntValue(keyIndex));
			allTypes.MapInt32TestAllTypesMessage.Remove(IntValue(keyIndex));
			allTypes.MapInt32TestAllTypesNoEventsMessage.Remove(IntValue(keyIndex));
		}

		public static void RemoveMap(TestAllTypesNoEvents allTypes, int keyIndex) {
			allTypes.MapInt32Int32.Remove(IntValue(keyIndex));
			allTypes.MapInt64Int64.Remove(IntValue(keyIndex));
			allTypes.MapUint32Uint32.Remove(UintValue(keyIndex));
			allTypes.MapUint64Uint64.Remove(UintValue(keyIndex));
			allTypes.MapSint32Sint32.Remove(IntValue(keyIndex));
			allTypes.MapSint64Sint64.Remove(IntValue(keyIndex));
			allTypes.MapFixed32Fixed32.Remove(UintValue(keyIndex));
			allTypes.MapFixed64Fixed64.Remove(UintValue(keyIndex));
			allTypes.MapSfixed32Sfixed32.Remove(IntValue(keyIndex));
			allTypes.MapSfixed64Sfixed64.Remove(IntValue(keyIndex));
			allTypes.MapInt32Float.Remove(IntValue(keyIndex));
			allTypes.MapInt32Double.Remove(IntValue(keyIndex));
			allTypes.MapBoolBool.Remove(BoolValue(keyIndex));
			allTypes.MapStringString.Remove(StringValue(keyIndex));
			allTypes.MapInt32Bytes.Remove(IntValue(keyIndex));
			allTypes.MapInt32Enum.Remove(IntValue(keyIndex));
			allTypes.MapInt32ForeignMessage.Remove(IntValue(keyIndex));
			allTypes.MapInt32ForeignNoEventsMessage.Remove(IntValue(keyIndex));
			allTypes.MapInt32TestAllTypesMessage.Remove(IntValue(keyIndex));
			allTypes.MapInt32TestAllTypesNoEventsMessage.Remove(IntValue(keyIndex));
		}

		public static void ReplaceMap(TestAllTypes allTypes, int keyIndex, int offsetIndex = 0) {
			allTypes.MapInt32Int32[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapInt64Int64[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapUint32Uint32[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapUint64Uint64[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapSint32Sint32[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapSint64Sint64[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapFixed32Fixed32[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapFixed64Fixed64[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapSfixed32Sfixed32[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapSfixed64Sfixed64[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapInt32Float[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapInt32Double[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapBoolBool[BoolValue(keyIndex)] = BoolValue(offsetIndex);
			allTypes.MapStringString[StringValue(keyIndex)] = StringValue(offsetIndex);
			allTypes.MapInt32Bytes[IntValue(keyIndex)] = ByteValue(offsetIndex);
			allTypes.MapInt32Enum[IntValue(keyIndex)] = MapEnumValue(offsetIndex);
			allTypes.MapInt32ForeignMessage[IntValue(keyIndex)] = ForeignMessageValue(offsetIndex);
			allTypes.MapInt32ForeignNoEventsMessage[IntValue(keyIndex)] = ForeignMessageNoEventsValue(offsetIndex);
			allTypes.MapInt32TestAllTypesMessage[IntValue(keyIndex)] = TestAllTypesValue(offsetIndex);
			allTypes.MapInt32TestAllTypesNoEventsMessage[IntValue(keyIndex)] = TestAllTypesNoEventsValue(offsetIndex);
		}

		public static void ReplaceMap(TestAllTypesNoEvents allTypes, int keyIndex, int offsetIndex = 0) {
			allTypes.MapInt32Int32[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapInt64Int64[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapUint32Uint32[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapUint64Uint64[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapSint32Sint32[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapSint64Sint64[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapFixed32Fixed32[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapFixed64Fixed64[UintValue(keyIndex)] = UintValue(offsetIndex);
			allTypes.MapSfixed32Sfixed32[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapSfixed64Sfixed64[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapInt32Float[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapInt32Double[IntValue(keyIndex)] = IntValue(offsetIndex);
			allTypes.MapBoolBool[BoolValue(keyIndex)] = BoolValue(offsetIndex);
			allTypes.MapStringString[StringValue(keyIndex)] = StringValue(offsetIndex);
			allTypes.MapInt32Bytes[IntValue(keyIndex)] = ByteValue(offsetIndex);
			allTypes.MapInt32Enum[IntValue(keyIndex)] = MapEnumValue(offsetIndex);
			allTypes.MapInt32ForeignMessage[IntValue(keyIndex)] = ForeignMessageValue(offsetIndex);
			allTypes.MapInt32ForeignNoEventsMessage[IntValue(keyIndex)] = ForeignMessageNoEventsValue(offsetIndex);
			allTypes.MapInt32TestAllTypesMessage[IntValue(keyIndex)] = TestAllTypesValue(offsetIndex);
			allTypes.MapInt32TestAllTypesNoEventsMessage[IntValue(keyIndex)] = TestAllTypesNoEventsValue(offsetIndex);
		}

		public static void ClearMap(TestAllTypes allTypes) {
			allTypes.MapInt32Int32.Clear();
			allTypes.MapInt64Int64.Clear();
			allTypes.MapUint32Uint32.Clear();
			allTypes.MapUint64Uint64.Clear();
			allTypes.MapSint32Sint32.Clear();
			allTypes.MapSint64Sint64.Clear();
			allTypes.MapFixed32Fixed32.Clear();
			allTypes.MapFixed64Fixed64.Clear();
			allTypes.MapSfixed32Sfixed32.Clear();
			allTypes.MapSfixed64Sfixed64.Clear();
			allTypes.MapInt32Float.Clear();
			allTypes.MapInt32Double.Clear();
			allTypes.MapBoolBool.Clear();
			allTypes.MapStringString.Clear();
			allTypes.MapInt32Bytes.Clear();
			allTypes.MapInt32Enum.Clear();
			allTypes.MapInt32ForeignMessage.Clear();
			allTypes.MapInt32ForeignNoEventsMessage.Clear();
			allTypes.MapInt32TestAllTypesMessage.Clear();
			allTypes.MapInt32TestAllTypesNoEventsMessage.Clear();
		}

		public static void ClearMap(TestAllTypesNoEvents allTypes) {
			allTypes.MapInt32Int32.Clear();
			allTypes.MapInt64Int64.Clear();
			allTypes.MapUint32Uint32.Clear();
			allTypes.MapUint64Uint64.Clear();
			allTypes.MapSint32Sint32.Clear();
			allTypes.MapSint64Sint64.Clear();
			allTypes.MapFixed32Fixed32.Clear();
			allTypes.MapFixed64Fixed64.Clear();
			allTypes.MapSfixed32Sfixed32.Clear();
			allTypes.MapSfixed64Sfixed64.Clear();
			allTypes.MapInt32Float.Clear();
			allTypes.MapInt32Double.Clear();
			allTypes.MapBoolBool.Clear();
			allTypes.MapStringString.Clear();
			allTypes.MapInt32Bytes.Clear();
			allTypes.MapInt32Enum.Clear();
			allTypes.MapInt32ForeignMessage.Clear();
			allTypes.MapInt32ForeignNoEventsMessage.Clear();
			allTypes.MapInt32TestAllTypesMessage.Clear();
			allTypes.MapInt32TestAllTypesNoEventsMessage.Clear();
		}

		public static void UpdateMap(TestAllTypes allTypes, int keyIndex, int offsetIndex = 0) {
			allTypes.MapInt32ForeignMessage[IntValue(keyIndex)].C = IntValue(offsetIndex);
			//allTypes.MapInt32ForeignNoEventsMessage[IntValue(keyIndex)].C = IntValue(offsetIndex);
			SetSingular(allTypes.MapInt32TestAllTypesMessage[IntValue(keyIndex)], offsetIndex);
			//SetSingular(allTypes.MapInt32TestAllTypesNoEventsMessage[IntValue(keyIndex)], offsetIndex);
		}

		public static void UpdateMap(TestAllTypesNoEvents allTypes, int keyIndex, int offsetIndex = 0) {
			allTypes.MapInt32ForeignMessage[IntValue(keyIndex)].C = IntValue(offsetIndex);
			//allTypes.MapInt32ForeignNoEventsMessage[IntValue(keyIndex)].C = IntValue(offsetIndex);
			SetSingular(allTypes.MapInt32TestAllTypesMessage[IntValue(keyIndex)], offsetIndex);
			//SetSingular(allTypes.MapInt32TestAllTypesNoEventsMessage[IntValue(keyIndex)], offsetIndex);
		}

		public static void ApplyAllChanges(TestAllTypes allTypes, int offsetIndex = 0) {
			// single
			SetSingular(allTypes, offsetIndex);
			SetEnums(allTypes, offsetIndex);
			SetNestedMessages(allTypes, offsetIndex);
			UpdateNestedMessages(allTypes, offsetIndex);

			// lists
			AddRepeated(allTypes, offsetIndex);
			RemoveRepeated(allTypes, offsetIndex);
			AddRepeated(allTypes, offsetIndex);
			RemoveAtRepeated(allTypes, 0);
			AddRepeated(allTypes, offsetIndex);
			ReplaceRepeated(allTypes, 0, offsetIndex);
			ReplaceRepeated(allTypes, 0, offsetIndex + 1);
			InsertRepeated(allTypes, 0, offsetIndex + 2);
			ClearRepeated(allTypes);
			AddRepeated(allTypes, offsetIndex);
			UpdateRepeated(allTypes, 0, offsetIndex);

			// maps
			AddMap(allTypes, offsetIndex, offsetIndex);
			RemoveMap(allTypes, offsetIndex);
			AddMap(allTypes, offsetIndex, offsetIndex);
			ReplaceMap(allTypes, offsetIndex, offsetIndex);
			ReplaceMap(allTypes, offsetIndex, offsetIndex + 1);
			ClearMap(allTypes);
			AddMap(allTypes, offsetIndex, offsetIndex);
			UpdateMap(allTypes, offsetIndex, offsetIndex + 1);

			// oneof
			allTypes.OneofUint32 = UintValue(offsetIndex);
			allTypes.OneofNestedMessage = NestedMessageValue(offsetIndex);
			allTypes.OneofString = StringValue(offsetIndex);
			allTypes.OneofBytes = ByteValue(offsetIndex);
			allTypes.OneofForeignMessage = ForeignMessageValue(offsetIndex);
			allTypes.OneofForeignMessageNoEvents = ForeignMessageNoEventsValue(offsetIndex);
			allTypes.OneofAllTypesNoEvents = TestAllTypesNoEventsValue(offsetIndex);
			allTypes.OneofAllTypes = TestAllTypesValue(offsetIndex);
			SetNestedMessages(allTypes.OneofAllTypes, offsetIndex);

			// nested types
			allTypes.AllTypes = TestAllTypesValue(offsetIndex);
			SetNestedMessages(allTypes.AllTypes , offsetIndex);
			allTypes.AllTypesNoEvents = TestAllTypesNoEventsValue(offsetIndex);
		}

		public static void ApplyAllChanges(TestAllTypesNoEvents allTypes) {
			// single
			SetSingular(allTypes);
			SetEnums(allTypes);
			SetNestedMessages(allTypes);
			UpdateNestedMessages(allTypes);

			// lists
			AddRepeated(allTypes);
			RemoveRepeated(allTypes);
			AddRepeated(allTypes);
			RemoveAtRepeated(allTypes, 0);
			AddRepeated(allTypes);
			ReplaceRepeated(allTypes, 0);
			ReplaceRepeated(allTypes, 0, 1);
			InsertRepeated(allTypes, 0, 2);
			ClearRepeated(allTypes);
			AddRepeated(allTypes);
			UpdateRepeated(allTypes, 0);

			// maps
			AddMap(allTypes, 1);
			RemoveMap(allTypes, 1);
			AddMap(allTypes, 1);
			ReplaceMap(allTypes, 1);
			ReplaceMap(allTypes, 1, 1);
			ClearMap(allTypes);
			AddMap(allTypes, 1);
			UpdateMap(allTypes, 1);

			// oneof
			allTypes.OneofUint32 = UintValue(0);
			allTypes.OneofNestedMessage = NestedMessageNoEventsValue(0);
			allTypes.OneofString = StringValue(0);
			allTypes.OneofBytes = ByteValue(0);
			allTypes.OneofForeignMessage = ForeignMessageValue(0);
			allTypes.OneofForeignMessageNoEvents = ForeignMessageNoEventsValue(0);
			allTypes.OneofAllTypes = TestAllTypesValue(0);
			allTypes.OneofAllTypesNoEvents = TestAllTypesNoEventsValue(0);

			// nested types
			allTypes.AllTypes = TestAllTypesValue(0);
			allTypes.AllTypesNoEvents = TestAllTypesNoEventsValue(0);
		}
	}
}