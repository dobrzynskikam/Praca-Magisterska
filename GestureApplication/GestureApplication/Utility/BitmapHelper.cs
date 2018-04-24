using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GestureApplication.Utility
{
    public static class BitmapHelper
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }

        public static Bitmap ToBinaryBitmap(this Bitmap bitmap)
        {
            return Grayscale.CommonAlgorithms.BT709.Apply(bitmap);
        }

        public static Bitmap ThresholdBitmap(this Bitmap bitmap, int threshold)
        {
            return new Threshold(threshold).Apply(bitmap);
        }

        public static Bitmap UseCannyEdgeDetector(this Bitmap bitmap)
        {
            return new CannyEdgeDetector().Apply(bitmap);
        }

        public static Bitmap ColorFilter(this Bitmap bitmap)
        {
            EuclideanColorFiltering euclideanColorFiltering = new EuclideanColorFiltering();
            euclideanColorFiltering.FillColor = new AForge.Imaging.RGB(Color.White);
            euclideanColorFiltering.CenterColor = new AForge.Imaging.RGB(Color.Black);
            euclideanColorFiltering.Radius = 100;
            return euclideanColorFiltering.Apply(bitmap);
        }

        public static Bitmap BiggestBlob(this Bitmap bitmap)
        {
            ExtractBiggestBlob filter = new ExtractBiggestBlob();
            Bitmap biggestBlobImage = filter.Apply(bitmap);
            return biggestBlobImage;
        }
    }
}
