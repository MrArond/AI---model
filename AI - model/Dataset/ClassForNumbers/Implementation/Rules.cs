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
        async Task<List<(double cX, double cY, double area)>> Interfaces.IRules.CalculateCentersAndAreasAsync(List<bool[,]> images)
        {
            var results = new (double cX, double cY, double area)[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);

                    double sumX = 0, sumY = 0, area = 0;
                    for (int y = 0; y < rows; y++)
                    {
                        for (int x = 0; x < cols; x++)
                        {
                            if (img[y, x])
                            {
                                sumX += x;
                                sumY += y;
                                area++;
                            }
                        }
                    }

                    if (area == 0)
                        results[i] = (0, 0, 0);
                    else
                        results[i] = (sumX / area, sumY / area, area);
                });
            });
            return results.ToList();
        }

        async Task<List<float>> Interfaces.IRules.CalculateRadialRatiosAsync(List<bool[,]> images, List<(double cX, double cY, double area)> metrics)
        {
            float[] results = new float[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);
                    var metric = metrics[i];

                    if (metric.area == 0) { results[i] = 0; return; }

                    double cX = metric.cX;
                    double cY = metric.cY;

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

        async Task<List<float>> Interfaces.IRules.CalculateInertiaRatiosAsync(List<bool[,]> images, List<(double cX, double cY, double area)> metrics)
        {
            float[] results = new float[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);
                    var metric = metrics[i];

                    if (metric.area == 0) return;

                    double cX = metric.cX, cY = metric.cY;
                    double m00 = metric.area;
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



        async Task<List<float>> Interfaces.IRules.CalculateCentralSymmetryAsync(List<bool[,]> images, List<(double cX, double cY, double area)> metrics)
        {
            float[] results = new float[images.Count];
            await Task.Run(() =>
            {
                Parallel.For(0, images.Count, i =>
                {
                    var img = images[i];
                    int rows = img.GetLength(0);
                    int cols = img.GetLength(1);
                    var metric = metrics[i];

                    if (metric.area == 0) { results[i] = 0; return; }

                    double cX = metric.cX;
                    double cY = metric.cY;

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
    }
}
