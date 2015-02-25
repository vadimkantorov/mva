function [] = improvedHough(fileName)
if nargin < 1
    fileName = 'P1020171.jpg';
end

addpath('../img/');

I  = imread(fileName);
I = blackwhite(I);
bwImage = edge(I);
imshow(bwImage);
%bwImage = 1-I(:,:,1) / 255;
colorImage = colorize(bwImage);

global R N_theta N_rho;
R = norm(size(I))/2;
N_rho = round(R+1);
N_theta = 3*N_rho;

EPS = 1;
lambda = 0.3;

sz1 = size(bwImage, 1);
sz2 = size(bwImage, 2);
halfX = int32(size(bwImage, 2)/2);
halfY = int32(size(bwImage, 1)/2);

X = zeros(sz1, sz2, 2);
bestThetaInds = [];
bestRhoInds = [];

fprintf('\nImproved not optimized: Starting on %s (N_theta = %d, N_rho = %d)\n', fileName, N_theta,N_rho);

for z = 1:3
    timeLineSearchStart = cputime();
    fprintf('Start of line search %d\n', z);
    M = zeros(1+N_theta, 1+2*N_rho);
    edgePixelCount = 0;
    [yind, xind] = find(bwImage);
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
                theta = (thetaInd-1)*pi/N_theta;
                rho = xAdj*cos(theta) + yAdj*sin(theta);
                rhoInd = min(max(int32(((rho + R) * N_rho) / R + 1), 0), 2*N_rho-1);
            end
            
            f1 = f(yAdj, xAdj, thetaInd, rhoInd, N_rho, N_theta, R);
            f2 = f(yAdj, xAdj, X(y, x, 1), X(y, x, 2) , N_rho, N_theta, R);
            M(1+thetaInd, 1+rhoInd) = M(1+thetaInd, 1+rhoInd) + max(0, f1 - f2);
        end
    end
    fprintf('Main loop took %.2f seconds\n', cputime() - timeMainLoopStart);
    
    fprintf('Found %d edge pixels (out of %d)\n', length(xind), sz1*sz2);
    
    bestThetaInd = 0;
    bestRhoInd = 0;
    for thetaInd = 0:N_theta
        for rhoInd = 0:2*N_rho
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
    
    for y = 1:sz1
        for x = 1:sz2
            xAdj = x - halfX;
            yAdj = y - halfY;
            if f(yAdj, xAdj, X(y, x, 1), X(y, x, 2), N_rho, N_theta, R) < f(yAdj, xAdj, bestThetaInd, bestRhoInd, N_rho, N_theta, R)
                  X(y, x, :) = [bestThetaInd, bestRhoInd];
            end
        end
    end
    
    fprintf('End of line search %d (%.2f seconds)\n', z, (cputime()-timeLineSearchStart));
end

for i = 1:length(bestThetaInds)
 %   fprintf ('theta = %f, rho = %f \n', bestThetaInds(i), bestRhoInds(i))
    thetaInd = bestThetaInds(i);
    rhoInd = bestRhoInds(i);
    theta = (thetaInd-1)*pi/N_theta;
    rho = -R + R*(rhoInd-1)/N_rho;
    colorImage = drawLine(colorImage, theta, rho-EPS, rho+EPS);
end
imshow(colorImage);
end