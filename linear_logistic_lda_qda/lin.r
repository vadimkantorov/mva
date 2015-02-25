LIN = function (train) with(train,
{
  n = length(y)
  x = cbind(rep(1, n), x1,x2)

  w = solve(t(x) %*% x) %*% t(x) %*% y
  
  posterior = function(x) t(w) %*% c(1, x)
    
  layers = c(geom_abline(slope = -w[2]/w[3], intercept = -(w[1] - 0.5)/w[3]))
  list(layers = layers, posterior = posterior)
})