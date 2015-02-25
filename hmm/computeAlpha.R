computeAlpha = function(data, params)
{
  N = nrow(data)
  K = params$K
  alpha = matrix(ncol=K, nrow=N)
  scalers = rep(0, N)
  
  for (k in 1:K)
    alpha[1, k] = params$pi[k]*params$emissionProb(data[1,], k)
  scalers[1] = sum(alpha[1,])
  alpha[1,] = alpha[1,] / scalers[1]
  
  
  for (t in 2:N)
  {
    for(k in 1:K)
    {
      as = sapply(1:K, function(prev_k) 
        alpha[t-1, prev_k]*params$a[prev_k, k]
                  )
      alpha[t, k] = sum(as)*params$emissionProb(data[t,], k)
    }
    scalers[t] = sum(alpha[t,])
    alpha[t,] = alpha[t,] / scalers[t]
  }
  print("Computed alphas")
  list(alpha=alpha, scalers=scalers)
}