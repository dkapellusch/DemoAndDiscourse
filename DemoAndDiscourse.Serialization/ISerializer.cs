using Google.Protobuf;

namespace DemoAndDiscourse.Serialization
{
    public interface ISerializer<T> where T : IMessage<T>
    {
        T Deserialize(byte[] data);

        byte[] Serialize(T data);
    }
}