Project
_______
This project "Treating multiple detections with Hough transform" has been developed as a part of course "Advanced mathematical methods in computer vision" 2012 at École Normale Supérieure de Cachan.


Installation
____________
Project is available on github under url : https://github.com/vadimkantorov/advancedcv-hough. You should download a zip-ball from https://github.com/vadimkantorov/advancedcv-hough/zipball/master or alternatively clone https://github.com/vadimkantorov/advancedcv-hough.

All executables are MatLab files.

This project contains multiple versions of Hough transform. Directory classicHoughTransform contains implementation of classical Hough transform taken from http://www.mathworks.com/matlabcentral/fileexchange/4983-line-detection-via-standard-hough-transform . In order to execute it it is enought to run file evaluate.m from this directory.

Directories notOptimizedHoughFromThePaper, optimizedHoughFromThePaper and optimizedWithGradientHough contains implementation of algorithm presented in paper "On detection of multiple object instances using hough transforms" by Olga Barinova and Victor Lempitsky and Pushmeet Kohli. Main difference between them are optimizations applied. Every version contains evaluate.m file which executes tests.
- notOptimizedHoughFromThePaper is a plain greedy algorithm from paper
- optimizedHoughFromThePaper enchance notOptimizedHoughFromThePaper by caching values. It has same execution time for a first iteration, however consequtive iterations are less expensive
- optimizedWithGradientHough use the gradient direction to reduce the number of votes. Code is build on top of optimizedHoughFromThePaper. It is fastest version presented by us.

Examplary images are available in directory img . 


Authors
_______

Project has been developed by Wojciech Zaremba and Vadim Kantorov (woj.zaremba, vadim.kantorov [at] where-everyone-has-a-mailbox)
