using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Implementation
{
    internal class Rules : Interfaces.IRules
    {
        async Task<int> Interfaces.IRules.Field(List<bool[,]> images)
        {
            int area = 0;

            foreach (var image in images)
            {
                int rows = image.GetLength(0);
                int cols = image.GetLength(1);

                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (image[y, x])
                        {
                            area++;
                        }
                    }
                }
            }

            if (area == 0)
                return 0;

            return area;
        }
        async Task<float> Interfaces.IRules.Circuit(List<bool[,]> images)
        {
            float perimeter = 0;

            foreach (var image in images)
            {
                int rows = image.GetLength(0);
                int cols = image.GetLength(1);

                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (image[y, x])
                        {
                            if (y == 0 || !image[y - 1, x]) perimeter++;
                            if (y == rows - 1 || !image[y + 1, x]) perimeter++;
                            if (x == 0 || !image[y, x - 1]) perimeter++;
                            if (x == cols - 1 || !image[y, x + 1]) perimeter++;
                        }
                    }
                }
            }

            return perimeter;
        }
        Task<float> Interfaces.IRules.Width(List<bool[,]> images)
        {
            throw new NotImplementedException();
        }
        Task<float> Interfaces.IRules.Height(List<bool[,]> images)
        {
            throw new NotImplementedException();
        }
        Task<float> Interfaces.IRules.Symetry(List<bool[,]> images)
        {
            throw new NotImplementedException();
        }
        Task<float> Interfaces.IRules.Proportion(List<bool[,]> images)
        {
            throw new NotImplementedException();
        }
    }
}
