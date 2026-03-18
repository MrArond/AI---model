using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Interfaces
{
    internal interface IRules
    {
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
    }
}
