using Google.Protobuf;

namespace DemoAndDiscourse.Serialization
{
    public class JsonMessageSerializer<T> : ISerializer<T> where T : IMessage<T>
    {
        public T Deserialize(byte[] data) => throw new System.NotImplementedException();

        public byte[] Serialize(T data) => throw new System.NotImplementedException();
    }
}