using System;
using System.Collections.Generic;
using System.Text;
using DemoAndDiscourse.RocksDb.RocksAbstractions;
using RocksDbSharp;

namespace DemoAndDiscourse.RocksDb.Extensions
{
    public static class RocksDbExtensions
    {
        public static void Put(this RocksDbSharp.RocksDb rocksDb, string key, byte[] value, ColumnFamilyHandle columnFamily)
        {
            rocksDb.Put(Encoding.UTF8.GetBytes(key), value, columnFamily);
        }

        public static byte[] Get(this RocksDbSharp.RocksDb rocksDb, string key, ColumnFamilyHandle columnFamily) => rocksDb.Get(Encoding.UTF8.GetBytes(key), columnFamily);

        public static bool ColumnFamilyExists(this RocksDatabase rocksDatabase, string columnFamilyName)
        {
            try
            {
                _ = rocksDatabase.RocksDb.GetColumnFamily(columnFamilyName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ColumnFamilyHandle GetOrCreateColumnFamily(this RocksDatabase rocksDb, string columnFamilyName)
        {
            try
            {
                return rocksDb.RocksDb.GetColumnFamily(columnFamilyName);
            }
            catch
            {
                var options = new DbOptions();

                rocksDb.RocksDb.CreateColumnFamily(options, columnFamilyName);

                return rocksDb.RocksDb.GetColumnFamily(columnFamilyName);
            }
        }

        public static IEnumerable<(byte[] key, byte[] value)> GetEnumerable(this Iterator iterator)
        {
            using var rocksIterator = iterator;

            while (rocksIterator.Valid())
            {
                var key = rocksIterator.Key();
                var value = rocksIterator.Value();

                yield return (key, value);

                rocksIterator.Next();
            }
        }

        public static IEnumerable<(byte[] key, byte[] value)> GetEnumerable(this Iterator iterator, Func<byte[], byte[], bool> condition)
        {
            using var rocksIterator = iterator;

            while (rocksIterator.Valid())
            {
                var key = rocksIterator.Key();
                var value = rocksIterator.Value();

                if (condition(key, value))
                    yield return (key, value);
                else
                    yield break;

                rocksIterator.Next();
            }
        }
    }
}