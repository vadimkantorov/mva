computeXi = function(data, params, normalize, alpha, gamma)
{
  N = nrow(data)
  K = params$K
  xi = array(dim=c(K, K, N-1))
  for(t in 1:(N-1))
  {
    for (k1 in 1:K)
    {
      for(k2 in 1:K)
      {
        t2 = t+1
        xi[k1, k2, t] = alpha[t, k1] /
                        alpha[t2, k2] *
                        params$emissionProb(data[t2,], k2) * 
                        gamma[t2, k2] * 
                        params$a[k1, k2]
      }
    }
    xi[,,t] = normalize(xi[,,t])
  }
  print("Computed xis")
  xi
}