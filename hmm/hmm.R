library(ggplot2)
library(ellipse)

source("em.r")
source("em2.r")
source("forwardBackward.r")
source("viterbi.r")

setwd("Z:/MVA/Graphical models/Assignment 3/src/")
data = as.matrix(read.table("../EMGaussienne.data", col.names=c("x","y")))
testData = as.matrix(read.table("../EMGaussienne.test", col.names=c("x","y")))
#data = testData

N = nrow(data)
K = 4

dmnorm = function(x, mu, sigma)
    1/(2*pi*sqrt(det(sigma)))*exp(-1/2*t(x-mu) %*% solve(sigma) %*% (x-mu))
aug = function(params) within(params, {
  emissionProb = function(x, k) dmnorm(x, params$mu[k, ], params$sigma[[k]])
  K = K
  if(exists('a') == F)
    a = matrix(1 / K, nrow=K, ncol=K)
  })

likelihood = function(data, params)
{
  scalers = forwardBackward(data, params)$alphaScalers
  sum(log(scalers))
}

params = aug(hmm_estimateParametersInitially(data, N, K))
params = hmm_estimateParameters(data, params)

trainMostProbable = viterbi(data, params)
trainLikelihood = likelihood(data, params)

testMostProbable = viterbi(testData, params)
testLikelihood = likelihood(testData, params)

qplot(x,y,data=as.data.frame(cbind(data, trainMostProbable)), col=factor(trainMostProbable)) +
  scale_color_discrete(name="Clusters")

qplot(x,y,data=as.data.frame(cbind(testData, testMostProbable)), col=factor(testMostProbable)) +
  scale_color_discrete(name="Clusters")