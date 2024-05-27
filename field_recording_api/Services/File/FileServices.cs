using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.FileModel;
using field_recording_api.Models.HttpModel;
using field_recording_api.Services.Logger;
using field_recording_api.Utilities;
using log4net;
using Microsoft.AspNetCore.Routing;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace field_recording_api.Services.File
{
    public class FileServices : IFileServices
    {
        private readonly ILoggerServices _logService;
        private readonly IConfiguration _config;
        private readonly IRepository<TbtImg> _tbt_img;
        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;
        public FileServices(
            ILoggerServices logService,
            IRepository<TbtImg> tbt_img,
            IConfiguration config) {
            _logService = logService;
            _config = config;
            _tbt_img = tbt_img;
        }
        public async Task<ResponseContext> Upload(FileModel Req)
        {
            var _resview = new ResponseContext();

            try
            {
                _logService.Info("Service Upload: Start");
                var pathUploadsfromConfig = _config.GetSection("UploadDir:path").Value;
                var maindir = !string.IsNullOrEmpty(pathUploadsfromConfig) ? pathUploadsfromConfig : Environment.CurrentDirectory;
                string path = Path.Combine(maindir, "Images", "Uploads");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var count = 1;
                foreach (IFormFile postedFile in Req.inputFile)
                {
                    var fileeee = postedFile;

                    if (fileeee.Length == 0) throw new Exception("Length File is Zero");
                    var image = Image.FromStream(fileeee.OpenReadStream());
                    //PropertyItem pi = image.GetPropertyItem(0x112);


                    //image.SetPropertyItem(BitConverter.ToInt16(pi.Value, 0));
                    //var resized = new Bitmap(image);
                    _logService.Info(string.Format("Height before rotate {0}", image.Height));
                    _logService.Info(string.Format("Width before rotate {0}", image.Width));
                    dynamic resized = FixedSize(image);
                    _logService.Info(string.Format("Height after rotate {0}", resized.Height));
                    _logService.Info(string.Format("Width after rotate {0}", resized.Width));
                    if (image.Width > 800)
                    {

                        decimal newHeight = 800 * image.Height / image.Width;
                        //FixedSize(image, newHeight);
                        //decimal newWidth = dWidth * sourceHeight / sourceWidth;
                        resized = new Bitmap(image, new Size(800, (int)Math.Ceiling(newHeight)));
                        //resized = FixedSize(image, newHeight);
                        //resized = new Bitmap(image);
                        //resized.SetResolution(72, 72);
                        //resized = FixedSize(resized, newHeight);
                    }
                    else {
                        resized = new Bitmap(image);
                    }
                    //else {
                    //    resized = new Bitmap(image);
                    //    resized.SetResolution(72, 72);
                    //}
                    resized.SetResolution(72, 72);



                    //var pi = (PropertyItem)FormatterServices
                    //    .GetUninitializedObject(typeof(PropertyItem));

                    //pi.Id = 0x0112;   // orientation
                    //pi.Len = 2;
                    //pi.Type = 3;
                    //pi.Value = new byte[2] { 1, 0 };

                    ////pi.Value[0] = image.GetPropertyItem(0x0112).Value[0];

                    //image.SetPropertyItem(pi);

                    //Rotate(resized);

                    //ExifRotate(image);

                    var contract_no = string.Format("Contract No: {0}", Req.contract_no);
                    var address = Req.adress;
                    var addressText = string.Format("GEO: {0}, {1}, {2}, {3}", address.subDistrict, address.district, address.province, address.zipCode);

                    var lat_lon_text = string.Format("lat lon : {0}", address.latlong);

                    var textHieght = resized.Height / 30;
                    var textWidth = 0;//resized.Width / 3;
                    decimal newHeightt = 800 * image.Height / image.Width;
                    using var imageStream = new MemoryStream();
                    using (var graphic = Graphics.FromImage(resized))
                    {
                        using (Font font1 = new Font(FontFamily.GenericSansSerif.ToString(), 60, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            Rectangle rect1 = new Rectangle(textWidth, resized.Height - ((resized.Height * 21) / 100) , resized.Width - textWidth, resized.Height);
                            Rectangle rect2 = new Rectangle(textWidth, resized.Height - ((resized.Height * 14) / 100), resized.Width - textWidth, resized.Height);

                            Rectangle rect3 = new Rectangle(textWidth, resized.Height - ((resized.Height * 7) / 100), resized.Width - textWidth, resized.Height);

                            var color = Color.DarkBlue;//Color.FromArgb(200, 0, 0, 255); //Color.FromArgb(64, 255, 0, 0);
                            var brush = new SolidBrush(color);
                            //StringFormat stringFormat = new StringFormat();
                            //stringFormat.Alignment = StringAlignment.Center;
                            //stringFormat.LineAlignment = StringAlignment.Center;
                            //graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                            Font font_contract_no = GetFontSize(graphic, contract_no, rect1.Size, font1);
                            Font font_address = GetFontSize(graphic, addressText, rect2.Size, font1);

                            //graphic.DrawString(addressText, font_contract_no, brush, rect1); //stringFormat
                            graphic.DrawString(lat_lon_text, font_contract_no, brush, rect2);
                            graphic.DrawString(contract_no, font_contract_no, brush, rect3);
                        }
;
                        resized.Save(imageStream, ImageFormat.Jpeg);
                    }

                    var imageBytes = imageStream.ToArray();
                    var imgName = string.Format("{0}_{1}_{2}_{3}", Req.contract_no, DateTime.Now.ToString("yyyyMMddHHmmss"), Req.contract_note_id, count.ToString()) + ".jpg";
                    using (var stream = new FileStream(Path.Combine(path, imgName), FileMode.Create, FileAccess.Write, FileShare.Write, 4096))
                    {
                        stream.Write(imageBytes, 0, imageBytes.Length);
                    }

                    var resltInsImg = await InstbtImg(Req.contract_no, imgName, path, Req.contract_note_id, count);
                    if (resltInsImg.statusCode != "200")
                    {
                        return resltInsImg;
                    }


                    //var fileeee = postedFile;
                    //var image = Image.FromStream(fileeee.OpenReadStream());
                    //var drawing = new ResizeTransformation(image.Height, image.Width, true, false);
                    //var imgReDraw = drawing.ApplyTransformation(image);
                    //using var imageStream = new MemoryStream();
                    //imgReDraw.Save(imageStream, ImageFormat.Jpeg);
                    //var imageBytes = imageStream.ToArray();
                    //var imgName = string.Format("{0}_{1}_{2}_{3}", Req.contract_no, DateTime.Now.ToString("yyyyMMddHHmmss"), Req.contract_note_id, count.ToString()) + ".jpg";
                    //using (var stream = new FileStream(Path.Combine(path, imgName), FileMode.Create, FileAccess.Write, FileShare.Write, 4096))
                    //{
                    //    stream.Write(imageBytes, 0, imageBytes.Length);
                    //}

                    //var resltInsImg = await InstbtImg(Req.contract_no, imgName, path, Req.contract_note_id, count);
                    //if (resltInsImg.statusCode != "200")
                    //{
                    //    return resltInsImg;
                    //}

                    count++;
                }

                _logService.Info("Service Upload: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = ex.Message;
                _logService.Info(string.Format("Service Upload Error: {0}", ex.Message));
            }

            return _resview;
        }

        private Font GetFontSize(Graphics g,string longString,Size Room,Font PreferedFont)
        {
            // you should perform some scale functions!!!
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;

            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio)
                ? ScaleRatio = HeightScaleRatio
                : ScaleRatio = WidthScaleRatio;

            float ScaleFontSize = PreferedFont.Size * (ScaleRatio / 2);

            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }

        private async Task<ResponseContext> InstbtImg(string contract_no, string imgName, string path, string contract_note_id, int ImgSeq)
        {
            var _resview = new ResponseContext();
            try
            {
                var tbtImgData = new TbtImg();
                tbtImgData.ContractNo = contract_no;
                tbtImgData.ImgName = imgName;
                tbtImgData.ImgPath = path;
                tbtImgData.ImgSeq = ImgSeq;
                tbtImgData.ContactNoteId = Convert.ToInt64(contract_note_id);      

                var resultInsert = await _tbt_img.AddAsync(tbtImgData).ConfigureAwait(false);
                if (resultInsert == null || resultInsert.Id == 0)
                {
                    _resview.statusCode = "201";
                    _resview.message = "Cannot Insert Tbt Img";
                    return _resview;
                }
                _resview.data = resultInsert;
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = String.Format("Insert Tbt Img Fail: {0}", ex.Message);
                return _resview;
            }

            return _resview;
        }

        private void Rotate(Bitmap bmp)
        {
            PropertyItem pi = bmp.PropertyItems.Select(x => x)
                                 .FirstOrDefault(x => x.Id == 0x0112);
            if (pi == null) return;

            byte o = pi.Value[0];

            if (o == 2) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            if (o == 3) bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            if (o == 4) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (o == 5) bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
            if (o == 6) bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (o == 7) bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
            if (o == 8) bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
        }

        private void ExifRotate(Image img)
        {
            if (!img.PropertyIdList.Contains(0x112))
                return;

            var prop = img.GetPropertyItem(0x112);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
            {
                img.RotateFlip(rot);
                img.RemovePropertyItem(0x112);
            }
        }

        private Image ResizeImage(Image imgToResize, Size size)
        {
            // Get the image current width
            int sourceWidth = imgToResize.Width;
            // Get the image current height
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calculate width and height with new desired size
            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);
            nPercent = Math.Min(nPercentW, nPercentH);
            // New Width and Height
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            b.SetResolution(72, 72);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, sourceWidth, sourceHeight);
            g.Dispose();
            return (Image)b;
        }

        public Image FixedSize(Image image)//, decimal newHeight
        {
            // Fix orientation if needed. It appears the code isn't compensating for exif data for the image.
            // So if a photo was taken with the camera sideways, the image was displaying sideways,
            // while the computer was reading that exif data and automatically rotating it in windows to display it as it's suppose to be.
            // This code reads the exif data and rotates accordingly.
            if (image.PropertyIdList.Contains(OrientationKey))
            {
                var orientation = (int)image.GetPropertyItem(OrientationKey).Value[0];
                switch (orientation)
                {
                    case NotSpecified: // Assume it is good.
                    case NormalOrientation:
                        // No rotation required.
                        break;
                    case MirrorHorizontal:
                        image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case UpsideDown:
                        image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case MirrorVertical:
                        image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case MirrorHorizontalAndRotateRight:
                        image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case RotateLeft:
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case MirorHorizontalAndRotateLeft:
                        image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case RotateRight:
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    default:
                        throw new NotImplementedException("An orientation of " + orientation + " isn't implemented.");
                }
            }

            // resize image code ...

            return image;//ResizeImage(image, new Size(800, (int)Math.Ceiling(newHeight))); ;
        }
    }




}
