using System;
using System.Diagnostics;
using System.Linq;

namespace Clik
{
    public class TrainModel
    {
        public void Run(int i)
        {
            var extractor = new Features();
            var model = new Model();

            //for (int i = SplitDatasetByChunks.ChunkCount - 1; i >= 0; i--)
            {
                Console.WriteLine("Starting on chunk {0}", i);
                var rdr = new ClickLogReader(string.Format("processed_dataset/{0}_trainClicks.txt", i));
                var currentParameters = new ModelParameters();
                
                Action dumpModel = () =>
                {
                    Console.WriteLine("Saved model");
                    currentParameters.Save(string.Format("processed_dataset/{0}_model_5.txt", i));
                };

                int z = 0;
                var sw = Stopwatch.StartNew();
                foreach (var q in rdr.ReadQueries())
                {
                    if (z % 10000 == 0) Console.WriteLine("Queries: {0} ({1} minutes)", z, sw.Elapsed.TotalMinutes); z++;
                    if (z % 1000000 == 0)
                        dumpModel();

                    var feats = extractor.ExtractFeatures(q);
                    model.LearnOneQuery(currentParameters, feats, q.IsClicked);

                }

                dumpModel();
            }
        }
    }
}