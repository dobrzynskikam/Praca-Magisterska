using Accord.Imaging;
using Accord.Imaging.Filters;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace MasterThesisApplication.Model.Utility
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
            bi.Freeze();
            return bi;
        }

        public static Bitmap BitmapImage2Bitmap(this BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static Bitmap DrawRectangle(this Bitmap bitmap, Rectangle rectangle)
        {
            RectanglesMarker marker = new RectanglesMarker(Color.Crimson);
            marker.SingleRectangle = rectangle;
            BitmapData imgData = bitmap.LockBits(ImageLockMode.ReadWrite);
            //Accord.Imaging.Drawing.Rectangle(imgData, tempRectangle, Color.Crimson);
            UnmanagedImage img = new UnmanagedImage(imgData);

            var rectangleBitmap = marker.Apply(img);
            return  rectangleBitmap.ToManagedImage();
        }
    }
}
