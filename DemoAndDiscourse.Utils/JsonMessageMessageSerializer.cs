using System.Text;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace DemoAndDiscourse.Utils
{
    public sealed class JsonMessageMessageSerializer<TMessage> : IMessageSerializer<TMessage> where TMessage : IMessage<TMessage>, new()
    {
        private readonly Encoding _encoding;
        private readonly JsonFormatter _jsonFormatter = new JsonFormatter(JsonFormatter.Settings.Default.WithTypeRegistry(TypeRegistry.FromMessages(new TMessage().Descriptor)));
        private readonly JsonParser _jsonParser = new JsonParser(new JsonParser.Settings(1000, TypeRegistry.FromMessages(new TMessage().Descriptor)));

        public JsonMessageMessageSerializer() : this(Encoding.UTF8)
        {
        }

        public JsonMessageMessageSerializer(Encoding encoding)
        {
            _encoding = encoding;
        }

        public TMessage Deserialize(byte[] bytes) => _jsonParser.Parse<TMessage>(_encoding.GetString(bytes));

        public byte[] Serialize(TMessage data) => _encoding.GetBytes(_jsonFormatter.Format(data));
    }
}