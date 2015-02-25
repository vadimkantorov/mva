using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Clik
{
    public class SplitDatasetByChunks
    {
        public const int ChunkCount = 9;
        const int MinHitsInChunk = 500000;

        static bool GoodForChunk(int freq, int chunk)
        {
            const int K = 1000;
            switch (chunk)
            {
                case 0:
                    return 500*K < freq && freq <= 1000*K;
                case 1:
                    return 100 * K < freq && freq <= 500 * K;
                case 2:
                    return 50 * K < freq && freq <= 100 * K;
                case 3:
                    return 10 * K < freq && freq <= 50 * K;
                case 4:
                    return 5 * K < freq && freq <= 10 * K;
                case 5:
                    return 1 * K < freq && freq <= 5 * K;
                case 6:
                    return 500 < freq && freq <= 1 * K;
                case 7:
                    return 100 < freq && freq <= 500;
                case 8:
                    return 0 < freq && freq <= 100;
            }
            return false;
        }
        
        public void Run()
        {
            var queriesJust = File
                .ReadAllLines("queries_just.txt")
                .Select(x => x.Split().Select(int.Parse).ToArray())
                .Select(x => new { QueryId = x[1], Frequency = x[0] }).ToArray();

            var freqByQuery = queriesJust.ToDictionary(x => x.QueryId, x => x.Frequency);

            queriesJust = queriesJust.Skip(2).ToArray();

            var chunksByQuery = new Dictionary<int, int>();
            var queriesByChunk = Enumerable.Range(0, ChunkCount).Select(_ => new List<int>()).ToArray();
            var chunkHits = new int[ChunkCount];
            int currentChunk = 0;
            
            for(int i = 0; i < queriesJust.Length; i++)
            {
                int freq = queriesJust[i].Frequency;
                int queryId = queriesJust[i].QueryId;

                if(!GoodForChunk(freq, currentChunk))
                    continue;

                queriesByChunk[currentChunk].Add(queryId);
                chunksByQuery[queryId] = currentChunk;
                chunkHits[currentChunk] += freq/2;

                if (chunkHits[currentChunk] > MinHitsInChunk)
                    currentChunk++;
                if(currentChunk >= ChunkCount)
                    break;
            }

            File.WriteAllLines("chunkStats.txt", queriesByChunk.Select(x => string.Join("\t",x.Select(y => freqByQuery[y]))));
            Console.WriteLine("Built chunk lists");
            
            var qs = new ClickLogReader(Constants.ClickLogPath).ReadQueries();

            var trainWriter = Enumerable.Range(0, ChunkCount).Select(i => new ClickLogWriter(string.Format("processed_dataset/{0}_trainClicks.txt", i))).ToArray();
            var testWriter = Enumerable.Range(0, ChunkCount).Select(i => new ClickLogWriter(string.Format("processed_dataset/{0}_testClicks.txt", i))).ToArray();

            var trainBuffer = Enumerable.Range(0, ChunkCount).Select(_ => new List<Session>()).ToArray();
            var testBuffer = Enumerable.Range(0, ChunkCount).Select(_ => new List<Session>()).ToArray();

            Action<bool> flush = dispose =>
            {
                for (int i = 0; i < ChunkCount; i++)
                {
                    if (dispose || trainBuffer[i].Count > 10000)
                    {
                        trainWriter[i].WriteSessions(trainBuffer[i]);
                        trainBuffer[i].Clear();
                    }

                    if (dispose || testBuffer[i].Count > 10000)
                    {
                        testWriter[i].WriteSessions(testBuffer[i]);
                        testBuffer[i].Clear();
                    }

                    if (dispose)
                    {
                        trainWriter[i].Dispose();
                        testWriter[i].Dispose();
                    }
                }
            };


            int z = 0;
            var sw = Stopwatch.StartNew();
            int sessionId = 0;
            var queries = new Dictionary<int, int>();
            foreach (var q in qs)
            {
                if (z % 1000000 == 0) Console.WriteLine("{0} queries ({1} minutes)", z, sw.Elapsed.TotalMinutes); z++;

                if(!chunksByQuery.ContainsKey(q.QueryId))
                    continue;

                var chunk = chunksByQuery[q.QueryId];
                queries.Inc(q.QueryId);
                bool train = queries[q.QueryId] % 2 == 1;

                if (train)
                    trainBuffer[chunk].Add(new Session { SessionId = sessionId, Queries = new[] { q } });
                else
                    testBuffer[chunk].Add(new Session { SessionId = sessionId, Queries = new[] { q } });
                sessionId++;

                flush(false);
            }

            flush(true);
        }
    }
}