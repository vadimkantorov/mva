function colorImage = colorize(bwImage)
    if ndims(bwImage) == 3
        colorImage = bwImage;
    else
        bwImage = ones(size(bwImage, 1), size(bwImage, 2)) -double(bwImage);
        colorImage = zeros(size(bwImage, 1), size(bwImage, 2), 3);
        colorImage(:,:,1) = bwImage;
        colorImage(:,:,2) = bwImage;
        colorImage(:,:,3) = bwImage;
    end
end