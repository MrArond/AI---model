using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Interfaces
{
    internal interface IRules
    {



       (double cX, double cY, double area) CalculateCentersAndAreasAsync(bool[,] images);
       float CalculateRadialRatiosAsync(bool[,]img, (double cX, double cY, double area) metric);
       float CalculateExtentsAsync(bool[,] img, float areas);
       float CalculateInertiaRatiosAsync(bool[,] img, (double cX, double cY, double area) metric);
       float CalculateCentralSymmetryAsync(bool[,] img, (double cX, double cY, double area) metric);
    }
}
