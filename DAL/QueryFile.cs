using System.Collections.Concurrent;

namespace DAL
{
    internal static class QueryFile
    {
        private static readonly ConcurrentDictionary<string, string> Cache = new(StringComparer.OrdinalIgnoreCase);

        public static string Load(string queryName)
        {
            return Cache.GetOrAdd(queryName, LoadFromDisk);
        }

        private static string LoadFromDisk(string queryName)
        {
            string fileName = queryName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)
                ? queryName
                : $"{queryName}.sql";

            List<string> searchedPaths = [];

            foreach (string path in GetCandidatePaths(fileName).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                searchedPaths.Add(path);

                if (File.Exists(path))
                {
                    return File.ReadAllText(path).Trim();
                }
            }

            throw new FileNotFoundException(
                $"SQL query file '{fileName}' was not found. Searched: {string.Join(", ", searchedPaths)}",
                fileName);
        }

        private static IEnumerable<string> GetCandidatePaths(string fileName)
        {
            yield return Path.Combine(AppContext.BaseDirectory, "Queries", fileName);
            yield return Path.Combine(AppContext.BaseDirectory, "DAL", "Queries", fileName);

            string currentDirectory = Directory.GetCurrentDirectory();
            yield return Path.Combine(currentDirectory, "Queries", fileName);
            yield return Path.Combine(currentDirectory, "DAL", "Queries", fileName);

            DirectoryInfo? directory = new(currentDirectory);

            while (directory != null)
            {
                yield return Path.Combine(directory.FullName, "Queries", fileName);
                yield return Path.Combine(directory.FullName, "DAL", "Queries", fileName);

                directory = directory.Parent;
            }
        }
    }
}
