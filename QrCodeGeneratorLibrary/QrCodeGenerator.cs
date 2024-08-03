using System;
using System.IO;
using Net.Codecrete.QrCodeGenerator;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace QrCodeGeneratorLibrary
{
    public class QrCodeGenerator
    {
        /// <summary>
        /// Generates a QR code from the provided data and returns it as a byte array in PNG format.
        /// </summary>
        /// <param name="data">The data to encode in the QR code.</param>
        /// <returns>A byte array representing the QR code image in PNG format.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided data is null or empty.</exception>
        public byte[] GenerateQrCode(string data)
        {
            // Validate input data
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data), "Data cannot be null or empty.");

            // Encode the provided text into a QR code
            var qrCode = QrCode.EncodeText(data, QrCode.Ecc.Medium);

            // Convert the QR code into a PNG image byte array
            return RenderQrCodeToPng(qrCode);
        }

        /// <summary>
        /// Renders the QR code to a PNG byte array using ImageSharp.
        /// </summary>
        /// <param name="qrCode">The QR code object to render.</param>
        /// <returns>A byte array representing the QR code image in PNG format.</returns>
        private byte[] RenderQrCodeToPng(QrCode qrCode)
        {
            int scale = 10; // Scale factor to enlarge the QR code
            int border = 4; // Border size around the QR code in modules
            int size = qrCode.Size * scale + border * 2 * scale; // Total image size

            // Create a new image with the calculated size
            using (var image = new Image<Rgba32>(size, size))
            {
                // Mutate the image context to modify its pixels
                image.Mutate(ctx =>
                {
                    // Set the background to white
                    ctx.Clear(Color.White);

                    // Iterate through each module in the QR code
                    for (int y = 0; y < qrCode.Size; y++)
                    {
                        for (int x = 0; x < qrCode.Size; x++)
                        {
                            // Draw a black rectangle for each dark module
                            if (qrCode.GetModule(x, y))
                            {
                                ctx.Fill(Color.Black, new Rectangle((x + border) * scale, (y + border) * scale, scale, scale));
                            }
                        }
                    }
                });

                // Save the image as a PNG file in a memory stream and return the byte array
                using (var ms = new MemoryStream())
                {
                    image.SaveAsPng(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
