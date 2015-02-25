using System;
using System.Text;

namespace Clik
{
    class ModelPerformance
    {
        public double LogLikelihood;
        public double Perplexity;
        public double[] PositionalPerplexity;
        public Tuple<double[], double[]> PositionalCtr;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Log-likelihood: {0}", LogLikelihood).AppendLine();
            sb.AppendFormat("Perplexity: {0}", Perplexity).AppendLine();

            for (int i = 0; i < Constants.ModelRanks; i++)
                sb.AppendFormat("Perplexity @ {0} = {1}", i, PositionalPerplexity[i]).AppendLine();

            for (int i = 0; i < Constants.ModelRanks; i++)
                sb.AppendFormat("Positional CTR @ {0}. Actual: {1}\tPredicted: {2}", i, PositionalCtr.Item1[i],
                                PositionalCtr.Item2[i]).AppendLine();

                return sb.ToString();
        }
    }
}