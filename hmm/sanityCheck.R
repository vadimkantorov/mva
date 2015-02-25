sanityCheck = function(alpha, gamma, xi)
{
  print("Sanity check")
  for (i in 1:N)
    print(sprintf("ALPHA 1.0 ? %f", sum(alpha[i, ])))
  
  for (i in 1:N)
    print(sprintf("GAMMA 1.0 ? %f", sum(gamma[i, ])))
  
  for (i in 1:(N-1))
    for (k in 1:K)
      print(sprintf("XI: G:%f X:%f", gamma[i, k], sum(xi[k,,i])))
  
  for (i in 1:(N-1))
      print(sprintf("XI2: %f", sum(xi[,,i])))
  print("Sanity check done")
}