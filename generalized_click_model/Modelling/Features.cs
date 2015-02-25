using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clik
{
    public class Features
    {
        static int TimePassedFeat(int timePassed)
        {
            for (int i = 0; i < timePassedBins.Length; i++)
                if (timePassed < timePassedBins[i])
                    return i;
            return timePassedBins.Length;
        }

        const int RelevanceBins = 3;
        int RelevanceFeat(int queryId, int urlId)
        {
            double? rel = relevanceLookup.Lookup(queryId, urlId);
            if (rel == null)
                return 0;
            if (rel < 0.5)
                return 1;
            return 2;
        }

        public Tuple<int[][], int[]> ExtractFeatures(Query q)
        {
            var userFeatures = new int[UserFeatures];
            userFeatures[0] = queryTable[q.QueryId];
            userFeatures[1] = q.RegionId;

            var urlFeatures = new int[Constants.Ranks][];
            for(int i = 0; i < Constants.Ranks; i++)
            {
                var urlId = q.URLs[i];
                var relevanceFeat = RelevanceFeat(q.QueryId, urlId);
                var timePassedFeat = TimePassedFeat(examinationTimeLookup.Lookup(urlId));
                var positionFeat = i;
                var urlFeat = urlTable[urlId];

                urlFeatures[i] = new[] { relevanceFeat, timePassedFeat, positionFeat, urlFeat };
            }
            return Tuple.Create(urlFeatures, userFeatures);
        }

        public Features()
        {
            relevanceLookup = new RelevanceLookup();
            examinationTimeLookup = new ExaminationTimeLookup();
            urlTable = File.ReadAllLines("trainTestUrlTable.txt").Select(x => x.Split().Select(int.Parse).ToArray()).ToDictionary(x => x[0], x => x[1]);
            queryTable = File.ReadAllLines("trainTestQueryTable.txt").Select(x => x.Split().Select(int.Parse).ToArray()).ToDictionary(x => x[0], x => x[1]);
        }

        const int TotalDifferentUrls = 787153+100;
        const int TotalDifferentQueries = 14980+100;

        static readonly int[] timePassedBins = new[] {100, 300, 500, 1000};
        static int TotalTimePassedBins = timePassedBins.Length + 1;
        const int TotalRegionBins = 4;
        
        public static int[] UrlBins = new[] { RelevanceBins, TotalTimePassedBins, Constants.Ranks, TotalDifferentUrls };
        public static int UrlFeatures = UrlBins.Length;

        public static int[] UserBins = new[] { TotalDifferentQueries, TotalRegionBins };
        public static int UserFeatures = UserBins.Length;
        
        public static int TotalFeatures = UrlFeatures + UserFeatures;
        readonly Dictionary<int, int> urlTable;
        readonly Dictionary<int, int> queryTable;
        readonly RelevanceLookup relevanceLookup;
        readonly ExaminationTimeLookup examinationTimeLookup;

        /*Different URLs: 16762
        Different queries: 1536
        Max URL: 128407121
        Max query: 24195202*/

        /*Chunk 0:
        Different URLs: 2105
        Different queries: 162
        Max URL: 119408632
        Max query: 11255673*/
    }
}