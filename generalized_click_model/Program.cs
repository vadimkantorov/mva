using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Models;

namespace Clik
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(@"D:\MVA\Graphical models\Final project\src\Clik\bin\x64\Debug");

            //new UrlQueryStats().Run();

            //new TrainModel().Run(777);
            
            new TestModel().Run();
            
            //Parallel.For(0, SplitDatasetByChunks.ChunkCount, i => new TrainModel().Run(i));
            //RelevanceLookup.Construct(new RelevanceLogReader(Constants.RelevanceLogPath).Read());
            //ExaminationTimeLookup.Construct(new ClickLogReader("trainClicks.txt").ReadSessions().SelectMany(x => x.Queries));
        }
    }
}
