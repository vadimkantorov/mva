using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Clik
{
    public class ClickLogStats
    {
        public void Run()
        {
            Dictionary<int, int>[] queriesByRegion = Enumerable.Range(0, Constants.Regions).Select(x => new Dictionary<int, int>()).ToArray();
            var justByRegion = new int[Constants.Regions];
            var queries = new Dictionary<int, int>();
            
            var rdr = new ClickLogReader(Constants.ClickLogPath);

            int z = 0;
            var sw = Stopwatch.StartNew();
            foreach(var q in rdr.ReadQueries())
            {
                if (z % 1000000 == 0) Console.WriteLine("{0} queries ({1} minutes)", z, sw.Elapsed.TotalMinutes); z++;

                justByRegion[q.RegionId]++;
                queriesByRegion[q.RegionId].Inc(q.QueryId);
                queries.Inc(q.QueryId);
            }

            File.WriteAllLines("regionStats.txt", justByRegion.Select((x, i) => string.Format("Region {0}:\t{1} impressions ({2} queries)", i, x, queriesByRegion[i].Count)));
            File.WriteAllLines("queries_just.txt", queries.OrderByDescending(x => x.Value).Select(x => string.Format(x.Value + "\t" + x.Key)));
        }
    }
}