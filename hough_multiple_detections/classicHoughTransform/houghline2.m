function [pdetect,tetadetect,Accumulator] = houghline2(Imbinary,pstep,tetastep,thresh) 
%HOUGHLINE - detects lines in a binary image using common computer vision operation known as the Hough Transform. 
% 
%Comments: 
% Function uses Standard Hough Transform to detect Lines in a binary image. 
% According to the Hough Transform, each pixel in image space 
% corresponds to a line in Hough space and vise versa.This function uses 
% polar representation of lines i.e. x*cos(teta)+y*sin(teta)=p to detect 
% lines in binary image. upper left corner of image is the origin of polar coordinate 
% system. 
% 
%Usage: [pdetect,tetadetect,Accumulator] = houghline(Imbinary,pstep,tetastep,thresh) 
% 
%Arguments: 
% Imbinary - a binary image. image pixels that have value equal to 1 are 
% interested pixels for HOUGHLINE function. 
% pstep - interval for radius of lines in polar coordinates. 
% tetastep - interval for angle of lines in polar coordinates. 
% thresh - a threshold value that determines the minimum number of 
% pixels that belong to a line in image space. threshold must 
% be bigger than or equal to 3(default). 
% 
%Returns: 
% pdetect - a vactor that contains radius of detected lines in 
% polar coordinates system. 
% tetadetect - a vector that contains angle of detcted lines in 
% polar coordinates system. 
% Accumulator - the accumulator array in Hough space. 
% 
%Originally Written by : 
% Amin Sarafraz 
% Photogrammetry & Computer Vision Devision 
% Geomatics Department,Faculty of Engineering 
% University of Tehran,Iran 
%  sarafraz@geomatics.ut.ac.ir 
% 
%Bug fixed by: 
% Nicolas HUOT 
% Electrical Engineering and Computer Science Student 
% UCLA (Los Angeles) and INPG (Grenoble) 
%  nicolas.huot@gmail.com 
% 
%May 5,2004 - Original version 
%November 24,2004 - Modified version,slightly faster and better performance. 
%June 13, 2005 - Positive sloped lines possible (Nicolas HUOT) 
                    

if nargin == 3 
    thresh = 3; 
elseif thresh < 3 
    error('threshold must be bigger than or equal to 3') 
    return; 
end

p = -sqrt((size(Imbinary,1))^2+(size(Imbinary,2))^2):pstep:sqrt((size(Imbinary,1))^2+(size(Imbinary,2))^2); 
teta = 0:tetastep:180-tetastep;

%Creating the accumulator: 
Accumulator = zeros(length(p),length(teta)); %creating the Accumulator 
[yIndex xIndex] = find(Imbinary); %extract the points from the image 
for cnt = 1:size(xIndex) %looping on the list of points of the image 
    Indteta = 0; 
    for tetai = teta*pi/180 %looping on the list of possible teta (in radians) 
        Indteta = Indteta+1; 
        roi = xIndex(cnt)*cos(tetai)+yIndex(cnt)*sin(tetai); %ro computed 
        if roi >= p(1) & roi <= p(end) %acceptable ro are filtered 
            temp = abs(roi-p); %computing the distances between the roi and the list of acceptable values 
            [mintemp,Indp] = min(temp);
            %mintemp = min(temp); %picking the minimum of it 
            %Indp = find(temp == mintemp); %finding the indexes of this value 
            Indp = Indp(1); %picking the first one 
            Accumulator(Indp,Indteta) = Accumulator(Indp,Indteta)+1; %hop, increasing the coresponding point in the Accu 
        end 
    end %end loop tetai 
end %end loop image points

% Finding local maxima in Accumulator 
AccumulatorbinaryMax = imregionalmax(Accumulator); %creating the array of regional max 
[Potential_p Potential_teta] = find(AccumulatorbinaryMax == 1); %picking up the maximums 
Accumulatortemp = Accumulator - thresh; %leveling the accu by the threshold 
pdetect = [];tetadetect = []; 
for cnt = 1:length(Potential_p) %looping on the possible ro 
    if Accumulatortemp(Potential_p(cnt),Potential_teta(cnt)) >= 0 %if the minimum threshold is reached 
        pdetect = [pdetect;Potential_p(cnt)]; %append the new confirmed values 
        tetadetect = [tetadetect;Potential_teta(cnt)]; 
    end 
end

% Calculation of detected lines parameters(Radius & Angle). 
pdetect = (pdetect - round(sqrt((size(Imbinary,1))^2+(size(Imbinary,2))^2)/pstep)) * pstep; 
tetadetect = tetadetect *tetastep - tetastep;