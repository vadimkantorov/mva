using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Clik
{
    public class TestModel
    {
        ModelPerformance LogLikelihoodAndPerplexity(IEnumerable<Query> queries, ModelParameters pars)
        {
            double[] p = new double[Constants.ModelRanks];
            var actualClicks = new int[Constants.ModelRanks];
            var modelledCtr = new double[Constants.ModelRanks];

            double logLikelihood = 0;
            int z = 0;
            var sw = Stopwatch.StartNew();
            foreach (var q in queries)
            {
                if (z % 100000 == 0)
                {
                    var ll = logLikelihood / (z*Constants.ModelRanks);
                    var pp = p.Select(x => Math.Pow(2, x / z)).Average();
                
                    Console.WriteLine("{0} queries ({1} minutes). LL: {2}, PP: {3}", z, sw.Elapsed.TotalMinutes, ll, pp);
                }
                z++;

                var feats = extractor.ExtractFeatures(q);
                var predictedCtr = model.GetClickProbabilities(pars, feats);

                for (int i = 0; i < Constants.ModelRanks; i++)
                {
                    double c = predictedCtr[i];
                    var add = q.IsClicked[i] ? c : 1 - c;

                    p[i] += Math.Log(add, 2);
                    logLikelihood += Math.Log(add);
                    
                    actualClicks[i] += q.IsClicked[i] ? 1 : 0;
                    modelledCtr[i] += c;
                }
            }

            var totalImps = z*Constants.ModelRanks;

            for (int i = 0; i < Constants.ModelRanks; i++)
                p[i] = Math.Pow(2, -p[i] / totalImps);

            logLikelihood /= totalImps;
            var perplexity = p.Average();

            var positionalCtr = Tuple.Create(actualClicks.Select(x => (double) x/z).ToArray(),
                                             modelledCtr.Select(x => (double) x/z).ToArray());

            return new ModelPerformance
                       {LogLikelihood = logLikelihood, Perplexity = perplexity, PositionalPerplexity = p, PositionalCtr = positionalCtr};
        }
        
        public void Run()
        {
            var strs = new List<string>();
            //for (int i = SplitDatasetByChunks.ChunkCount-1; i >= 0; i--)
            var i = 777;
            {
                Console.WriteLine("Starting on chunk {0}", i);

                var pars = ModelParameters.Load(string.Format("processed_dataset/{0}_model_5.txt", i));
                var reader = new ClickLogReader(string.Format("processed_dataset/{0}_testClicks.txt", i));
                var qs = reader.ReadQueries();
                var res = LogLikelihoodAndPerplexity(qs, pars);

                strs.Add(i.ToString());
                strs.Add(res.ToString());

                Console.WriteLine(strs.Last());
                File.WriteAllLines("testStats.txt", strs);
            }
        }

        readonly Features extractor = new Features();
        readonly Model model = new Model();
    }
}