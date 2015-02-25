using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MicrosoftResearch.Infer.Distributions;

namespace Clik
{
    public class ModelParameters
    {
        public Gaussian[][][] UrlThetaA = new Gaussian[Constants.ModelRanks][][];
        public Gaussian[][][] UrlThetaB = new Gaussian[Constants.ModelRanks][][];
        public Gaussian[][][] UrlThetaR = new Gaussian[Constants.ModelRanks][][];
        public Gaussian[][] UserThetaA = new Gaussian[Features.UserFeatures][];
        public Gaussian[][] UserThetaB = new Gaussian[Features.UserFeatures][];
        public Gaussian[][] UserThetaR = new Gaussian[Features.UserFeatures][];

        public static void TestLoadSave()
        {
            var mp = new ModelParameters();
            var rnd = new Random();

            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                for (int j = 0; j < Features.UrlFeatures; j++)
                {
                    for (int k = 0; k < Features.UrlBins[j]; k++)
                    {
                        mp.UrlThetaA[i][j][k].SetMeanAndVariance(rnd.Next(0, 31337), rnd.Next(0, 31337));
                        mp.UrlThetaB[i][j][k].SetMeanAndVariance(rnd.Next(0, 31337), rnd.Next(0, 31337));
                        mp.UrlThetaR[i][j][k].SetMeanAndVariance(rnd.Next(0, 31337), rnd.Next(0, 31337));
                    }
                }
            }

            for (int j = 0; j < Features.UserFeatures; j++)
            {
                for (int k = 0; k < Features.UserBins[j]; k++)
                {
                    mp.UserThetaA[j][k].SetMeanAndVariance(rnd.Next(0, 31337), rnd.Next(0, 31337));
                    mp.UserThetaB[j][k].SetMeanAndVariance(rnd.Next(0, 31337), rnd.Next(0, 31337));
                    mp.UserThetaR[j][k].SetMeanAndVariance(rnd.Next(0, 31337), rnd.Next(0, 31337));
                }
            }

            mp.Save("LoadSaveTest.txt");

            var lp = ModelParameters.Load("LoadSaveTest.txt");

            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                for (int j = 0; j < Features.UrlFeatures; j++)
                {
                    for (int k = 0; k < Features.UrlBins[j]; k++)
                    {
                        Debug.Assert((int)Math.Round(mp.UrlThetaA[i][j][k].GetMean()) == (int)Math.Round(lp.UrlThetaA[i][j][k].GetMean()));
                        Debug.Assert((int)Math.Round(mp.UrlThetaA[i][j][k].GetVariance()) == (int)Math.Round(lp.UrlThetaA[i][j][k].GetVariance()));

                        Debug.Assert((int)Math.Round(mp.UrlThetaB[i][j][k].GetMean()) == (int)Math.Round(lp.UrlThetaB[i][j][k].GetMean()));
                        Debug.Assert((int)Math.Round(mp.UrlThetaB[i][j][k].GetVariance()) == (int)Math.Round(lp.UrlThetaB[i][j][k].GetVariance()));

                        Debug.Assert((int)Math.Round(mp.UrlThetaR[i][j][k].GetMean()) == (int)Math.Round(lp.UrlThetaR[i][j][k].GetMean()));
                        Debug.Assert((int)Math.Round(mp.UrlThetaR[i][j][k].GetVariance()) == (int)Math.Round(lp.UrlThetaR[i][j][k].GetVariance()));
                    }
                }
            }

