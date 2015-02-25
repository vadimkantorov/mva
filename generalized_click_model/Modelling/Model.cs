using System;
using System.Collections.Generic;
using System.Linq;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Factors;
using MicrosoftResearch.Infer.Models;

namespace Clik
{
    public class Model
    {
        Range userFeatures = new Range(Features.UserFeatures).Named("UserFeatures");
        Range urlFeatures = new Range(Features.UrlFeatures).Named("UrlFeatures");
        
        VariableArray<Gaussian> userPriorsA;
        VariableArray<Gaussian> userPriorsB;
        VariableArray<Gaussian> userPriorsR;
        VariableArray<double> userThetaA;
        VariableArray<double> userThetaB;
        VariableArray<double> userThetaR;
        Variable<double> userSumA;
        Variable<double> userSumB;
        Variable<double> userSumR;

        VariableArray<Gaussian>[] urlPriorsA;
        VariableArray<Gaussian>[] urlPriorsB;
        VariableArray<Gaussian>[] urlPriorsR;
        VariableArray<double>[] urlThetaA;
        VariableArray<double>[] urlThetaB;
        VariableArray<double>[] urlThetaR;
        Variable<double>[] urlSumA;
        Variable<double>[] urlSumB;
        Variable<double>[] urlSumR;
        Variable<double>[] A;
        Variable<double>[] B;
        Variable<double>[] R;
        
        Variable<bool>[] Click;
        Variable<bool>[] Examine;
        InferenceEngine InferenceEngine;

        public void LearnOneQuery(ModelParameters currentGuess, Tuple<int[][], int[]> feats, bool[] isClicked)
        {
            SetPriors(currentGuess, feats);
            for (int i = 0; i < Constants.ModelRanks; i++)
                Click[i].ObservedValue = isClicked[i];

            SetUrlPosteriors(feats.Item1, currentGuess);
            SetUserPosteriors(feats.Item2, currentGuess);
        }

        public Model()
        {
            InferenceEngine = new InferenceEngine(new ExpectationPropagation()) { ShowFactorGraph = false, ShowProgress = false };
            //InferenceEngine.Compiler.UseParallelForLoops = true;

            InitOtherVariables();
            InitThetaUrl();
            InitThetaUser();

            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                A[i] = Variable.GaussianFromMeanAndVariance((userSumA + urlSumA[i]).Named("A_"+i), 1).Named("Noised A_"+i);
                B[i] = Variable.GaussianFromMeanAndVariance((userSumB + urlSumB[i]).Named("B_" + i), 1).Named("Noised B_"+i);
                R[i] = Variable.GaussianFromMeanAndVariance((userSumR + urlSumR[i]).Named("R_"+i), 1).Named("Noised R_"+i);

                int prev = i - 1;
                if(i == 0)
                    Examine[i] = Always1().Named("E_"+i);
                else
                {
                    var nextIfClick = (A[prev] > 0).Named("A_"+prev + " > 0");
                    var nextIfNotClick = (B[prev] > 0).Named("B_"+prev + " > 0");
                    var noClick = (!Click[prev]).Named("No C_"+prev);
                    var next = ((noClick & nextIfNotClick) | (Click[prev] & nextIfClick));
                    Examine[i] = (Examine[prev] & next).Named("E_"+i);
                }
                var rg0 = (R[i] > 0).Named("R_" + i + " > 0");
                Click[i] = (Examine[i] & rg0).Named("C_"+i);
            }
        }

        void SetUserPosteriors(int[] userFeats, ModelParameters inferredParameters)
        {
            var thetaUserA_inferred = InferenceEngine.Infer<Gaussian[]>(userThetaA);
            var thetaUserB_inferred = InferenceEngine.Infer<Gaussian[]>(userThetaB);
            var thetaUserR_inferred = InferenceEngine.Infer<Gaussian[]>(userThetaR);

            for (int j = 0; j < Features.UserFeatures; j++)
            {
                int k = userFeats[j];
                inferredParameters.UserThetaA[j][k] = thetaUserA_inferred[j];
                inferredParameters.UserThetaB[j][k] = thetaUserB_inferred[j];
                inferredParameters.UserThetaR[j][k] = thetaUserR_inferred[j];
            }
        }

        void SetUserPriors(ModelParameters priors, int[] userFeats)
        {
            var priorThetaUserA_observed = new Gaussian[Features.UserFeatures];
            var priorThetaUserB_observed = new Gaussian[Features.UserFeatures];
            var priorThetaUserR_observed = new Gaussian[Features.UserFeatures];

            for (int j = 0; j < Features.UserFeatures; j++)
            {
                int k = userFeats[j];
                priorThetaUserA_observed[j] = priors.UserThetaA[j][k];
                priorThetaUserB_observed[j] = priors.UserThetaB[j][k];
                priorThetaUserR_observed[j] = priors.UserThetaR[j][k];
            }

            userPriorsA.ObservedValue = priorThetaUserA_observed;
            userPriorsB.ObservedValue = priorThetaUserB_observed;
            userPriorsR.ObservedValue = priorThetaUserR_observed;
        }

