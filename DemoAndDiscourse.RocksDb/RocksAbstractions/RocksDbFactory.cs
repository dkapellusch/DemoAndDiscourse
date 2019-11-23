using System.IO;
using System.Linq;
using RocksDbSharp;

namespace DemoAndDiscourse.RocksDb.RocksAbstractions
{
    public static class RocksDbFactory
    {
        internal static RocksDbSharp.RocksDb GetDatabase(string databasePath, SliceTransform prefixTransform)
        {
            var columnFamilies = new ColumnFamilies();

            if (File.Exists(databasePath) || Directory.Exists(databasePath))
            {
                var currentColumnFamilies = RocksDbSharp.RocksDb.ListColumnFamilies(new DbOptions(), databasePath);

                foreach (var columnFamily in currentColumnFamilies.ToHashSet())
                {
                    columnFamilies.Add(columnFamily,
                        new ColumnFamilyOptions()
                            .SetPrefixExtractor(prefixTransform)
                            .SetBlockBasedTableFactory(new BlockBasedTableOptions()
                                .SetWholeKeyFiltering(true)
                                .SetIndexType(BlockBasedTableIndexType.Binary)));
                }
            }

            return RocksDbSharp.RocksDb.Open(new DbOptions()
                    .SetCreateIfMissing()
                    .SetCreateMissingColumnFamilies()
                    .IncreaseParallelism(10),
                databasePath,
                columnFamilies);
        }
    }
}