using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Interfaces
{
    internal interface IRules
    {
        Task<int> Field(List<bool[,]> images);
        Task<float> Circuit(List<bool[,]> images);
        Task<float> Width(List<bool[,]> images);
        Task<float> Height(List<bool[,]> images);
        Task<float> Symetry(List<bool[,]> images);
        Task<float> Proportion(List<bool[,]> images);

    }
}
