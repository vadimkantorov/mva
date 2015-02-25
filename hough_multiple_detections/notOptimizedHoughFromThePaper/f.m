function res = f(yAdj, xAdj, thetaInd, rhoInd, N_rho, N_theta, R)
    if thetaInd == 0 && rhoInd == 0
        res = 0;
    else
        theta = (thetaInd-1)*pi/N_theta;
        rho = -R + R*(rhoInd-1)/N_rho;
        res = truncatedGaussian(xAdj*cos(theta)+yAdj*sin(theta) - rho);
        %res =  (abs(xAdj*cos(theta)+yAdj*sin(theta) - rho) < 2);
    end
end