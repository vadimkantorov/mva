using System;
using System.IO;
using System.Linq;

namespace Clik
{
    public class RelevanceLogReader
    {
        public Judgement[] Read()
        {
            return File
                .ReadAllLines(filePath)
                .Select(x => x.Split('\t').Select(y => Convert.ToInt32(y)).ToArray())
                .Select(x => new Judgement {QueryId = x[0], RegionId = x[1], UrlId = x[2], RelevanceLabel = x[3]})
                .ToArray();
        }

        public RelevanceLogReader(string filePath)
        {
            this.filePath = filePath;
        }

        readonly string filePath;
    }
}