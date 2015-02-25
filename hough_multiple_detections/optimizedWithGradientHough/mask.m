function res = mask(bwImage, theta, rho1, rho2)
    res = zeros(size(bwImage, 1), size(bwImage, 2));
    halfX = int32(size(bwImage, 2)/2);
    halfY = int32(size(bwImage, 1)/2);
    
    if abs(theta - 0) < 1e-2 || abs(theta - pi) < 1e-2
        res(1:size(bwImage, 1), halfX+(int32(rho1):int32(rho2))) = 1;
    else
        for x = 1:size(bwImage,  2)
            y1 = int32(rho1/sin(theta) - (x-halfX)*cos(theta)/sin(theta));
            y2 = int32(rho2/sin(theta) - (x-halfX)*cos(theta)/sin(theta));
            minY = min(y1, y2) + halfY;
            maxY = max(y1, y2) + halfY;
            
            minY = max(minY, 1);
            maxY = min(maxY, size(bwImage, 1));

            res(minY:maxY, x) = 1;
        end
    end
    res = logical(res);
end