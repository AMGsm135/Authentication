using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace Amg.Authentication.Application.Services
{
    public class CaptchaManager : ICaptchaManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CaptchaSettings _captchaSettings;

        public CaptchaManager(IOptions<CaptchaSettings> captchaSettings, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _captchaSettings = captchaSettings.Value;
        }

        public CaptchaItem GenerateCaptcha()
        {
            var random = new Random();
            var captchaText = random.Next(10000, 99999).ToString();
            var id = RegisterCaptcha(captchaText);
            var image = CreateCaptchaImage(captchaText);
            return new CaptchaItem()
            {
                CaptchaId = id,
                CaptchaImage = image,
            };
        }

        public void ValidateCaptcha(ICaptchaValidation captcha)
        {
            if (!_captchaSettings.IsEnabled)
                return;

            if (captcha == null)
                throw new ValidationException("کد امنیتی صحیح نمی باشد");

            if (_captchaSettings.DevelopmentMode)
            {
                // a.ammari : در حالت توسعه، برای دریافت توکن توسط پُستمن و عمیات تست
                // از یک شناسه و کد کپچای از پیش تعریف شده و ثابت استفاده می کنیم.
                if (string.Equals(_captchaSettings.DevelopmentCaptchaId, captcha.CaptchaId) &&
                    string.Equals(_captchaSettings.DevelopmentCaptchaCode, captcha.CaptchaCode))
                    return;
            }

            if (string.IsNullOrEmpty(captcha.CaptchaId) || string.IsNullOrEmpty(captcha.CaptchaCode))
                throw new ValidationException("کد امنیتی صحیح نمی باشد");

            if (!_memoryCache.TryGetValue(captcha.CaptchaId, out string code) ||
                !string.Equals(captcha.CaptchaCode, code, StringComparison.Ordinal))
            {
                _memoryCache.Remove(captcha.CaptchaId);
                throw new ValidationException("کد امنیتی صحیح نمی باشد");
            }

            _memoryCache.Remove(captcha.CaptchaId);
        }

        public CaptchaItem RefreshCaptcha(string oldCaptchaId)
        {
            if (!string.IsNullOrEmpty(oldCaptchaId))
                _memoryCache.Remove(oldCaptchaId);
            return GenerateCaptcha();
        }

        private string RegisterCaptcha(string captchaCode)
        {
            var id = Guid.NewGuid().ToString("n");
            _memoryCache.Set(id, captchaCode,
                TimeSpan.FromMinutes(_captchaSettings.Timeout));
            return id;
        }

        /*private byte[] CreateCaptchaImage2(string captchaText)
        {
            var random = new Random();
            var fontSizes = new[] { 25, 30, 35, 40 };
            var fontNames = new[] { "Arial", "Comic Sans MS", "Verdana", "Trebuchet MS", "Georgia", "Times New Roman", "Courier New", "Tahoma", "Segoe UI", "Impact" };
            var fontStyles = Enum.GetValues(typeof(SKFontStyleWeight)).Cast<SKFontStyleWeight>().ToArray();
            var hatchStyles = Enum.GetValues(typeof(SKHatchStyle)).Cast<SKHatchStyle>().ToArray();

            // Create a surface for drawing
            using var surface = SKSurface.Create(new SKImageInfo(190, 80));
            var canvas = surface.Canvas;

            // Draw background (Lighter colors RGB 100 to 255)
            var brush = new SKPaint
            {
                Shader = SKShader.CreateHatchPattern(hatchStyles[random.Next(hatchStyles.Length - 1)], SKColors.Black, SKColors.White),
            };
            canvas.DrawRect(0, 0, surface.Width, surface.Height, brush);

            for (var i = 0; i < captchaText.Length; i++)
            {
                var x = surface.Width / (captchaText.Length + 1) * i;

                // Rotate text Random
                var rotation = random.Next(-40, 40);
                var textPaint = new SKPaint
                {
                    Typeface = SKTypeface.FromFamilyName(fontNames[random.Next(fontNames.Length - 1)], SKTypefaceStyle.Normal),
                    TextSize = fontSizes[random.Next(fontSizes.Length - 1]),
                    IsAntialias = true,
                    Color = SKColor.FromHsl(random.Next(360), 80, 50),
                };
                canvas.RotateDegrees(rotation, x, surface.Height / 2);

                // Draw the letters with Random Font Type, Size, and Color
                canvas.DrawText(captchaText.Substring(i, 1), x, random.Next(10, 40), textPaint);
                canvas.ResetMatrix();
            }

            using var image = surface.Snapshot();
            using var stream = new SKMemoryWStream();
            image.Encode(stream, SKEncodedImageFormat.Png, 100);
            var bytes = stream.ToArray();
            return bytes;
        }*/

        private byte[] CreateCaptchaImage(string captchaText)
        {
            var random = new Random();
            var fontSizes = new[] { 30, 35, 45, 50 };
            var fontNames = new[] { "Arial", "Comic Sans MS", "Verdana", "Trebuchet MS", "Georgia", "Times New Roman", "Courier New", "Tahoma", "Segoe UI", "Impact" };

            // Create a drawing surface
            using var surface = SKSurface.Create(new SKImageInfo(190, 80));
            var canvas = surface.Canvas;
            
            // Background color
            var backgroundColor = SKColor.FromHsl(0, 0, 90);
            canvas.Clear(backgroundColor);

            using var paint = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName(fontNames[random.Next(fontNames.Length - 1)], SKTypefaceStyle.Bold),
                TextAlign = SKTextAlign.Center,
                Color = SKColor.FromHsl(random.Next(360), 80, 50),
                IsAntialias = true,
                TextSize = 45/*fontSizes[random.Next(fontSizes.Length - 1)]*/,
            };

            var textBounds = new SKRect();
            paint.MeasureText(captchaText, ref textBounds);

            // Draw text in the center
            var x = canvas.LocalClipBounds.Width / 2;
            var y = canvas.LocalClipBounds.Height * 2 /3;
            canvas.DrawText(captchaText, x, y, paint);

            // Convert the drawing to a PNG image
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();

            //var random = new Random();
            //var fontSizes = new[] { 25, 30, 35, 40 };
            //var fontNames = new[] { "Arial", "Comic Sans MS", "Verdana", "Trebuchet MS",
            //    "Georgia", "Times New Roman", "Courier New", "Tahoma", "Segoe UI", "Impact" };
            //var fontStyles = Enum.GetValues(typeof(FontStyle)).Cast<FontStyle>().ToArray();
            //var hatchStyles = Enum.GetValues(typeof(HatchStyle)).Cast<HatchStyle>().ToArray();

            ////Creates an output Bitmap
            //using var bitmap = new Bitmap(190, 80, PixelFormat.Format24bppRgb);
            //var graphics = Graphics.FromImage(bitmap);
            //graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            ////Create a Drawing area
            //var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            ////Draw background (Lighter colors RGB 100 to 255)
            //var brush = new HatchBrush(
            //    hatchStyles[random.Next(hatchStyles.Length - 1)],
            //    Color.FromArgb(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255)),
            //    Color.White);
            //graphics.FillRectangle(brush, rect);
            //var matrix = new Matrix();
            //for (var i = 0; i <= captchaText.Length - 1; i++)
            //{
            //    matrix.Reset();
            //    var textLength = captchaText.Length;
            //    var x = bitmap.Width / (textLength + 1) * i;

            //    //Rotate text Random
            //    matrix.RotateAt(random.Next(-40, 40), new Point(x, bitmap.Height / 2));
            //    graphics.Transform = matrix;

            //    //Draw the letters with Random Font Type, Size and Color
            //    graphics.DrawString(captchaText.Substring(i, 1),
            //        new Font(fontNames[random.Next(fontNames.Length - 1)],
            //            fontSizes[random.Next(fontSizes.Length - 1)],
            //            fontStyles[random.Next(fontStyles.Length - 1)]),
            //        new SolidBrush(Color.FromArgb(random.Next(0, 155), random.Next(0, 155), random.Next(0, 155))),
            //        x, random.Next(10, 40));

            //    graphics.ResetTransform();
            //}

            //using var stream = new MemoryStream();
            ////bitmap.Save(stream, ImageFormat.Png);
            ////var bytes = stream.ToArray();
            ////return bytes;
            //return stream.ToArray();
        }

    }
}
