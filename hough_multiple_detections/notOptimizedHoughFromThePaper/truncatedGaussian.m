function res = truncatedGaussian(dist)
    truncationWindowSize = 3;
    windowSize = 1.5;
    if abs(dist) > truncationWindowSize
        res = 0;
    else
        res = exp(double(-dist^2/((2*windowSize^2))));
    end
end