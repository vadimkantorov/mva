using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Clik
{
    public class SplitDatasetInTrainTest
    {
        static IEnumerable<string> ReadLines(string filePath, int take)
        {
            var res = new string[take];
            using (var sr = new StreamReader(filePath))
            {
                for (int i = 0; i < take; i++ )
                    res[i] = sr.ReadLine();
            }
            return res;
        }

        public void Run()
        {
            var queriesJust = ReadLines("queries_just.txt", 5002).Skip(2)
                .Select(x => x.Split().Select(int.Parse).ToArray())
                .Select(x => new { QueryId = x[1], Frequency = x[0] });

            var queries = new HashSet<int>(queriesJust.Select(x => x.QueryId));
            var dic = new Dictionary<int, int>();

            var reader = new ClickLogReader(Constants.ClickLogPath);
            var trainWriter = new ClickLogWriter("processed_dataset/777_trainClicks.txt");
            var testWriter = new ClickLogWriter("processed_dataset/777_testClicks.txt");

            var trainBuffer = new List<Session>();
            var testBuffer = new List<Session>();

            Action<bool> flush = dispose =>
            {
                if (dispose || trainBuffer.Count > 10000)
                {
                    trainWriter.WriteSessions(trainBuffer);
                    trainBuffer.Clear();
                }

                if (dispose || testBuffer.Count > 10000)
                {
                    testWriter.WriteSessions(testBuffer);
                    testBuffer.Clear();
                }

                if (dispose)
                {
                    trainWriter.Dispose();
                    testWriter.Dispose();
                }
            };

            int sessionId = 0;
            int z = 0;
            var sw = Stopwatch.StartNew();
            foreach(var q in reader.ReadQueries())
            {
                if (z % 1000000 == 0) Console.WriteLine("{0} queries ({1} minutes)", z, sw.Elapsed.TotalMinutes); z++;

                if(!queries.Contains(q.QueryId))
                    continue;

                dic.Inc(q.QueryId);
                var buf = dic[q.QueryId] % 2 == 1 ? trainBuffer : testBuffer;
                buf.Add(new Session {SessionId = sessionId, Queries = new[] {q}});
                sessionId++;

                flush(false);
            }
            flush(true);
        }
    }
}