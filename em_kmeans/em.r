library(ggplot2)
library(ellipse)
setwd("D:/MVA/Graphical models/Assignment 2/")
data = as.matrix(read.table("EMGaussienne.data", col.names=c("x","y")))
testData = as.matrix(read.table("EMGaussienne.test", col.names=c("x","y")))
#data = testData
source("kmeans.r")

N = nrow(data)
K = 4
circleCovariances = T

dmnorm = function(x, mu, sigma)
  1/(2*pi*sqrt(det(sigma)))*exp(-1/2*t(x-mu) %*% solve(sigma) %*% (x-mu))

estimateParameters = function(gamma)
{
  Nk = colSums(gamma)
  pi_new = Nk/N
  mu_new = crossprod(gamma, data) / Nk
  
  op = function(v) if(circleCovariances) 1/2*as.numeric(t(v) %*% v)*diag(2) else v %*% t(v)
  
  sigma_new = lapply(1:K, function(k) 
    Reduce("+", lapply(1:N, function(n) gamma[n,k]/Nk[k]*op(data[n,] - mu_new[k,])))
  )  
  list(pi = pi_new, mu = mu_new, sigma = sigma_new)  
}

gamma_numers = function(params, n)
  sapply(1:K, function(k) params$pi[k]*dmnorm(data[n,], params$mu[k,], params$sigma[[k]]))

estimateGamma = function(params)
{
  gamma = matrix(nrow=N, ncol=K)
  for(n in 1:N)
  {
    gamma[n,] = gamma_numers(params, n)
    gamma[n,] = gamma[n,] / sum(gamma[n,])
  }
  gamma
}

estimateParametersInitially = function(assignment)
{
  gamma = matrix(0, N, K)
  for(n in 1:N)
    gamma[n, assignment[n]] = 1
  estimateParameters(gamma)
}

likelihood = function(params)
  sum(sapply(1:N, function(n) log(sum(gamma_numers(params, n)))))

clustering = kmeans(data, N, K)
qplot(x,y,data=data.frame(data, assignment=clustering$assignment), col=factor(assignment), main=sprintf("Loss = %.2f",clustering$loss)) +
  geom_point(aes(x, y), data=clustering$centroids, col="red", size = 7) +
  scale_color_discrete(name="Clusters")

params = estimateParametersInitially(clustering$assignment)
iterations = 100
likelihoods = c()
for(i in 1:iterations)
{
  gamma = estimateGamma(params)
  params = estimateParameters(gamma)
  
  #likelihood_ = sum(sapply(1:N, function(n) gamma[n, ] %*% log(gamma_numers(params, n))))
  likelihoods = c(likelihoods, likelihood(params))
}
data = testData
gamma = estimateGamma(params)
print(likelihood(params))
assignment = sapply(1:N, function(n) order(-gamma[n,])[1])
qplot(x,y,data=as.data.frame(data, assignment), col=factor(assignment)) +
  geom_point(aes(x, y), data=as.data.frame(params$mu), col="red", size = 7) +
  scale_color_discrete(name="Clusters") +
  lapply(1:K, function(k) geom_path(col="red", data=as.data.frame(ellipse(params$sigma[[k]],centre=params$mu[k,],level=0.9))))