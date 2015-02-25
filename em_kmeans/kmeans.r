kmeans = function(data, N, K)
{
  data = as.data.frame(data)
  iterations = 1
  
  bestLoss = Inf
  bestCentroids = NA
  
  losses = c()
  
  for(i in 1:10)
  {
    centroids = data[sample(nrow(data), K),]
    
    for(j in 1:30)
    {
      assignment = apply(data, 1, function(row)
      {
        dists = apply(centroids, 1, function(centroid) dist(rbind(centroid, row)))
        order(dists)[1]
      })
      
      centroids = as.data.frame(t(sapply(1:K, function(i) mean(data[assignment == i,]))))
      loss = sum(sapply(1:K, function(i) sum(dist(rbind(data[assignment == i,], centroids[i,]))^2)))
    }
    
    if(loss < bestLoss)
    {
      bestLoss = loss
      bestCentroids = centroids
      data$assignment = assignment
    }
    losses = c(losses, loss)
  }
  list(assignment = assignment, centroids = bestCentroids, loss = bestLoss, allLosses = losses)
}