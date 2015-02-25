using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clik
{
    public class ChunkStats
    {
        public HashSet<int> DifferentUrls = new HashSet<int>();
        public int Sessions;
    }
    
    public class UrlQueryStats
    {
        public void Run()
        {
            var urlTable = new Dictionary<int, int>();
            var queryTable = new Dictionary<int, int>();

            Action<int> queryUsed = queryId =>
                                        {
                                            if (!queryTable.ContainsKey(queryId))
                                                queryTable[queryId] = queryTable.Count;
                                        };

            Action<int> urlUsed = urlId =>
            {
                if (!urlTable.ContainsKey(urlId))
                    urlTable[urlId] = urlTable.Count;
            };

            int maxUrl = 0;
            int maxQuery = 0;
            int z = 0;

            //var trainChunkStats = Enumerable.Range(0, SplitDatasetByChunks.ChunkCount).Select(_ => new ChunkStats()).ToArray();
            //var testChunkStats = Enumerable.Range(0, SplitDatasetByChunks.ChunkCount).Select(_ => new ChunkStats()).ToArray();

            //for(int c = 0; c < SplitDatasetByChunks.ChunkCount; c++)

            var trainChunkStats = Enumerable.Range(0, 1000).Select(_ => new ChunkStats()).ToArray();
            var testChunkStats = Enumerable.Range(0, 1000).Select(_ => new ChunkStats()).ToArray();
            var c = 777;
            {
                Console.WriteLine("Starting on chunk {0}", c);
                var trainClicks = new ClickLogReader(string.Format("processed_dataset/{0}_trainClicks.txt", c)).ReadQueries();
                var testClicks = new ClickLogReader(string.Format("processed_dataset/{0}_testClicks.txt", c)).ReadQueries();
                
                foreach (var q in trainClicks)
                {
                    if (z % 1000000 == 0) Console.WriteLine(z); z++;

                    maxQuery = Math.Max(maxQuery, q.QueryId);
                    queryUsed(q.QueryId);
                    for (int i = 0; i < Constants.Ranks; i++)
                    {
                        urlUsed(q.URLs[i]);
                        maxUrl = Math.Max(maxUrl, q.URLs[i]);

                        trainChunkStats[c].DifferentUrls.Add(q.URLs[i]);
                    }
                    trainChunkStats[c].Sessions++;
                }

                foreach (var q in testClicks)
                {
                    if (z % 1000000 == 0) Console.WriteLine(z); z++;

                    maxQuery = Math.Max(maxQuery, q.QueryId);
                    queryUsed(q.QueryId);
                    for (int i = 0; i < Constants.Ranks; i++)
                    {
                        urlUsed(q.URLs[i]);
                        maxUrl = Math.Max(maxUrl, q.URLs[i]);

                        testChunkStats[c].DifferentUrls.Add(q.URLs[i]);
                    }
                    testChunkStats[c].Sessions++;
                }
            }
            
           //File.WriteAllLines("chunkStats_report.txt", Enumerable.Range(0, SplitDatasetByChunks.ChunkCount).Select(i => string.Format("Chunk {0}: [Train. Sessions: {1}, URLs: {2}] [Test. Sessions: {3}, URLs: {4} ]", i, trainChunkStats[i].Sessions, trainChunkStats[i].DifferentUrls.Count, testChunkStats[i].Sessions, testChunkStats[i].DifferentUrls.Count)));

            File.WriteAllLines("trainTestUrlTable.txt", urlTable.Select(x => x.Key + "\t" + x.Value));
            File.WriteAllLines("trainTestQueryTable.txt", queryTable.Select(x => x.Key + "\t" + x.Value));

            File.WriteAllLines("urlQueryStats.txt", new[]
                                                        {
                                                            string.Format("Different URLs: {0}", urlTable.Count)
                                                            , string.Format("Different queries: {0}", queryTable.Count)
                                                            , string.Format("Max URL: {0}", maxUrl)
                                                            , string.Format("Max query: {0}", maxQuery)
                                                        });
        }
    }
}