function f1 = f(yAdj, xAdj, thetaInd, rhoInd, sins, coss, rhos)
truncationWindowSize = 3;
windowSize = 1.5;
if thetaInd == 0 && rhoInd == 0
    f1 = 0;
else
    dist = xAdj*coss(thetaInd)+yAdj*sins(thetaInd) - rhos(rhoInd);
    if abs(dist) > truncationWindowSize
        f1 = 0;
    else
        f1 = exp(double(-dist^2/((2*windowSize^2))));
    end
end
