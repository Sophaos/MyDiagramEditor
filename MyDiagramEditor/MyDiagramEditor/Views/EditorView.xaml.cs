using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using MyDiagramEditor.ViewModels;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;

namespace MyDiagramEditor.Views
{
    /// <summary>
    /// Logique d'interaction pour FenetreDessin.xaml
    /// </summary>
    /// 
    public partial class EditorView
    {
        public EditorView()
        {
            InitializeComponent();
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            ItemsControl control = this.Items; //this is the name of the ListBox
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            Rect bounds = VisualTreeHelper.GetDescendantBounds(control);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(control);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }
            rtb.Render(dv);
            SaveFileDialog dlg = new SaveFileDialog();
            //Encoder
            dlg.FileName = "ExportedCanvas";
            dlg.Filter =
                "(Jpeg) Joint Photographic Experts Group Image|*.jpg|" +
                "(BMP) Bitmap Image|*.bmp|" +
                "(PNG) Portable Network Graphics Image|*.png|" +
                "(TIFF) Tagged Image File Format Image|*.tiff";
            if (dlg.ShowDialog() == true)
            {
                string ext = System.IO.Path.GetExtension(dlg.FileName);

                switch (ext)
                {
                    case ".jpg":
                        JpegBitmapEncoder jpeg = new JpegBitmapEncoder();
                        jpeg.Frames.Add(BitmapFrame.Create(rtb));
                        using (var stream = dlg.OpenFile())
                        {
                            jpeg.Save(stream);
                        }
                        break;
                    case ".bmp":
                        BmpBitmapEncoder bmp = new BmpBitmapEncoder();
                        bmp.Frames.Add(BitmapFrame.Create(rtb));
                        using (var stream = dlg.OpenFile())
                        {
                            bmp.Save(stream);
                        }
                        break;
                    case ".png":
                        PngBitmapEncoder png = new PngBitmapEncoder();
                        png.Frames.Add(BitmapFrame.Create(rtb));
                        using (var stream = dlg.OpenFile())
                        {
                            png.Save(stream);
                        }
                        break;
                    case ".tiff":
                        TiffBitmapEncoder tiff = new TiffBitmapEncoder();
                        tiff.Frames.Add(BitmapFrame.Create(rtb));
                        using (var stream = dlg.OpenFile())
                        {
                            tiff.Save(stream);
                        }
                        break;
                    default:
                        return;
                }

            }
        }
    }
}
