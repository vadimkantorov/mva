function bwImage = drawLine(img, theta, rho1, rho2)
    m = mask(img, theta, rho1, rho2);
    
    r = img(:,:,1);
    g = img(:,:,2);
    b = img(:,:,3);
    
    r(m) = 0;
    g(m) = 1;
    b(m) = 1;
    
    bwImage(:,:,1) = r;
    bwImage(:,:,2) = g;
    bwImage(:,:,3) = b;
end