            for (int j = 0; j < Features.UserFeatures; j++)
            {
                for (int k = 0; k < Features.UserBins[j]; k++)
                {
                    Debug.Assert((int)Math.Round(mp.UserThetaA[j][k].GetMean()) == (int)Math.Round(lp.UserThetaA[j][k].GetMean()));
                    Debug.Assert((int)Math.Round(mp.UserThetaA[j][k].GetVariance()) == (int)Math.Round(lp.UserThetaA[j][k].GetVariance()));

                    Debug.Assert((int)Math.Round(mp.UserThetaB[j][k].GetMean()) == (int)Math.Round(lp.UserThetaB[j][k].GetMean()));
                    Debug.Assert((int)Math.Round(mp.UserThetaB[j][k].GetVariance()) == (int)Math.Round(lp.UserThetaB[j][k].GetVariance()));

                    Debug.Assert((int)Math.Round(mp.UserThetaR[j][k].GetMean()) == (int)Math.Round(lp.UserThetaR[j][k].GetMean()));
                    Debug.Assert((int)Math.Round(mp.UserThetaR[j][k].GetVariance()) == (int)Math.Round(lp.UserThetaR[j][k].GetVariance()));
                }
            }
        }
        
        public static ModelParameters Load(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var urlMeans = lines[0].Split('\t').Select(double.Parse).ToArray();
            var urlVars = lines[1].Split('\t').Select(double.Parse).ToArray();

            var userMeans = lines[2].Split('\t').Select(double.Parse).ToArray();
            var userVars = lines[3].Split('\t').Select(double.Parse).ToArray();

            int z = 0;
            var pars = new ModelParameters();

            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                for (int j = 0; j < Features.UrlFeatures; j++)
                {
                    for (int k = 0; k < Features.UrlBins[j]; k++)
                    {
                        pars.UrlThetaA[i][j][k].SetMeanAndVariance(urlMeans[z], urlVars[z]); z++;
                        pars.UrlThetaB[i][j][k].SetMeanAndVariance(urlMeans[z], urlVars[z]); z++;
                        pars.UrlThetaR[i][j][k].SetMeanAndVariance(urlMeans[z], urlVars[z]); z++;
                    }
                }
            }

            z = 0;
            for (int j = 0; j < Features.UserFeatures; j++)
            {
                for (int k = 0; k < Features.UserBins[j]; k++)
                {
                    pars.UserThetaA[j][k].SetMeanAndVariance(userMeans[z], userVars[z]); z++;
                    pars.UserThetaB[j][k].SetMeanAndVariance(userMeans[z], userVars[z]); z++;
                    pars.UserThetaR[j][k].SetMeanAndVariance(userMeans[z], userVars[z]); z++;
                }
            }

            return pars;
        }

        public void Save(string filePath)
        {
            var urlMeans = new List<double>();
            var urlVars = new List<double>();

            var userMeans = new List<double>();
            var userVars = new List<double>();

            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                for(int j = 0; j < Features.UrlFeatures; j++)
                {
                    for(int k = 0; k < Features.UrlBins[j]; k++)
                    {
                        urlMeans.Add(UrlThetaA[i][j][k].GetMean());
                        urlMeans.Add(UrlThetaB[i][j][k].GetMean());
                        urlMeans.Add(UrlThetaR[i][j][k].GetMean());

                        urlVars.Add(UrlThetaA[i][j][k].GetVariance());
                        urlVars.Add(UrlThetaB[i][j][k].GetVariance());
                        urlVars.Add(UrlThetaR[i][j][k].GetVariance());
                    }
                }
            }

            for (int j = 0; j < Features.UserFeatures; j++)
            {
                for (int k = 0; k < Features.UserBins[j]; k++)
                {
                    userMeans.Add(UserThetaA[j][k].GetMean());
                    userMeans.Add(UserThetaB[j][k].GetMean());
                    userMeans.Add(UserThetaR[j][k].GetMean());

                    userVars.Add(UserThetaA[j][k].GetVariance());
                    userVars.Add(UserThetaB[j][k].GetVariance());
                    userVars.Add(UserThetaR[j][k].GetVariance());
                }
            }

            File.WriteAllLines(filePath, new[] {
                string.Join("\t", urlMeans), 
                string.Join("\t", urlVars),
                string.Join("\t", userMeans), 
                string.Join("\t", userVars)
            });
        }

        public ModelParameters()
        {
            double priorThetaMean = 0;
            double priorThetaVariance = 1.0 / Features.TotalFeatures;

            for (int i = 0; i < Constants.ModelRanks; i++)
            {
                UrlThetaA[i] = new Gaussian[Features.UrlFeatures][];
                UrlThetaB[i] = new Gaussian[Features.UrlFeatures][];
                UrlThetaR[i] = new Gaussian[Features.UrlFeatures][];
                for (int j = 0; j < Features.UrlFeatures; j++)
                {
                    UrlThetaA[i][j] = new Gaussian[Features.UrlBins[j]];
                    UrlThetaB[i][j] = new Gaussian[Features.UrlBins[j]];
                    UrlThetaR[i][j] = new Gaussian[Features.UrlBins[j]];
                    
                    for (int k = 0; k < Features.UrlBins[j]; k++)
                    {
                        UrlThetaA[i][j][k] = Gaussian.FromMeanAndVariance(priorThetaMean, priorThetaVariance);
                        UrlThetaB[i][j][k] = Gaussian.FromMeanAndVariance(priorThetaMean, priorThetaVariance);
                        UrlThetaR[i][j][k] = Gaussian.FromMeanAndVariance(priorThetaMean, priorThetaVariance);
                    }
                }
            }

            for (int j = 0; j < Features.UserFeatures; j++)
            {
                UserThetaA[j] = new Gaussian[Features.UserBins[j]];
                UserThetaB[j] = new Gaussian[Features.UserBins[j]];
                UserThetaR[j] = new Gaussian[Features.UserBins[j]];

                for (int k = 0; k < Features.UserBins[j]; k++)
                {
                    UserThetaA[j][k] = Gaussian.FromMeanAndVariance(priorThetaMean, priorThetaVariance);
                    UserThetaB[j][k] = Gaussian.FromMeanAndVariance(priorThetaMean, priorThetaVariance);
                    UserThetaR[j][k] = Gaussian.FromMeanAndVariance(priorThetaMean, priorThetaVariance);
                }
            }
        }
    }
}