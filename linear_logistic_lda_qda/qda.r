QDA = function (train) with(train,
{
  x = cbind(train$x1,train$x2)
  n = nrow(x)
  py = train$y
  ny = 1-py
  n_pos = sum(py)
  n_neg = sum(ny)
  
  p_pos = n_pos / n
  p_neg = n_neg / n
  
  mu_pos = matrix(mapply(function(i) py %*% x[,i] / n_pos, 1:2))
  mu_neg = matrix(mapply(function(i) ny %*% x[,i] / n_neg, 1:2))
  
  cov_pos = Reduce("+", Map(function(i) py[i]*(matrix(x[i,] - mu_pos) %*% t(matrix(x[i,] - mu_pos))),1:n)) / n_pos
  cov_neg = Reduce("+", Map(function(i) ny[i]*(matrix(x[i,] - mu_neg) %*% t(matrix(x[i,] - mu_neg))),1:n)) / n_neg
  
  cov_pos_inv = solve(cov_pos)
  cov_neg_inv = solve(cov_neg)
  
  a = 1/2*(cov_neg_inv - cov_pos_inv)
  b = cov_pos_inv %*% mu_pos - cov_neg_inv %*% mu_neg
  c = 1/2*log(det(cov_neg)/det(cov_pos)) + 1/2*(t(mu_neg) %*% cov_neg_inv %*% mu_neg - t(mu_pos) %*% cov_pos_inv %*% mu_pos)
  c = c + log(p_pos / p_neg)
  posterior = function(x)
  {
    p1 = p_pos*exp(-1/2*(t(x - mu_pos) %*% cov_pos_inv %*% (x - mu_pos)))/sqrt(det(cov_pos))
    p2 = p_neg*exp(-1/2*(t(x - mu_neg) %*% cov_neg_inv %*% (x - mu_neg)))/sqrt(det(cov_neg))
    
    p1/(p1+p2)
  }
  
  
  xs = c()
  ys = c()
  qs = c()
  ps = c()
  for(x11 in seq(-5, 5, 0.1))
  {
    aa = a[2,2]
    bb = b[2] + (a[1,2] + a[2,1])*x11
    cc = as.numeric(c + b[1]*x11 + a[1,1]*x11^2)
    
    D = bb^2 - 4*aa*cc
    if(D >= 0 && abs(D) >= 1e-8)
    {
      x2_1 = (-bb + sqrt(D))/(2*aa)
      x2_2 = (-bb - sqrt(D))/(2*aa)
      xs = c(xs, x11, x11)
      ys = c(ys, x2_1, x2_2)
      
      xxx = matrix(c(x11, x2_1))
      ps = c(ps, posterior(xxx))
      qs = c(qs, t(xxx) %*% a %*% xxx + t(b) %*% xxx + c)
      
      xxx = matrix(c(x11, x2_2))
      ps = c(ps, posterior(xxx))
      qs = c(qs, t(xxx) %*% a %*% xxx + t(b) %*% xxx + c)
    }
  }
  layers = c(geom_point(aes(x = x, y = y), col=I("black"), size=I(2), data=data.frame(x = xs, y = ys)))
  list(layers = layers, posterior = posterior)
})