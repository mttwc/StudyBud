using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudyBud.Persistence
{
    public static class Parser
    {
        public static List<T> Parse<T>(string path)
        {
            using (var reader = File.OpenText(path))
            {
                var cr = new CsvReader(reader);
                var records = cr.GetRecords<T>().ToList();
                return records;
            }
        }
    }
}