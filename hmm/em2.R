source("forwardBackward.R")

hmm_estimateParameters = function(data, params)
{
  N = nrow(data)
  K = params$K
  
  estimateParameters = function(gamma, xi)
  {
    pi_new = gamma[1,]
    
    Nk = colSums(gamma)
    mu_new = crossprod(gamma, data) / Nk
    
    op = function(v) v %*% t(v)
    sigma_new = lapply(1:K, function(k) 
      Reduce("+", lapply(1:N, function(n) gamma[n,k]/Nk[k]*op(data[n,] - mu_new[k,])))
    )
    
    a_new = matrix(nrow=K, ncol=K)
    for (i in 1:K)
      for (j in 1:K)
        a_new[i,j] = sum(xi[i, j, ]) / (Nk[i] - gamma[N, i])
        
    list(pi = pi_new, mu = mu_new, sigma = sigma_new, a = a_new)
  }
  
  iterations = 30
  for(i in 1:iterations)
  {
    params = aug(params)
    gamma_xi = forwardBackward(data, params)
    params = estimateParameters(gamma_xi$gamma, gamma_xi$xi)
  }
  aug(params)
}