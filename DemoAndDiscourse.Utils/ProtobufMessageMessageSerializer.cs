using Google.Protobuf;

namespace DemoAndDiscourse.Utils
{
    public sealed class ProtobufMessageMessageSerializer<TMessage> : IMessageSerializer<TMessage> where TMessage : IMessage<TMessage>, new()
    {
        private readonly MessageParser<TMessage> _messageParser = new MessageParser<TMessage>(() => new TMessage());

        public TMessage Deserialize(byte[] bytes) => _messageParser.ParseFrom(bytes);

        public byte[] Serialize(TMessage data) => data.ToByteArray();
    }
}