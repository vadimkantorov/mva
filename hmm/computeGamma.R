computeGamma = function(data, params, alpha)
{
  N = nrow(data)
  K = params$K
  gamma = matrix(ncol=K, nrow=N)
  gamma[N, ] = alpha[N, ]
  for (t in (N-1):1)
  {
    for(k in 1:K)
    {
      as = sapply(1:K, function(next_k)
      {
        denom = sum(sapply(1:K, function (k) alpha[t, k]*params$a[k, next_k]))
        params$a[k, next_k] * gamma[t+1, next_k] / denom
      })
      gamma[t, k] = alpha[t, k] * sum(as)
    }
  }
  print("Computed gammas")
  gamma
}
  
  