        void SetUrlPosteriors(int[][] urlFeats, ModelParameters inferredParameters)
        {
            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                var thetaUrlA_inferred = InferenceEngine.Infer<Gaussian[]>(urlThetaA[i]);
                var thetaUrlB_inferred = InferenceEngine.Infer<Gaussian[]>(urlThetaB[i]);
                var thetaUrlR_inferred = InferenceEngine.Infer<Gaussian[]>(urlThetaR[i]);
                for (int j = 0; j < Features.UrlFeatures; j++)
                {
                    int k = urlFeats[i][j];
                    inferredParameters.UrlThetaA[i][j][k] = thetaUrlA_inferred[j];
                    inferredParameters.UrlThetaB[i][j][k] = thetaUrlB_inferred[j];
                    inferredParameters.UrlThetaR[i][j][k] = thetaUrlR_inferred[j];
                }
            }
        }

        void SetUrlPriors(ModelParameters priors, int[][] urlFeats)
        {
            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                var priorThetaA_observed = new Gaussian[Features.UrlFeatures];
                var priorThetaB_observed = new Gaussian[Features.UrlFeatures];
                var priorThetaR_observed = new Gaussian[Features.UrlFeatures];
                for (int j = 0; j < Features.UrlFeatures; j++)
                {
                    int k = urlFeats[i][j];
                    priorThetaA_observed[j] = priors.UrlThetaA[i][j][k];
                    priorThetaB_observed[j] = priors.UrlThetaB[i][j][k];
                    priorThetaR_observed[j] = priors.UrlThetaR[i][j][k];
                }

                urlPriorsA[i].ObservedValue = priorThetaA_observed;
                urlPriorsB[i].ObservedValue = priorThetaB_observed;
                urlPriorsR[i].ObservedValue = priorThetaR_observed;
            }

        }

        public double[] GetClickProbabilities(ModelParameters pars, Tuple<int[][], int[]> feats)
        {
            SetPriors(pars, feats);

            var res = new double[Constants.ModelRanks];
            for (int i = 0; i < Constants.ModelRanks; i++)
                res[i] = InferenceEngine.Infer<Bernoulli>(Click[i]).GetProbTrue();
            return res;
        }

        void SetPriors(ModelParameters priors, Tuple<int[][], int[]> feats)
        {
            SetUrlPriors(priors, feats.Item1);
            SetUserPriors(priors, feats.Item2);
        }

        void InitOtherVariables()
        {
            A = new Variable<double>[Constants.ModelRanks];
            B = new Variable<double>[Constants.ModelRanks];
            R = new Variable<double>[Constants.ModelRanks];
            Examine = new Variable<bool>[Constants.ModelRanks];
            Click = new Variable<bool>[Constants.ModelRanks];
        }

        void InitThetaUrl()
        {
            urlPriorsA = new VariableArray<Gaussian>[Constants.ModelRanks];
            urlPriorsB = new VariableArray<Gaussian>[Constants.ModelRanks];
            urlPriorsR = new VariableArray<Gaussian>[Constants.ModelRanks];
            urlThetaA = new VariableArray<double>[Constants.ModelRanks];
            urlThetaB = new VariableArray<double>[Constants.ModelRanks];
            urlThetaR = new VariableArray<double>[Constants.ModelRanks];
            urlSumA = new Variable<double>[Constants.ModelRanks];
            urlSumB = new Variable<double>[Constants.ModelRanks];
            urlSumR = new Variable<double>[Constants.ModelRanks];

            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                urlPriorsA[i] = Variable.Array<Gaussian>(urlFeatures).Named("UrlPriorsA_"+i);
                urlPriorsB[i] = Variable.Array<Gaussian>(urlFeatures).Named("UrlPriorsB_" + i);
                urlPriorsR[i] = Variable.Array<Gaussian>(urlFeatures).Named("UrlPriorsR_" + i);
                urlThetaA[i] = Variable.Array<double>(urlFeatures).Named("UrlThetaA_" + i);
                urlThetaB[i] = Variable.Array<double>(urlFeatures).Named("UrlThetaB_" + i);
                urlThetaR[i] = Variable.Array<double>(urlFeatures).Named("UrlThetaR_" + i);

                urlThetaA[i][urlFeatures] = Variable.Random<double, Gaussian>(urlPriorsA[i][urlFeatures]);
                urlThetaB[i][urlFeatures] = Variable.Random<double, Gaussian>(urlPriorsB[i][urlFeatures]);
                urlThetaR[i][urlFeatures] = Variable.Random<double, Gaussian>(urlPriorsR[i][urlFeatures]);

                urlSumA[i] = Variable.Sum(urlThetaA[i]).Named("UrlSumA_"+i);
                urlSumB[i] = Variable.Sum(urlThetaB[i]).Named("UrlSumB_" + i);
                urlSumR[i] = Variable.Sum(urlThetaR[i]).Named("UrlSumR_" + i);
            }
        }

        void InitThetaUser()
        {
            userPriorsA = Variable.Array<Gaussian>(userFeatures).Named("UserPriorsA");
            userThetaA = Variable.Array<double>(userFeatures).Named("UserThetaA");
            userThetaA[userFeatures] = Variable.Random<double, Gaussian>(userPriorsA[userFeatures]);
            userSumA = Variable.Sum(userThetaA).Named("UserSumA");

            userPriorsB = Variable.Array<Gaussian>(userFeatures).Named("UserPriorsB");
            userThetaB = Variable.Array<double>(userFeatures).Named("UserThetaB");
            userThetaB[userFeatures] = Variable.Random<double, Gaussian>(userPriorsB[userFeatures]);
            userSumB = Variable.Sum(userThetaB).Named("UserSumB");

            userPriorsR = Variable.Array<Gaussian>(userFeatures).Named("UserPriorsR");
            userThetaR = Variable.Array<double>(userFeatures).Named("UserThetaR");
            userThetaR[userFeatures] = Variable.Random<double, Gaussian>(userPriorsR[userFeatures]);
            userSumR = Variable.Sum(userThetaR).Named("UserSumR");
        }

        static Variable<bool> Always1()
        {
            return Variable.Bernoulli(1);
        }
    }
}