library(ggplot2)
library(kernlab)

setwd("Z:\\MVA\\Kernel methods\\TD\\TD2\\src")
data = as.matrix(read.csv("..\\data\\movement_libras.data"))
ground_truth = data[,91]
data = data[,-91]

k1 = "vanilladot"
k2 = "Linear"

clustering = specc(x=data, centers=15, kernel=k1)
pca = kpca(x=data, features=2, kernel=k1)
comps = rotated(pca)
centroids = predict(pca, attributes(clustering)$centers)
colnames(centroids) = c("x", "y")
centroids = data.frame(x=centroids[,"x"], y=centroids[,"y"], z=1:nrow(centroids))

qplot(x,y,data=data.frame(x=comps[,1], y=comps[,2], assignment=clustering), col=factor(assignment), main=paste(k2, "kernel")) +
  geom_point(aes(x, y, col=factor(z)), data=centroids, size = 7) +
  scale_color_discrete(name="Clusters",l=40,c=150)