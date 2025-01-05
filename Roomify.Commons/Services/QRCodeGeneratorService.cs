using System;
using System.Drawing;  // Make sure to add the System.Drawing.Common package
using QRCoder;

namespace Roomify.Commons.Services
{
    public class QRCodeGeneratorService : IQRCodeGeneratorService
{
    public async Task<byte[]> GenerateQRCode(string text)
    {
        byte[] qrCodeBytes = null;

        if (!string.IsNullOrEmpty(text))
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData data = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode bitmap = new BitmapByteQRCode(data);
            qrCodeBytes = bitmap.GetGraphic(20);
        }

        return qrCodeBytes;  // Return the byte array
    }
}

}
