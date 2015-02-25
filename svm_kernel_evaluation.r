library(ROCR)
library(e1071)
library(ggplot2)
library(reshape)
setwd('Z:/MVA/Kernel methods/TD/TD3')

data = as.matrix(read.table('./data/movement_libras.data', sep=','))
x = data[,-ncol(data)]
y = factor(data[,ncol(data)])
n = nrow(x)

nTrain = 0.8*n
trainInd = sample(1:n, nTrain)
trainx = x[trainInd,]
trainy = y[trainInd]
testx = x[-trainInd,]
testy = y[-trainInd]

res = data.frame()
acc = function(svmModel, xs, ys) mean(predict(svmModel, xs) == ys)
for(kernel in c('linear', 'polynomial', 'radial'))
{
  Cs = 10^seq(-5,2,by=0.01)
  clen = length(Cs)
  trainAcc = rep(0, clen)
  testAcc = rep(0, clen)
  for(i in 1:clen)
  {
    C = Cs[i]
    svmModel = svm(trainx, trainy, kernel=kernel, cost = C)
    trainAcc[i] = acc(svmModel, trainx, trainy)
    testAcc[i] = acc(svmModel,testx, testy)
  }
  res = rbind(res, data.frame(Cs, trainAcc, testAcc, kernel))
}

res = melt(res, id=c("kernel","Cs"))

qplot(data = res, x=Cs, y=value, facets = . ~ kernel, col=variable, geom='line', xlab='C', ylab='Accuracy', ylim=c(0,1)) +
  scale_x_log10() +
  scale_color_discrete(name='', labels=c('Train', 'Test'), breaks=c('trainAcc', 'testAcc'))