using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AI___model.Dataset.ClassForNumbers.Implementation
{
    //cos
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
            double perimeter = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (!image[y, x]) continue;

                    bool n = (y > 0) && image[y - 1, x];
                    bool s = (y < rows - 1) && image[y + 1, x];
                    bool w = (x > 0) && image[y, x - 1];
                    bool e = (x < cols - 1) && image[y, x + 1];

                    if (!n) perimeter += 1.0;
                    if (!s) perimeter += 1.0;
                    if (!w) perimeter += 1.0;
                    if (!e) perimeter += 1.0;

                    bool ne = (y > 0 && x < cols - 1) && image[y - 1, x + 1];
                    bool nw = (y > 0 && x > 0) && image[y - 1, x - 1];
                    bool se = (y < rows - 1 && x < cols - 1) && image[y + 1, x + 1];
                    bool sw = (y < rows - 1 && x > 0) && image[y + 1, x - 1];

                    if (!n && !w && !nw) perimeter += 0.414;
                    if (!n && !e && !ne) perimeter += 0.414;
                    if (!s && !w && !sw) perimeter += 0.414;
                    if (!s && !e && !se) perimeter += 0.414;
                }
            }
            return (float)perimeter;
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

        // --- NEW OR UPDATED PRECISE MATHEMATICAL FEATURES ---

        async Task<List<float>> Interfaces.IRules.CalculateCircularity(List<float> areas, List<float> perimeters)
        {
            return await Task.Run(() =>
            {
                int count = Math.Min(areas.Count, perimeters.Count);
                if (count <= 0) return new List<float>();

                var circularityList = new List<float>(count);
                const float fourPi = 4f * (float)Math.PI;

                for (int i = 0; i < count; i++)
                {
                    float area = areas[i];
                    float perimeter = perimeters[i];
                    if (perimeter > 0)
                    {
                        float circularity = (fourPi * area) / (perimeter * perimeter);
                        circularityList.Add(circularity);
                    }
                    else
                    {
                        circularityList.Add(0f);
                    }
                }
                return circularityList;
            });
        }

        async Task<List<float>> Interfaces.IRules.CalculateRadialRatiosAsync(List<bool[,]> images)
        {
            float[] results = new float[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);

                    long sumX = 0, sumY = 0, count = 0;
                    for (int y = 0; y < rows; y++)
                        for (int x = 0; x < cols; x++)
                            if (img[y, x]) { sumX += x; sumY += y; count++; }

                    if (count == 0) { results[i] = 0; return; }
                    float cX = (float)sumX / count;
                    float cY = (float)sumY / count;

                    double maxD = 0;
                    double minD = double.MaxValue;

                    for (int y = 0; y < rows; y++)
                    {
                        for (int x = 0; x < cols; x++)
                        {
                            if (img[y, x])
                            {
                                if (y == 0 || !img[y - 1, x] || y == rows - 1 || !img[y + 1, x] ||
                                    x == 0 || !img[y, x - 1] || x == cols - 1 || !img[y, x + 1])
                                {
                                    double dist = Math.Sqrt(Math.Pow(x - cX, 2) + Math.Pow(y - cY, 2));
                                    if (dist > maxD) maxD = dist;
                                    if (dist < minD) minD = dist;
                                }
                            }
                        }
                    }
                    results[i] = (minD > 0) ? (float)(maxD / minD) : 0;
                });
            });
            return results.ToList();
        }

        async Task<List<float>> Interfaces.IRules.CalculateExtentsAsync(List<bool[,]> images, List<float> areas)
        {
            float[] results = new float[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);

                    int minX = cols, maxX = 0, minY = rows, maxY = 0;

                    for (int y = 0; y < rows; y++)
                    {
                        for (int x = 0; x < cols; x++)
                        {
                            if (img[y, x])
                            {
                                if (x < minX) minX = x; if (x > maxX) maxX = x;
                                if (y < minY) minY = y; if (y > maxY) maxY = y;
                            }
                        }
                    }

                    float width = maxX - minX + 1;
                    float height = maxY - minY + 1;
                    float boundingBoxArea = width * height;

                    results[i] = (boundingBoxArea > 0) ? areas[i] / boundingBoxArea : 0;
                });
            });
            return results.ToList();
        }

        async Task<List<float>> Interfaces.IRules.CalculateInertiaRatiosAsync(List<bool[,]> images)
        {
            float[] results = new float[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);

                    double m00 = 0, m10 = 0, m01 = 0;
                    for (int y = 0; y < rows; y++)
                        for (int x = 0; x < cols; x++)
                            if (img[y, x]) { m00++; m10 += x; m01 += y; }

                    if (m00 == 0) return;
                    double cX = m10 / m00, cY = m01 / m00;
                    double mu20 = 0, mu02 = 0, mu11 = 0;

                    for (int y = 0; y < rows; y++)
                        for (int x = 0; x < cols; x++)
                            if (img[y, x])
                            {
                                mu20 += Math.Pow(x - cX, 2);
                                mu02 += Math.Pow(y - cY, 2);
                                mu11 += (x - cX) * (y - cY);
                            }

                    double common = Math.Sqrt(Math.Pow(mu20 - mu02, 2) + 4 * Math.Pow(mu11, 2));
                    double axisMajor = Math.Sqrt(2 * (mu20 + mu02 + common) / m00);
                    double axisMinor = Math.Sqrt(2 * (mu20 + mu02 - common) / m00);
                    results[i] = (axisMinor > 0) ? (float)(axisMajor / axisMinor) : 1f;
                });
            });
            return results.ToList();
        }

        async Task<List<int>> Interfaces.IRules.CalculatePeakCountsAsync(List<bool[,]> images)
        {
            int[] results = new int[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);

                    double m00 = 0, m10 = 0, m01 = 0;
                    for (int y = 0; y < rows; y++)
                        for (int x = 0; x < cols; x++)
                            if (img[y, x]) { m00++; m10 += x; m01 += y; }

                    if (m00 == 0) { results[i] = 0; return; }
                    double cX = m10 / m00;
                    double cY = m01 / m00;

                    float[] radial = new float[360];
                    for (int y = 0; y < rows; y++)
                    {
                        for (int x = 0; x < cols; x++)
                        {
                            if (!img[y, x]) continue;
                            double dx = x - cX;
                            double dy = y - cY;
                            int angle = (int)((Math.Atan2(dy, dx) * 180.0 / Math.PI) + 360) % 360;
                            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                            if (dist > radial[angle]) radial[angle] = dist;
                        }
                    }

                    float[] smooth = new float[360];
                    int window = 7;
                    for (int a = 0; a < 360; a++)
                    {
                        float sum = 0; int count = 0;
                        for (int k = -window; k <= window; k++)
                        {
                            int idx = (a + k + 360) % 360;
                            sum += radial[idx]; count++;
                        }
                        smooth[a] = sum / count;
                    }

                    float maxVal = smooth.Max();
                    if (maxVal > 0)
                    {
                        for (int a = 0; a < 360; a++) smooth[a] /= maxVal;
                    }

                    int peaks = 0;
                    float threshold = 0.2f;
                    for (int a = 0; a < 360; a++)
                    {
                        int prev = (a - 1 + 360) % 360;
                        int next = (a + 1) % 360;
                        if (smooth[a] > smooth[prev] && smooth[a] > smooth[next] && smooth[a] > threshold) peaks++;
                    }
                    results[i] = peaks;
                });
            });
            return results.ToList();
        }

        async Task<List<float>> Interfaces.IRules.CalculateCentralSymmetryAsync(List<bool[,]> images)
        {
            float[] results = new float[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);

                    double m10 = 0, m01 = 0, m00 = 0;
                    for (int y = 0; y < rows; y++)
                        for (int x = 0; x < cols; x++)
                            if (img[y, x]) { m00++; m10 += x; m01 += y; }

                    if (m00 == 0) { results[i] = 0; return; }
                    double cX = m10 / m00;
                    double cY = m01 / m00;

                    int symmetricPoints = 0;
                    int totalPoints = 0;

                    for (int y = 0; y < rows; y++)
                    {
                        for (int x = 0; x < cols; x++)
                        {
                            if (img[y, x])
                            {
                                totalPoints++;
                                int oppX = (int)Math.Round(2 * cX - x);
                                int oppY = (int)Math.Round(2 * cY - y);

                                if (oppX >= 0 && oppX < cols && oppY >= 0 && oppY < rows)
                                    if (img[oppY, oppX]) symmetricPoints++;
                            }
                        }
                    }
                    results[i] = (float)symmetricPoints / totalPoints;
                });
            });
            return results.ToList();
        }

        (float threshold, float impurity) Interfaces.IRules.GiniImpurityFast(List<float> values, List<int> labels)
        {
            int n = values.Count;

            var sorted = values
                .Select((v, i) => (value: v, label: labels[i]))
                .OrderBy(x => x.value)
                .ToList();

            Dictionary<int, int> rightCounts = new();
            foreach (var l in labels)
            {
                if (rightCounts.ContainsKey(l)) rightCounts[l]++;
                else rightCounts[l] = 1;
            }

            Dictionary<int, int> leftCounts = new();
            float bestThreshold = 0;
            float bestImpurity = float.MaxValue;

            int leftSize = 0;
            int rightSize = n;

            for (int i = 0; i < n - 1; i++)
            {
                var label = sorted[i].label;

                if (!leftCounts.ContainsKey(label)) leftCounts[label] = 0;
                leftCounts[label]++;

                rightCounts[label]--;
                if (rightCounts[label] == 0) rightCounts.Remove(label);

                leftSize++;
                rightSize--;

                if (sorted[i].value == sorted[i + 1].value) continue;

                float threshold = (sorted[i].value + sorted[i + 1].value) / 2;

                float leftImp = ComputeFastGini(leftCounts, leftSize);
                float rightImp = ComputeFastGini(rightCounts, rightSize);

                float totalImp = (leftSize / (float)n) * leftImp + (rightSize / (float)n) * rightImp;

                if (totalImp < bestImpurity)
                {
                    bestImpurity = totalImp;
                    bestThreshold = threshold;
                }
            }
            return (bestThreshold, bestImpurity);
        }

        private float ComputeFastGini(Dictionary<int, int> counts, int total)
        {
            if (total == 0) return 0;
            float impurity = 1f;
            foreach (var c in counts.Values)
            {
                float p = c / (float)total;
                impurity -= p * p;
            }
            return impurity;
        }

        void Interfaces.IRules.StandardScale(List<float> values)
        {
            int n = values.Count;
            if (n == 0) return;

            double sum = 0;
            for (int i = 0; i < n; i++) sum += values[i];
            double mean = sum / n;

            double variance = 0;
            for (int i = 0; i < n; i++)
            {
                double diff = values[i] - mean;
                variance += diff * diff;
            }
            variance /= n;
            double std = Math.Sqrt(variance);

            if (std == 0) return;

            for (int i = 0; i < n; i++)
            {
                values[i] = (float)((values[i] - mean) / std);
            }
        }
    }
}
