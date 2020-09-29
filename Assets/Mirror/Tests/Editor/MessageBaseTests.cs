using NUnit.Framework;

namespace Mirror.Tests.MessageTests
{
    struct TestMessage : NetworkMessage
    {
        public int IntValue;
        public string StringValue;
        public double DoubleValue;

        public TestMessage(int i, string s, double d)
        {
            IntValue = i;
            StringValue = s;
            DoubleValue = d;
        }

        public void Deserialize(NetworkReader reader)
        {
            IntValue = reader.ReadInt32();
            StringValue = reader.ReadString();
            DoubleValue = reader.ReadDouble();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.WriteInt32(IntValue);
            writer.WriteString(StringValue);
            writer.WriteDouble(DoubleValue);
        }
    }

    struct StructWithEmptyMethodMessage : NetworkMessage
    {
        public int IntValue;
        public string StringValue;
        public double DoubleValue;

        // weaver populates (de)serialize automatically
        public void Deserialize(NetworkReader reader) {}
        public void Serialize(NetworkWriter writer) {}
    }

    class NullableObject
    {
        public int id = 3;
    }

    struct NullableObjectMessage : NetworkMessage
    {
        public string name;
        public NullableObject nullObj;

        // weaver populates (de)serialize automatically
        public void Deserialize(NetworkReader reader) {}
        public void Serialize(NetworkWriter writer) {}
    }

    [TestFixture]
    public class MessageBaseTests
    {
        [Test]
        public void StructWithMethods()
        {
            byte[] arr = MessagePackingTest.PackToByteArray(new TestMessage(1, "2", 3.3));
            TestMessage t = MessagePackingTest.UnpackFromByteArray<TestMessage>(arr);

            Assert.AreEqual(1, t.IntValue);
        }

        [Test]
        public void StructWithEmptyMethods()
        {
            byte[] arr = MessagePackingTest.PackToByteArray(new StructWithEmptyMethodMessage { IntValue = 1, StringValue = "2", DoubleValue = 3.3 });
            StructWithEmptyMethodMessage t = MessagePackingTest.UnpackFromByteArray<StructWithEmptyMethodMessage>(arr);

            Assert.AreEqual(1, t.IntValue);
            Assert.AreEqual("2", t.StringValue);
            Assert.AreEqual(3.3, t.DoubleValue);
        }

        [Test]
        public void NullObjectMessageTest()
        {
            NullableObjectMessage nullableObjectMessage = new NullableObjectMessage
            {
                name = "anon",
                nullObj = null
            };

            byte[] data = MessagePackingTest.PackToByteArray(nullableObjectMessage);

            NullableObjectMessage unpacked = MessagePackingTest.UnpackFromByteArray<NullableObjectMessage>(data);
            Assert.That(unpacked.name, Is.EqualTo("anon"));
            Assert.That(unpacked.nullObj, Is.Null);
        }
    }
}
