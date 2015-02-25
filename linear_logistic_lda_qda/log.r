LOG = function (train) with(train,
{
  n = length(y)
  x = cbind(rep(1, n), x1,x2)
  
  logistic = function(x) 1/(1 + exp(-x))
  posterior = function(x) logistic(t(theta) %*% c(1, x))
    
  theta = matrix(0, 3)
  losses = c()
  for(i in 1:20)
  {
    eta = x %*% theta
    mu = logistic(eta)
    w = diag(c(mu*(1-mu)))
    z = eta + solve(w) %*% (y - mu)
    theta = solve(t(x) %*% w %*% x) %*% t(x) %*% w %*% z
    
    j = t(x) %*% (y - mu)
    losses = c(losses, t(j) %*% j)
  }
  
  layers = c(geom_abline(slope = -theta[2]/theta[3], intercept = -(theta[1])/theta[3])) #possible bug, no 0.5 needed
  list(layers = layers, posterior = posterior)    
})