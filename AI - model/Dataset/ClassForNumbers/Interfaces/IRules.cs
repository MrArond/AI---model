using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Interfaces
{
    internal interface IRules
    {
        //cos
        Task<List<float>> Field(List<bool[,]> images);
        Task<List<float>> Circuit(List<bool[,]> images);
        Task<List<float>> Width(List<bool[,]> images);
        Task<List<float>> Height(List<bool[,]> images);
        Task<List<float>> Symetry(List<bool[,]> images);
        Task<List<float>> Proportion(List<bool[,]> images);
        Task<List<float>> CalculatePerimeters(List<bool[,]> images);
        Task<float> CalculateSinglePerimeter(bool[,] image);

        bool[,] MoveImageToCenter(bool[,] img);
        void normalize_minmax(List<float> values);
        (float, float) GiniImpurity(List<float> values, List<int> labels);
        Task<List<float>> CalculateCircularity(List<float> areas, List<float> perimeters);
        Task<List<float>> CalculateRadialRatiosAsync(List<bool[,]> images);
        Task<List<float>> CalculateExtentsAsync(List<bool[,]> images, List<float> areas);
        Task<List<float>> CalculateInertiaRatiosAsync(List<bool[,]> images);
        Task<List<int>> CalculatePeakCountsAsync(List<bool[,]> images);
        Task<List<float>> CalculateCentralSymmetryAsync(List<bool[,]> images);
        (float threshold, float impurity) GiniImpurityFast(List<float> values, List<int> labels);
        void StandardScale(List<float> values);
    }
}
