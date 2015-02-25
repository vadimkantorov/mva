hmm_estimateParametersInitially = function(data, N, K)
{
  source("kmeans.r")
  
  estimateParameters = function(gamma)
  {
    Nk = colSums(gamma)
    pi_new = Nk/N
    mu_new = crossprod(gamma, data) / Nk
    
    op = function(v) v %*% t(v)
    
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
  
  clustering = kmeans(data, N, K)
    
  params = estimateParametersInitially(clustering$assignment)
  iterations = 100
  for(i in 1:iterations)
  {
    gamma = estimateGamma(params)
    params = estimateParameters(gamma)
  }
  params
}