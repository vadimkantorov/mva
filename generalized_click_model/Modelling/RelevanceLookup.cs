using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clik
{
    public class RelevanceLookup
    {
        public static void Construct(Judgement[] jds)
        {
            var dic = jds
                .GroupBy(x => Key(x.QueryId, x.UrlId))
                .ToDictionary(g => g.Key, g => g.Average(x => (double)x.RelevanceLabel));
            File.WriteAllLines(dicPath, dic.Select(x => x.Key + "\t" + x.Value));
        }
        
        public RelevanceLookup()
        {
            dic = File.ReadAllLines(dicPath).Select(x => x.Split()).ToDictionary(x => ulong.Parse(x[0]),
                                                                                 x => double.Parse(x[1]));
        }

        public double? Lookup(int queryId, int urlId)
        {
            var key = Key(queryId,urlId);
            double res;
            if (!dic.TryGetValue(key, out res))
                return null;
            return res;
        }

        static ulong Key(int queryId, int urlId)
        {
            return (ulong)urlId * 100000000L + (uint)queryId;
        }

        readonly Dictionary<ulong, double> dic;
        const string dicPath = "relevanceLookup.txt";
    }
}