function [thetaConvTable, rhoConvTable] = precalc(N_theta, N_rho)

    thetaConvTable = zeros(1 + N_theta, 1);
    for thetaInd = 0:N_theta
        thetaConvTable(1 + thetaInd) = 
    end
    
    for rhoInd = 0:N_rho
        
    end
    
        
    if thetaInd == 0 && rhoInd == 0
        res = 0;
    else
        rho = rhoConv(rhoInd);
        theta = thetaConv(thetaInd);
        res = 0;
    end

    end
end