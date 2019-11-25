namespace DemoAndDiscourse.RocksDb.RocksAbstractions
{
    public enum Operation
    {
        DataUpdated,
        DataDeleted
    }

    public class DataChangedEvent<TKey, TValue>
    {
        public DataChangedEvent(Operation operation, (TKey key, TValue value) data)
        {
            Operation = operation;
            Data = data;
        }

        public Operation Operation { get; set; }

        public (TKey key, TValue value) Data { get; set; }
    }
}