library(ggplot2)
library(reshape)

setwd('Z:/MVA/Advanced CV/src2/report')

time800 = read.table('time800.txt', header=T)
time600 = read.table('time600.txt', header=T)
time400 = read.table('time400.txt', header=T)

time = rbind(cbind(time800, theta=800), cbind(time600, theta=600), cbind(time400, theta=400))
timeSummed = data.frame(NO = time$NO1 + time$NO2 + time$NO3, 
                        VR = time$VR1 + time$VR2 + time$VR3,
                        RG = time$RG1 + time$RG2 + time$RG3,
                        CL = time$CL,
                        theta = time$theta)

timeAveraged = aggregate(timeSummed, by=list(timeSummed$theta), FUN=mean)
timeAveraged$Group.1 = NULL
melted = melt(timeAveraged, id=c('theta'))
qplot(data=melted, col=variable, x=theta, y = value, geom=c('line', 'point')) +
  geom_point(size=3) +
  scale_color_discrete(name='Variant', breaks=c('NO', 'VR', 'RG', 'CL'), labels=c('Not optimized', 'Votes reuse', 'Votes reuse + gradient opt.', 'Classical')) +
  scale_x_continuous(breaks=timeSummed$theta, name='Angle count') +
  scale_y_continuous(name='Time')
