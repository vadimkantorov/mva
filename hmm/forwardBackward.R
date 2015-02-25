source("computeAlpha.R")
source("computeGamma.R")
source("computeXi.R")
source("sanityCheck.R")

forwardBackward = function(data, params)
{
  normalize = function(x) x / sum(x)
  
  as = computeAlpha(data, params)
  alpha = as$alpha
  alphaScalers = as$scalers
  
  gamma = computeGamma(data, params, alpha)
  xi = computeXi(data, params, normalize, alpha, gamma)
  #sanityCheck(alpha, gamma, xi)
  
  list(alphaScalers=alphaScalers, gamma = gamma, xi = xi)
}