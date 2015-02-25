function res = votes(bwImage, theta, rho1, rho2)
    res = sum(bwImage(mask(bwImage, theta, rho1, rho2)));
end