function [] = improvedHough(fileName, edgeCount, drawResults)
if nargin < 3
    drawResults = true;
end
if nargin < 2
    edgeCount = 7;
end
if nargin < 1
    fileName = 'painted.bmp';
end

addpath('../img/');

I  = imread(fileName);
%I = colorize(I);
I = blackwhite(I);
bwImage = edge(I);
%bwImage = 1-I(:,:,1) / 255;
colorImage = colorize(bwImage);

R = norm(size(I))/2;
N_rho = round(R+1);
N_theta = 2*N_rho;

sins = zeros(N_theta, 1);
coss = zeros(N_theta, 1);
for thetaInd = 1:N_theta
    theta = (thetaInd-1)*pi/N_theta;
    sins(thetaInd) = sin(theta);
    coss(thetaInd) = cos(theta);
end
rhos = zeros(2*N_rho, 1);
for rhoInd = 1:2*N_rho
    rhos(rhoInd) = -R + R*(rhoInd-1)/N_rho;
end

lambda = 0.3;

sz1 = size(bwImage, 1);
sz2 = size(bwImage, 2);
halfX = int32(size(bwImage, 2)/2);
halfY = int32(size(bwImage, 1)/2);

X = zeros(sz1, sz2, 2);
bestThetaInds = [];
bestRhoInds = [];

fprintf('\nImproved _optimized_: Starting on %s (N_theta = %d, N_rho = %d)\n', fileName, N_theta,N_rho);
truncationWindowSize = 5.0;
windowSize = 5.0;

[yind, xind] = find(bwImage);
fprintf('Found %d edge pixels (out of %d)\n', length(xind), sz1*sz2);
M = zeros(1+N_theta, 1+2*N_rho);
M_invalidated = ones(1+N_theta, 1+2*N_rho);
for z = 1:edgeCount
    timeLineSearchStart = cputime();
    fprintf('Start of line search %d\n', z);
    timeMainLoopStart = cputime();
    for k = 1:length(xind)
        x = xind(k);
        y = yind(k);
            
        xAdj = x - halfX;
        yAdj = y - halfY;

        for thetaInd = 0:N_theta
            if thetaInd == 0
                rhoInd = 0;
            else
                rho = xAdj*coss(thetaInd) + yAdj*sins(thetaInd);
                rhoInd = min(max(int32(((rho + R) * N_rho) / R + 1), 0), 2*N_rho-1);
            end
            
            if z > 0
                if M_invalidated(1+thetaInd, 1+rhoInd) == 0
                    continue;
                end
            end
            
            if thetaInd == 0 && rhoInd == 0
                f1 = 0;
            else
                dist = xAdj*coss(thetaInd)+yAdj*sins(thetaInd) - rhos(rhoInd);
                %f1 = max(0, 1-dist);
                if abs(dist) > truncationWindowSize
                    f1 = 0;
                else
                    f1 = exp(double(-dist^2/((2*windowSize^2))));
                end
            end
            
            thetaInd2 = X(y, x, 1);
            rhoInd2 = X(y, x, 2);
            if thetaInd2 == 0 && rhoInd2 == 0
                f2 = 0;
            else
                dist = xAdj*coss(thetaInd2)+yAdj*sins(thetaInd2) - rhos(rhoInd2);
                %f2 = max(0, 1-dist);
                if abs(dist) > truncationWindowSize
                    f2 = 0;
                else
                    f2 = exp(double(-dist^2/((2*windowSize^2))));
                end
                %res =  (abs(xAdj*cos(theta)+yAdj*sin(theta) - rho) < 2);
            end
            
            M(1+thetaInd, 1+rhoInd) = M(1+thetaInd, 1+rhoInd) + max(0, f1 - f2);
        end
    end
    fprintf('Main loop took %.2f seconds\n', cputime() - timeMainLoopStart);
    
    fprintf('Starting extra loops\n');
    timeExtraLoopStart = cputime();
    
    bestThetaInd = 0;
    bestRhoInd = 0;
    for thetaInd = 0:N_theta
        for rhoInd = 0:2*N_rho
            M_invalidated(1+thetaInd, 1+rhoInd) = 0;
            if M(1+thetaInd, 1+rhoInd) > M(1+bestThetaInd, 1+bestRhoInd)
                bestThetaInd = thetaInd;
                bestRhoInd = rhoInd;
            end            
        end
    end
    
    if M(1+bestThetaInd, 1+bestRhoInd) < lambda
        fprintf('value %f\n', M(1+bestThetaInd, 1+bestRhoInd));
        fprintf('Breaking line search %d (%.2f seconds)\n', z, (cputime()-timeLineSearchStart));
        break;
    end
    
    bestThetaInds = [bestThetaInds bestThetaInd];
    bestRhoInds = [bestRhoInds bestRhoInd];
    
    for k = 1:length(xind)
        x = xind(k);
        y = yind(k);
            
        xAdj = x - halfX;
        yAdj = y - halfY;
            
        if f(yAdj, xAdj, X(y, x, 1), X(y, x, 2), sins, coss, rhos) < f(yAdj, xAdj, bestThetaInd, bestRhoInd, sins, coss, rhos)
              X(y, x, :) = [bestThetaInd, bestRhoInd];              
              for thetaInd = 0:N_theta
                  if thetaInd == 0
                    rhoInd = 0;
                  else
                    rho = xAdj*coss(thetaInd) + yAdj*sins(thetaInd);
                    rhoInd = min(max(int32(((rho + R) * N_rho) / R + 1), 0), 2*N_rho-1);
                  end
                  M_invalidated(thetaInd + 1, rhoInd + 1) = 1;
                  M(thetaInd + 1, rhoInd + 1) = 0;
              end
        end
    end
    fprintf('End of extra loops (%.2f seconds)\n', cputime() - timeExtraLoopStart);
    fprintf('End of line search %d (%.2f seconds)\n', z, (cputime()-timeLineSearchStart));
end
if drawResults
    for i = 1:length(bestThetaInds)
     %   fprintf ('theta = %f, rho = %f \n', bestThetaInds(i), bestRhoInds(i))
        thetaInd = bestThetaInds(i);
        rhoInd = bestRhoInds(i);
        theta = (thetaInd-1)*pi/N_theta;
        rho = rhos(rhoInd);
        colorImage = drawLine(colorImage, theta, rho - 0.5, rho +0.5);
    end
    imshow(colorImage);
end
end