viterbi = function(data, params)
{
  N = nrow(data)
  K = params$K
  v = matrix(nrow = N, ncol = K)
  ptr = matrix(nrow = N, ncol = K)
  
  for(k in 1:K)
    v[1, k] = log(params$emissionProb(1, k)) + log(params$pi[k])
  
  for(t in 2:N)
  {
    for(k in 1:K)
    {
      v[t, k] = log(params$emissionProb(data[t,], k))
      curBest = -Inf
      for(z in 1:K)
      {
        tr = log(params$a[z, k]) + v[t-1,z]
        
        if(tr > curBest)
        {
          curBest = tr
          ptr[t, k] = z
        }
      }
      v[t, k] = v[t, k] + curBest
    }
  }
  print(v)
  print(ptr)
  
  res = rep(0, N)
  res[N] = which.max(v[N,])
  
  for(t in (N-1):1)
    res[t] = ptr[t+1, res[t+1]]
  
  res
}