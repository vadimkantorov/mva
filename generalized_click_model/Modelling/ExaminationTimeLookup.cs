using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Clik
{
    public class ExaminationTimeLookup
    {
        public ExaminationTimeLookup()
        {
            dic =
                File.ReadAllLines(DicPath).Select(x => x.Split().Select(int.Parse).ToArray()).ToDictionary(x => x[0], x => x[1]);
            AverageExaminationTime = (int)dic.Values.Average();
        }

        public readonly int AverageExaminationTime;

        public int Lookup(int urlId)
        {
            int res;
            if(!dic.TryGetValue(urlId, out res))
                return AverageExaminationTime;
            return res;
        }

        public static void Construct(IEnumerable<Query> qs)
        {
            var urlExaminationTime = new Dictionary<int, int>();
            var urlCnt = new Dictionary<int, int>();

            var oneToTen = Enumerable.Range(0, Constants.Ranks).ToArray();
            int z = 0;
            var sw = Stopwatch.StartNew();
            foreach(var q in qs)
            {
                if (z % 100000 == 0) Console.WriteLine("{0} queries ({1} minutes)", z, sw.Elapsed.TotalMinutes); z++;
                var inds = oneToTen.OrderBy(i => q.ClickTimePassed[i]).ToArray();
                for(int i = 0; i < Constants.Ranks; i++)
                {
                    if(q.ClickTimePassed[inds[i]] == -1)
                        continue;
                    if (i == 0 || q.ClickTimePassed[inds[i-1]] == -1)
                        continue;

                    var url = q.URLs[inds[i - 1]];
                    var diff = q.ClickTimePassed[inds[i]] - q.ClickTimePassed[inds[i - 1]];
                    if(diff < 0) throw new Exception("Diff cannot be negative");

                    urlCnt.Inc(url);
                    urlExaminationTime.Inc(url, diff);
                }
            }

            File.WriteAllLines(DicPath, urlCnt.Select(x => x.Key + "\t" + (int)Math.Floor((double)urlExaminationTime[x.Key] / x.Value)));
        }

        const string DicPath = "examinationTimeLookup.txt";
        readonly Dictionary<int, int> dic = new Dictionary<int, int>();
    }
}