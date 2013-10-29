using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThePaperWall.WP8.Fixins
{
    public class BackgroundImageDownloader
    {

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(string), typeof(BitmapImage), new PropertyMetadata(null, callback));


        public static void SetSource(DependencyObject element, string value)
        {
            element.SetValue(SourceProperty, value);
        }

        public static string GetSource(DependencyObject element)
        {
            return (string)element.GetValue(SourceProperty);
        }

        private static async void callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panorama panorama = d as Panorama;

            if (panorama != null)
            {
                var path = e.NewValue as string;
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (path.StartsWith("isostore:/"))
                        {
                            string localPath = path.Substring(10);
                            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                            {
                                using (IsolatedStorageFileStream stream = store.OpenFile(localPath, FileMode.Open))
                                {
                                    BitmapImage image = new BitmapImage();
                                    image.SetSource(stream);
                                    ImageBrush brush = new ImageBrush
                                    {
                                        Opacity = 0.3,
                                        Stretch = Stretch.UniformToFill,
                                        ImageSource = image,

                                    };
                                    panorama.Background = brush;
                                }
                            }
                        }
                        else
                        {
                            BitmapImage image = new BitmapImage(new Uri(path, UriKind.Absolute));
                            ImageBrush brush = new ImageBrush
                            {
                                Opacity = 0.3,
                                Stretch = Stretch.UniformToFill,
                                ImageSource = image,

                            };
                            panorama.Background = brush;
                        }
                    }
                }
            }
        }
    }
    //public class BackgroundImageDownloader
    //{

    //    public static readonly DependencyProperty SourceProperty =
    //        DependencyProperty.RegisterAttached("Source", typeof(ImageSource), typeof(BitmapImage), new PropertyMetadata(null, callback));


    //    public static void SetSource(DependencyObject element, string value)
    //    {
    //        element.SetValue(SourceProperty, value);
    //    }

    //    public static string GetSource(DependencyObject element)
    //    {
    //        return (string)element.GetValue(SourceProperty);
    //    }

    //    private static async void callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        Panorama panorama = d as Panorama;

    //        if (panorama != null)
    //        {
    //          var imageSource = e.NewValue as ImageSource;
    //          ImageBrush brush = new ImageBrush
    //                                {
    //                                    Opacity = 0.3,
    //                                    Stretch = Stretch.UniformToFill,
    //                                    ImageSource = imageSource,

    //                                };
    //          panorama.Background = brush;
    //        }
    //    }
    //}
}
