using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Implementation
{
    internal class Rules : Interfaces.IRules
    {

        private int imageCenterX = 112;
        private int imageCenterY = 112;
        private int imageWidth = 224;
        private int imageHeight = 224;

        private float CalculateSinglePerimeterSync(bool[,] image)
        {
            int rows = image.GetLength(0);
            int cols = image.GetLength(1);
            float perimeter = 0;

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
            return perimeter;
        }

        private float CalculateSingleFieldSync(bool[,] image)
        {
            int area = 0;
            int rows = image.GetLength(0);
            int cols = image.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (image[y, x]) area++;
                }
            }
            return (float)area;
        }

        private void GetBoundingBox(bool[,] image, out int minX, out int maxX, out int minY, out int maxY)
        {
            minX = int.MaxValue;
            maxX = int.MinValue;
            minY = int.MaxValue;
            maxY = int.MinValue;

            int rows = image.GetLength(0);
            int cols = image.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (image[y, x])
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }
            }
        }

        private float CalculateSingleWidth(bool[,] image)
        {
            GetBoundingBox(image, out int minX, out int maxX, out int _, out int _);
            if (minX > maxX) return 0f;
            return (maxX - minX + 1);
        }

        private float CalculateSingleHeight(bool[,] image)
        {
            GetBoundingBox(image, out int _, out int _, out int minY, out int maxY);
            if (minY > maxY) return 0f;
            return (maxY - minY + 1);
        }

        private float CalculateSingleProportion(bool[,] image)
        {
            float width = CalculateSingleWidth(image);
            float height = CalculateSingleHeight(image);
            if (height == 0) return 0f;
            return width / height;
        }

        private float CalculateSingleSymmetry(bool[,] image)
        {
            GetBoundingBox(image, out int minX, out int maxX, out int minY, out int maxY);
            if (minX > maxX || minY > maxY) return 0f;

            int cols = image.GetLength(1);
            float centerX = (minX + maxX) / 2.0f;
            int totalPixels = 0;
            int matchingPixels = 0;

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (image[y, x])
                    {
                        totalPixels++;
                        int mirroredX = (int)Math.Round(2 * centerX - x);
                        if (mirroredX >= 0 && mirroredX < cols && image[y, mirroredX])
                        {
                            matchingPixels++;
                        }
                    }
                }
            }

            if (totalPixels == 0) return 0f;
            return (float)matchingPixels / totalPixels;
        }

        private (int, int) CenterOfMass(bool[,] img)
        {
            float sumX = 0;
            float sumY = 0;
            int count = 0;
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (img[y, x])
                    {
                        sumX += x;
                        sumY += y;
                        count++;
                    }
                }
            }

            if (count == 0) return (0, 0);

            var center = ((int)Math.Round(sumX / count), (int)Math.Round(sumY / count));

            int dx = center.Item1 - imageCenterX;
            int dy = center.Item2 - imageCenterY;

            return (-dx, -dy);
        }

        private float Impurity(Dictionary<int, int> shapesDic)
        {
            float impurity = 1;
            int shapesDicCount = shapesDic.Values.Sum();

            if (shapesDicCount == 0) return 0;

            foreach (var values in shapesDic.Values)
            {
                float p = (float)values / shapesDicCount;
                impurity -= p * p;
            }
            return impurity;
        }


        bool[,] Interfaces.IRules.MoveImageToCenter(bool[,] img)
        {
            var (dx, dy) = CenterOfMass(img);

            if (dx == 0 || dy == 0) return img;

            bool[,] newImage = new bool[imageWidth, imageHeight];
            int rows = img.GetLength(0);
            int cols = img.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int newX = x + dx;
                    int newY = y + dy;

                    if (newX >= 0 && newX < imageWidth && newY >= 0 && newY < imageHeight)
                    {
                        newImage[newY, newX] = img[y, x];
                    }
                }
            }

            return newImage;
        }

        void Interfaces.IRules.normalize_minmax(List<float> values)
        {
            float max = float.MinValue;
            float min = float.MaxValue;

            foreach (var v in values)
            {
                if (v > max) max = v;
                if (v < min) min = v;
            }
            float diff = max - min;

            if (diff == 0) return;

            for (int i = 0; i < values.Count; i++)
            {
                values[i] = (values[i] - min) / diff;
            }
        }

        (float, float) Interfaces.IRules.GiniImpurity(List<float> values, List<int> labels)
        {
            var sorted = values.Select((v, idx) => (value: v, label: labels[idx])).OrderBy(x => x.value).ToList();

            float bestAverage = 0;
            float smallestImpurity = float.MaxValue;
            Dictionary<int, int> leftShapesDic = new Dictionary<int, int>();
            Dictionary<int, int> rightShapesDic = new Dictionary<int, int>();

            for (int i = 1; i < values.Count; i++)
            {
                float average = (sorted[i].value + sorted[i - 1].value) / 2;

                for (int l = 0; l < values.Count; l++)
                {
                    if (sorted[l].value < average)
                    {
                        if (leftShapesDic.ContainsKey(sorted[l].label)) leftShapesDic[sorted[l].label]++;
                        else leftShapesDic[sorted[l].label] = 1;
                    }
                    else
                    {
                        if (rightShapesDic.ContainsKey(sorted[l].label)) rightShapesDic[sorted[l].label]++;
                        else rightShapesDic[sorted[l].label] = 1;
                    }
                }

                float leftImpurity = Impurity(leftShapesDic);
                float rightImpurity = Impurity(rightShapesDic);

                float leftCount = leftShapesDic.Values.Sum();
                float rightCount = rightShapesDic.Values.Sum();

                float totalImpurity = leftCount / values.Count * leftImpurity + rightCount / values.Count * rightImpurity;

                if (totalImpurity < smallestImpurity)
                {
                    smallestImpurity = totalImpurity;
                    bestAverage = average;
                }

                leftShapesDic.Clear();
                rightShapesDic.Clear();
            }

            return (bestAverage, smallestImpurity);
        }

        async Task<List<float>> Interfaces.IRules.Field(List<bool[,]> images)
        {
            return await Task.Run(() =>
            {
                List<float> areas = new List<float>(images.Count);
                foreach (var image in images)
                {
                    areas.Add(CalculateSingleFieldSync(image));
                }
                return areas;
            });
        }

        async Task<List<float>> Interfaces.IRules.Circuit(List<bool[,]> images)
        {
            return await Task.Run(() =>
            {
                List<float> perimeters = new List<float>(images.Count);
                foreach (var image in images)
                {
                    perimeters.Add(CalculateSinglePerimeterSync(image));
                }
                return perimeters;
            });
        }

        Task<float> Interfaces.IRules.CalculateSinglePerimeter(bool[,] image)
        {
            return Task.FromResult(CalculateSinglePerimeterSync(image));
        }

        async Task<List<float>> Interfaces.IRules.CalculatePerimeters(List<bool[,]> images)
        {
            return await ((Interfaces.IRules)this).Circuit(images);
        }

        async Task<List<float>> Interfaces.IRules.Width(List<bool[,]> images)
        {
            return await Task.Run(() =>
            {
                List<float> widths = new List<float>(images.Count);
                foreach (var img in images)
                {
                    widths.Add(CalculateSingleWidth(img));
                }
                return widths;
            });
        }

        async Task<List<float>> Interfaces.IRules.Height(List<bool[,]> images)
        {
            return await Task.Run(() =>
            {
                List<float> heights = new List<float>(images.Count);
                foreach (var img in images)
                {
                    heights.Add(CalculateSingleHeight(img));
                }
                return heights;
            });
        }

        async Task<List<float>> Interfaces.IRules.Symetry(List<bool[,]> images)
        {
            return await Task.Run(() =>
            {
                List<float> symmetries = new List<float>(images.Count);
                foreach (var img in images)
                {
                    symmetries.Add(CalculateSingleSymmetry(img));
                }
                return symmetries;
            });
        }

        async Task<List<float>> Interfaces.IRules.Proportion(List<bool[,]> images)
        {
            return await Task.Run(() =>
            {
                List<float> proportions = new List<float>(images.Count);
                foreach (var img in images)
                {
                    proportions.Add(CalculateSingleProportion(img));
                }
                return proportions;
            });
        }
    }
}
