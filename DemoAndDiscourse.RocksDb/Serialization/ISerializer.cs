namespace DemoAndDiscourse.RocksDb.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] serializedData);

        byte[] Serialize<T>(T dataToSerialize);
    }

    public interface ISerializer<T>
    {
        T Deserialize(byte[] serializedData);

        byte[] Serialize(T dataToSerialize);
    }
}