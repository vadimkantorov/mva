addpath('../img');
addpath('../notOptimizedHoughFromThePaper');
clear;
fileNames = {'P1020171.jpg' '1.jpg' '2.jpg' '3.jpg' '4.jpg' '5.jpg'};
for z = 2:2%length(fileNames)
    
    fileName = fileNames{z};
    I  = imread(fileName);
    I = blackwhite(I);
    bwImage = edge(I,'canny');
    
    R = norm(size(I))/2;
    N_rho = int32(R+1);
    N_theta = 3*N_rho;
    
    thetaStep = pi / double(N_theta);
    rhoStep = R / double(N_rho);
    
    timeLineSearchStart = cputime();
    fprintf('Classical: Starting on %s (N_theta = %d, N_rho = %d)\n', fileName, N_theta,N_rho);
    fprintf('Theta space size: %d\n', N_theta);
    [pdetect,tetadetect,Accumulator] = houghline2(bwImage, 1, 180/double(N_theta), 20);
    fprintf('Ending %s: %.2f seconds\n\n', fileName, (cputime()-timeLineSearchStart));
    [acc, ~] = sort(Accumulator(:), 'descend');
    for i = 1 : 20
        fprintf('%d\n', acc(i));
    end
    colorImage = colorize(I)/255;
    EPS = 0.1;
    fprintf('number of lines %d', length(pdetect));
    for j = 1:length(pdetect)        
        colorImage = drawLine(colorImage, pi*tetadetect(j)/180, pdetect(j)-EPS, pdetect(j)+EPS);
    end
    
    
    imshow(colorImage);
end