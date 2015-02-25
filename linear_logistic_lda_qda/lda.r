LDA = function (train) with(train,
{
  x = cbind(x1,x2)
  n = nrow(x)
  ny = 1-y
  n_pos = sum(y)
  n_neg = sum(ny)
  
  p_pos = n_pos / n
  p_neg = n_neg / n
  
  mu_pos = matrix(mapply(function(i) y %*% x[,i] / n_pos, 1:2))
  mu_neg = matrix(mapply(function(i) ny %*% x[,i] / n_neg, 1:2))
  
  cov_pos = Reduce("+", Map(function(i) y[i]*(matrix(x[i,] - mu_pos) %*% t(matrix(x[i,] - mu_pos))),1:n)) / n_pos
  cov_neg = Reduce("+", Map(function(i) ny[i]*(matrix(x[i,] - mu_neg) %*% t(matrix(x[i,] - mu_neg))),1:n)) / n_neg
  cov = n_pos/n * cov_pos + n_neg/n * cov_neg
  cov_inv = solve(cov)
  
  w = cov_inv %*% (mu_pos - mu_neg)
  w0 = -1/2*t(mu_pos) %*% cov_inv %*% mu_pos + 1/2*t(mu_neg) %*% cov_inv %*% mu_neg + log(p_pos / p_neg)
  
  logistic = function(x) 1/(1+exp(-x))
  posterior = function(x) logistic(t(w) %*% x + w0)
  
  layers = c(
    geom_point(aes_string(x=mu_pos[1], y=mu_pos[2]),col=I("blue"), size=I(5)),
    geom_point(aes_string(x=mu_neg[1], y=mu_neg[2]),col=I("red"), size=I(5)),
    geom_abline(slope = -w[1]/w[2], intercept = -w0/w[2]))
  
  list(layers = layers, posterior = posterior)
})
