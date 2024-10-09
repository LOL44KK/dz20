namespace dz20
{
    internal class Program
    {
        struct FileExtensionInfo
        {
            public int Rank;
            public string Extension;
            public long Count;
            public long TotalSize;
            public double PercentCount;
            public double PercentSize;
        }

        static void Main()
        {
            var enumerationOptions = new EnumerationOptions
            {
                IgnoreInaccessible = true, 
                RecurseSubdirectories = true
            };

            var files = Directory.GetFiles(@"C:\", "*.*", enumerationOptions);

            Dictionary<string, (long Count, long TotalSize)> fileStats = new Dictionary<string, (long, long)>();

            foreach (var file in files)
            {
                string extension = Path.GetExtension(file).ToLower();
                long fileSize = new FileInfo(file).Length;

                if (string.IsNullOrEmpty(extension)) continue;

                extension = extension.TrimStart('.');

                if (fileStats.ContainsKey(extension))
                {
                    fileStats[extension] = (fileStats[extension].Count + 1, fileStats[extension].TotalSize + fileSize);
                }
                else
                {
                    fileStats[extension] = (1, fileSize);
                }
            }

            long totalFiles = fileStats.Sum(f => f.Value.Count);
            long totalSize = fileStats.Sum(f => f.Value.TotalSize);

            var extensions = fileStats
                .OrderByDescending(f => f.Value.Count)
                .Take(25)
                .Select((f, index) => new FileExtensionInfo
                {
                    Rank = index + 1,
                    Extension = f.Key,
                    Count = f.Value.Count,
                    TotalSize = f.Value.TotalSize,
                    PercentCount = (double)f.Value.Count / totalFiles * 100,
                    PercentSize = (double)f.Value.TotalSize / totalSize * 100
                }
            ).ToList();


            Console.WriteLine("+----+------------+---------+-----------------+------------------+-----------------+");
            Console.WriteLine("| №  | Extension  | Count   | Total Size in B | % of Total Count | % of Total Size |");
            Console.WriteLine("+----+------------+---------+-----------------+------------------+-----------------+");
            foreach (var extension in extensions)
            {
                Console.WriteLine($"| {extension.Rank,-2} | {extension.Extension,-10} | {extension.Count,-7} | {extension.TotalSize,-15} | {extension.PercentCount,-16:F2} | {extension.PercentSize,-15:F2} |");
            }
            Console.WriteLine("+----+------------+---------+-----------------+------------------+-----------------+");
            Console.WriteLine($"| TOTAL:          | {totalFiles,-7} | {totalSize,-15} | 100.00           | 100.00          |");
            Console.WriteLine("+----+------------+---------+-----------------+------------------+-----------------+");
        }
    }
}
