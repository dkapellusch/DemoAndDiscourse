namespace DemoAndDiscourse.Utils
{
    public interface IMessageSerializer<T>
    {
        T Deserialize(byte[] bytes);

        byte[] Serialize(T data);
    }
}