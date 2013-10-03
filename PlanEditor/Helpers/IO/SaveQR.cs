using System.Collections.Generic;
using System.Drawing;
using PlanEditor.Entities;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace PlanEditor.Helpers.IO
{
    public class SaveQR
    {
        public static void GenerateQR(List<QRPointer> pointers)
        {
            var hints = new Dictionary<EncodeHintType, object> { {EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.M} };
            var writer = new QRCodeWriter();

            foreach (var pointer in pointers)
            {
                var matrix = writer.encode(@"http://www.rintd.ru/", BarcodeFormat.QR_CODE, 350, 350, hints);
                var img = new Bitmap(matrix.Width, matrix.Height);
                var graphics = Graphics.FromImage(img);
                graphics.Clear(Color.White);

                for (int i = 0; i < matrix.Height; ++i)
                {
                    for (int j = 0; j < matrix.Height; ++j)
                    {
                        if (matrix[i, j])
                        {
                            graphics.FillRectangle(Brushes.Black, i, j, 1, 1);
                        }
                        else
                        {
                            graphics.FillRectangle(Brushes.White, i, j, 1, 1);
                        }
                    }
                }
                img.Save(@"D:\" + pointer.Code + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
