library(ggplot2)
library(gridExtra)
library(ROCR)
setwd('D:/MVA/Graphical models/Assignment 1/')
names = c("QDA", "LIN", "LOG", "LDA")
files = c("A", "B", "C")

# for(file in files)
# {
#   train = read.table(paste('classification', file, ".train", sep=""), sep='\t', col.names=c('x1', 'x2', 'y'))
#   test = read.table(paste('classification', file, ".test", sep=""), sep='\t', col.names=c('x1', 'x2', 'y'))
#   
#   print(paste(file,"(train):", nrow(train)))
#   print(paste(file,"(test):", nrow(test)))
#   
#   print(paste(file,"0 (train):", nrow(train[train$y == 0,])))
#   print(paste(file,"1 (train):", nrow(train[train$y == 1,])))
#   
#   print(paste(file,"0 (test):", nrow(test[test$y == 0,])))
#   print(paste(file,"1 (test):", nrow(test[test$y == 1,])))
# }

for(name in names)
{
  source(paste(name,".R", sep=""))
  avAcc = 0
  
  for(file in files)
  {
    train = read.table(paste('classification', file, ".train", sep=""), sep='\t', col.names=c('x1', 'x2', 'y'))
    test = read.table(paste('classification', file, ".test", sep=""), sep='\t', col.names=c('x1', 'x2', 'y'))
    
    l = eval(call(name, train))
    
    classify = function(x) as.numeric(l$posterior(x) > 0.5)
    test$classified = apply(test, 1, function(x) classify(x[c(1,2)]))
    
    desc = paste("Method: ", name, ", Dataset: ", file,sep="")
    p = ggplot() +
        geom_point(aes(x=x1, y=x2, data=train, col=factor(y)), data=train) + 
        geom_point(aes(x=x1, y=x2, shape=7, size=3, col=factor(classified)), data=test) +
        xlab('x1') +
        ylab('x2') +
        opts(title=desc)
    p = Reduce("+", l$layers, p) 
    
    ggsave(p, filename=paste("plots/",name,"-",file,"-points.png", sep=""))
    
    #TODO: read test data
    png(paste("plots/",name,"-",file,"-rocr.png",sep=""))
    rocr_preds_train = prediction(apply(cbind(train$x1, train$x2), 1, l$posterior), train$y)
    rocr_perf_train = performance(rocr_preds_train, "prec", "rec")
    plot(rocr_perf_train, colorize=T, lwd=1)
    
    rocr_preds_test = prediction(apply(cbind(test$x1, test$x2), 1, l$posterior), test$y)
    rocr_perf_test = performance(rocr_preds_test, "prec", "rec")
    plot(rocr_perf_test, colorize=T, add=T, lwd=6)
    
    rocr_perf_test = performance(rocr_preds_test, "acc")
    cutoffs = unlist(rocr_perf_test@x.values)
    ind = which(abs(cutoffs - 0.5)<1e-1)[1]
    acc = rocr_perf_test@y.values[[1]][ind]
    print(paste(desc, ", Acc: ", acc, sep=""))
    avAcc = avAcc + acc
    
    legend("bottomleft", "", c("Train", "Test"), lwd=c(1,6))
    dev.off()
  }
    
  print(paste(name, " av. acc: ", avAcc/3, sep=""))
}