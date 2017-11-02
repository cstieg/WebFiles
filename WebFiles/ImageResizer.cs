using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Helpers;

namespace Cstieg.WebFiles
{
    /// <summary>
    /// A utility to resize image streams
    /// </summary>
    public class ImageResizer
    {
        WebImage _webImage;

        /// <summary>
        /// Constructor for ImageResizer setting the image stream to manipulate.
        /// Creates a clone of the stream so not to consume the original
        /// </summary>
        /// <param name="imageStream">The image stream to manipulate</param>
        public ImageResizer(Stream imageStream )
        {
            _webImage = new WebImage(imageStream.CloneToMemoryStream());
            FixOrientation(imageStream.CloneToMemoryStream());
        }

        /// <summary>
        /// Gets a stream containing the image resized to a given width
        /// </summary>
        /// <param name="width">The width in pixels which to resize the image</param>
        /// <returns>A stream with the resized image</returns>
        public Stream GetResizedImageStream(int width)
        {
            return GetResizedImage(width).GetImageStream();
        }

        /// <summary>
        /// Gets a WebImage object containing the image resized to a given width
        /// </summary>
        /// <param name="width">The width in pixels which to resize the image</param>
        /// <returns>The resized WebImage object</returns>
        public WebImage GetResizedImage(int width)
        {
            int originalWidth = _webImage.Width;
            int originalHeight = _webImage.Height;
            float aspectRatio = (float)(originalWidth) / originalHeight;
            int height = (int)(originalWidth / aspectRatio);
            WebImage resizedImage = _webImage.Resize(width, height, true);
            return resizedImage;
        }

        /// <summary>
        /// Saves the resized image to disk
        /// </summary>
        /// <param name="filePath">The filepath where to save the resized image</param>
        /// <param name="width">The width in pixels which to save the image</param>
        public void SaveResizedImage(string filePath, int width)
        {
            GetResizedImage(width);
            SaveAs(filePath);
        }

        /// <summary>
        /// Saves the image to disk
        /// </summary>
        /// <param name="filePath">The filepath where to save the image</param>
        public void SaveAs(string filePath)
        {
            _webImage.FileName = filePath;
            _webImage.Save();
        }

        /// <summary>
        /// Gets the width of the image in pixels
        /// </summary>
        /// <returns>The width of the image in pixels</returns>
        public int GetImageWidth()
        {
            return _webImage.Width;
        }

        // Code adapted from ReenignE, https://stackoverflow.com/questions/6222053/problem-reading-jpeg-metadata-orientation
        /// <summary>
        /// Fixes the orientation of an image
        /// </summary>
        /// <param name="imageStream">Image stream of image to fix</param>
        /// <returns>Fixed image stream</returns>
        public void FixOrientation(Stream imageStream)
        {
            Image img = Image.FromStream(imageStream);
            
            if (Array.IndexOf(img.PropertyIdList, 274) > -1)
            {
                var orientation = (int)img.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        return;
                    case 2:
                        _webImage = _webImage.FlipHorizontal();
                        //img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        _webImage = _webImage.RotateRight().RotateRight();
                        //img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        _webImage = _webImage.RotateRight().RotateRight().FlipHorizontal();
                        //img.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        _webImage = _webImage.RotateRight().FlipHorizontal();
                        //img.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        _webImage = _webImage.RotateRight();
                        //img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        _webImage = _webImage.RotateLeft().FlipHorizontal();
                        //img.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        _webImage = _webImage.RotateLeft();
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                //img.RemovePropertyItem(274);
            }
        }
    }

    /// <summary>
    /// A helper class to add extensions to WebImage
    /// </summary>
    public static class WebImageHelper
    {
        /// <summary>
        /// An extension to convert the WebImage object to a Stream object
        /// </summary>
        /// <param name="webImage">The WebImage object to convert</param>
        /// <returns>A Stream object containing the image data</returns>
        public static Stream GetImageStream(this WebImage webImage)
        {
            return new MemoryStream(webImage.GetBytes());
        }
    }
}