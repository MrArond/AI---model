using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Interfaces
{
    internal interface IRules
    {



        Task<List<(double cX, double cY, double area)>> CalculateCentersAndAreasAsync(List<bool[,]> images);
        Task<List<float>> CalculateRadialRatiosAsync(List<bool[,]> images, List<(double cX, double cY, double area)> metrics);
        Task<List<float>> CalculateExtentsAsync(List<bool[,]> images, List<float> areas);
        Task<List<float>> CalculateInertiaRatiosAsync(List<bool[,]> images, List<(double cX, double cY, double area)> metrics);
        Task<List<float>> CalculateCentralSymmetryAsync(List<bool[,]> images, List<(double cX, double cY, double area)> metrics);
    }
}
