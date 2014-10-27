using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payboard.Common.Serializers;
using ProtoBuf;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void ProtobufSerializer_ShouldRoundTrip()
        {
            var obj = new ClassToSerialize
            {
                Name = "Bob",
                Id = 100
            };

            var serializer = new ProtobufSerializer();
            var serialized = serializer.SerializeToStream(obj);
            var deserialized = serializer.Deserialize<ClassToSerialize>(serialized);

            deserialized.Name.ShouldEqual(obj.Name);
            deserialized.Id.ShouldEqual(obj.Id);
        }

        [TestMethod]
        public void ProtobufSerializer_ShouldRoundTripCircularReferences()
        {
            var obj = new SelfReferential
            {
                ClassToSerialize = new ClassToSerialize
                {
                    Name = "Bob",
                    Id = 100
                }
            };
            obj.Me = obj;

            var serializer = new ProtobufSerializer();
            var serialized = serializer.SerializeToStream(obj);
            var deserialized = serializer.Deserialize<SelfReferential>(serialized);

            deserialized.ClassToSerialize.Name.ShouldEqual(obj.ClassToSerialize.Name);
            deserialized.ClassToSerialize.Id.ShouldEqual(obj.ClassToSerialize.Id);
            deserialized.Me.ShouldEqual(deserialized);
        }

        [TestMethod]
        public void JsonSerializer_ShouldRoundTrip()
        {
            var obj = new ClassToSerialize
            {
                Name = "Bob",
                Id = 100
            };

            var serializer = new JsonSerializer();
            var serialized = serializer.SerializeToStream(obj);
            var deserialized = serializer.Deserialize<ClassToSerialize>(serialized);

            deserialized.Name.ShouldEqual(obj.Name);
            deserialized.Id.ShouldEqual(obj.Id);
        }

        [TestMethod]
        public void JsonSerializer_ShouldRoundTripCircularReferences()
        {
            var obj = new SelfReferential
            {
                ClassToSerialize = new ClassToSerialize
                {
                    Name = "Bob",
                    Id = 100
                }
            };
            obj.Me = obj;

            var serializer = new JsonSerializer();
            var serialized = serializer.SerializeToStream(obj);
            var deserialized = serializer.Deserialize<SelfReferential>(serialized);

            deserialized.ClassToSerialize.Name.ShouldEqual(obj.ClassToSerialize.Name);
            deserialized.ClassToSerialize.Id.ShouldEqual(obj.ClassToSerialize.Id);
            deserialized.Me.ShouldEqual(deserialized);
        }
    }

    [ProtoContract]
    public class ClassToSerialize
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int Id { get; set; }
    }

    [ProtoContract]
    public class SelfReferential
    {
        [ProtoMember(1)]
        public ClassToSerialize ClassToSerialize { get; set; }

        [ProtoMember(2, AsReference = true)]
        public SelfReferential Me { get; set; }
    }
}
