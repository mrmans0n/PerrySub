using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace scriptASS
{
    class BitmapFunctions
    {

        public static Bitmap Crop(Image imgPhoto, int targetW, int targetH, int targetX, int targetY)
        {
            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage((Image)imgPhoto, new Rectangle(0, 0, targetW, targetH), targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);
            MemoryStream mm = new MemoryStream();
            bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
            imgPhoto.Dispose();
            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Bitmap Resize(Bitmap b, int nWidth, int nHeight, bool bBilinear)
        {
            Bitmap bTemp = (Bitmap)b.Clone();
            b = new Bitmap(nWidth, nHeight, bTemp.PixelFormat);

            double nXFactor = (double)bTemp.Width / (double)nWidth;
            double nYFactor = (double)bTemp.Height / (double)nHeight;

            if (bBilinear)
            {
                double fraction_x, fraction_y, one_minus_x, one_minus_y;
                int ceil_x, ceil_y, floor_x, floor_y;
                Color c1 = new Color();
                Color c2 = new Color();
                Color c3 = new Color();
                Color c4 = new Color();
                byte red, green, blue;

                byte b1, b2;

                for (int x = 0; x < b.Width; ++x)
                    for (int y = 0; y < b.Height; ++y)
                    {
                        // Setup

                        floor_x = (int)Math.Floor(x * nXFactor);
                        floor_y = (int)Math.Floor(y * nYFactor);
                        ceil_x = floor_x + 1;
                        if (ceil_x >= bTemp.Width) ceil_x = floor_x;
                        ceil_y = floor_y + 1;
                        if (ceil_y >= bTemp.Height) ceil_y = floor_y;
                        fraction_x = x * nXFactor - floor_x;
                        fraction_y = y * nYFactor - floor_y;
                        one_minus_x = 1.0 - fraction_x;
                        one_minus_y = 1.0 - fraction_y;

                        c1 = bTemp.GetPixel(floor_x, floor_y);
                        c2 = bTemp.GetPixel(ceil_x, floor_y);
                        c3 = bTemp.GetPixel(floor_x, ceil_y);
                        c4 = bTemp.GetPixel(ceil_x, ceil_y);

                        // Blue
                        b1 = (byte)(one_minus_x * c1.B + fraction_x * c2.B);
                        b2 = (byte)(one_minus_x * c3.B + fraction_x * c4.B);
                        blue = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                        // Green
                        b1 = (byte)(one_minus_x * c1.G + fraction_x * c2.G);
                        b2 = (byte)(one_minus_x * c3.G + fraction_x * c4.G);
                        green = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                        // Red
                        b1 = (byte)(one_minus_x * c1.R + fraction_x * c2.R);
                        b2 = (byte)(one_minus_x * c3.R + fraction_x * c4.R);
                        red = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                        b.SetPixel(x, y, System.Drawing.Color.FromArgb(255, red, green, blue));
                    }
            }
            else
            {
                for (int x = 0; x < b.Width; ++x)
                    for (int y = 0; y < b.Height; ++y)
                        b.SetPixel(x, y, bTemp.GetPixel((int)(Math.Floor(x * nXFactor)), (int)(Math.Floor(y * nYFactor))));
            }

            return b;
        }

        public static Bitmap FastResize(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }

        public static Bitmap FastCrop(Bitmap b, int nWidth, int nHeight, int x)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(b, 0, 0, new Rectangle(x, 0, nWidth, nHeight), GraphicsUnit.Pixel);
            return result;
        }
        

        public static Bitmap PutText(Bitmap b, string text,int x, int y)
        {

            Graphics gr = System.Drawing.Graphics.FromImage(new Bitmap(1, 1));

            StringFormat stringformat = new StringFormat(StringFormat.GenericTypographic);
            Font fuente = new Font("Arial", 7, FontStyle.Regular);

            int ancho = Convert.ToInt32(gr.MeasureString(text, fuente, new PointF(0, 0), stringformat).Width);
            int alto = Convert.ToInt32(gr.MeasureString(text, fuente, new PointF(0, 0), stringformat).Height);


            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.FillRectangle(new SolidBrush(Color.Crimson), new Rectangle(x-1, y-1, ancho+2, alto+2));
                g.FillRectangle(new SolidBrush(Color.White),new Rectangle(x,y,ancho,alto));
                g.DrawString(text, fuente, new SolidBrush(Color.Black), new PointF(x, y));
            }
            
            return b;
        }

        public static Bitmap PutLine(Bitmap b, int x1, int y1, int x2, int y2)
        {
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.DrawLine(new Pen(new SolidBrush(Color.White)), x1, y1, x2, y2);
                g.FillRectangle(new SolidBrush(Color.Crimson),new Rectangle(x1-3,y1-3,6,6));
            }

            return b;

        }

        public static Bitmap PutRectangle(Bitmap b, int x1, int y1, int x2, int y2)
        {
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                int rx1, rx2, ry1, ry2;
                rx1 = x1; rx2 = x2;
                if (x2<x1) { rx1 = x2; rx2 = x1;}
                ry1 = y1; ry2 = y2;
                if (y2 < y1) { ry1 = y2; ry2 = y1; }
                
                g.DrawRectangle(new Pen(new HatchBrush(HatchStyle.Trellis,Color.Orange)), rx1, ry1, rx2-rx1, ry2-ry1);
                g.FillRectangle(new SolidBrush(Color.Crimson), new Rectangle(x1 - 2, y1 - 2, 4, 4));
                g.FillRectangle(new SolidBrush(Color.Crimson), new Rectangle(x2 - 2, y1 - 2, 4, 4));
                g.FillRectangle(new SolidBrush(Color.Crimson), new Rectangle(x1 - 2, y2 - 2, 4, 4));
                g.FillRectangle(new SolidBrush(Color.Crimson), new Rectangle(x2 - 2, y2 - 2, 4, 4));
            }

            return b;
        }

        public static Bitmap GenerateBorder(Bitmap b,Color c, int thickness)
        {
            using (Graphics g = Graphics.FromImage((Bitmap)b))
            {
                g.DrawRectangle(new Pen(new SolidBrush(c),thickness),new Rectangle(0,0,b.Width-thickness,b.Height-thickness));
            }
            return b;
        }

        public static Bitmap GenerateKeyFrameDisplay(int width, int height, int[] keyFrames,int max)
        {
            Bitmap b = new Bitmap(width, height);

            Color key = Color.DarkGray;

            for (int i = 0; i < keyFrames.Length; i++)
            {
                int actual = keyFrames[i];
                float p = (float)((float)(actual * width) / max);

                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.DrawLine(new Pen(new SolidBrush(key)), p, 0, p, (float)height);
                }
                
            }
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.DrawRectangle(new Pen(new SolidBrush(Color.Gray)), new Rectangle(0, 0, width-1, height-1));
            }


            return b;
        }

        public static Bitmap GenerateKeyFrameDisplayOne(int width, int height, int keyframe, int max)
        {
            Bitmap b = new Bitmap(width, height);
            //if (i == null) b = 
            //else b = (Bitmap)i; ;//new Bitmap(width, height); 
                
            Color key = Color.DarkGray;

            int actual = keyframe;
            float p = (float)((float)(actual * width) / max);

            using (Graphics g = Graphics.FromImage((Image)b))
            {
                //g.DrawLine(new Pen(new SolidBrush(key)), p, 0, p, (float)height);
                g.FillRectangle(new SolidBrush(Color.Crimson), 0, 0, p, (float)height);
            }
            /*
            if (keyframe == 0){
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.DrawRectangle(new Pen(new SolidBrush(Color.Gray)), new Rectangle(0, 0, width - 1, height - 1));
                }
            }
             */

            return b;

        }

        public static Bitmap GenerateKeyFrameDisplaySlider(Bitmap b, int frame, int max)
        {
            Color slider = Color.OrangeRed;
            Color slider_border = Color.Red;

            float p = (float)((float)(frame * b.Width) / max);

            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.DrawLine(new Pen(new SolidBrush(slider)), p, 1, p, (float)b.Height-1);
                g.DrawRectangle(new Pen(new SolidBrush(slider_border)),new Rectangle(((int)p-1),0,2,b.Height-1));
            }
            return b;
        }


        public static Bitmap GenerateAudioGridDisplay(Bitmap b, int pxpersec, int value10, bool putText)
        {

            int value = value10 / 10;
            int resto = value10 % 10;

            Color back = Color.Black;
            Color font = Color.White;

            Color linea_seg = Color.White;
            Color linea_medio = Color.Red;
            Color linea_dec = Color.LightSalmon;

            int alto_medio = 10;
            int alto_dec = 7;

            int alto = b.Height;
            int ancho = b.Width;

            int decimos = pxpersec / 10;
            int medios = decimos * 5;
            int segundo = medios * 2;

            using (Graphics g = Graphics.FromImage((Image)b))
            {
                // fondo
                // g.FillRectangle(new SolidBrush(back),new Rectangle(0,0,ancho,alto));

                // segundos
                for (int i = 0; i < segundo; i++)
                {
                    int x = (segundo * i)-(resto*decimos);                    
                    //g.DrawLine(new Pen(new HatchBrush(HatchStyle.Horizontal, linea_seg)), x, 0, x, alto);
                    if (!putText)
                        g.DrawLine(new Pen(new HatchBrush(HatchStyle.Horizontal, linea_seg)), x, 0, x, alto);
                    else
                    {
                        int text_x = x + 3;
                        int text_y = 1;

                        StringFormat stringformat = new StringFormat(StringFormat.GenericTypographic);
                        Font f = new Font("Tahoma", 8);
                        string texto = Tiempo.SecondToTimeString((double)i + value);
                        int anchoi = Convert.ToInt32(g.MeasureString(texto, f, new PointF(0, 0), stringformat).Width)+4;
                        int altoi = Convert.ToInt32(g.MeasureString(texto, f, new PointF(0, 0), stringformat).Height);

                        g.FillRectangle(new SolidBrush(Color.Snow), new Rectangle(text_x - 1, text_y - 1, anchoi + 2, altoi + 2));
                        g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(text_x, text_y, anchoi, altoi));

                        g.DrawString(texto, f, new SolidBrush(font), new PointF(text_x-1, text_y));
                    }
                }
                if (!putText)
                {
                    // decimos
                    for (int i = 0; i < decimos * 50; i++)
                    {
                        int x = decimos * i;
                        g.DrawLine(new Pen(new SolidBrush(linea_dec)), x, alto - alto_dec, x, alto);
                    }

                    // medios

                    for (int i = 0; i < medios * 2; i++)
                    {
                        int x = (medios * i) - (resto * decimos);
                        g.DrawLine(new Pen(new SolidBrush(linea_medio)), x, alto - alto_medio, x, alto);
                    }
                }
            }

            return b;
        }

        public static Bitmap GenerateAudioGridDisplayMarkers(Bitmap b, double pinicio, double pfinal, double marca, int pxsec, bool ActualTime, lineaASS lass, bool DrawText)
        {

            if (pinicio != -1 || pfinal != -1)
            {
                int x_inicial=0, x_final=0;

                Color inicio = (ActualTime) ? Color.Red : Color.DarkGreen;
                Color final = (ActualTime)? Color.Orange : Color.DarkGreen;
                Color selected = (ActualTime)? Color.AntiqueWhite : Color.Black;
                int alphaselected = (ActualTime)? 50 : 150;
                Color fondotiempo = Color.White;

                int ancho = b.Width;
                int alto = b.Height;

                string sinicio = Tiempo.SecondToTimeString(pinicio);
                string sfinal = Tiempo.SecondToTimeString(pfinal);
                
                StringFormat stringformat = new StringFormat(StringFormat.GenericTypographic);
                Font fuente = new Font("Tahoma", 8, FontStyle.Bold);



                double sec = (double)ancho / (double)pxsec;

                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    int anchoi = Convert.ToInt32(g.MeasureString(sinicio, fuente, new PointF(0, 0), stringformat).Width);
                    int altoi = Convert.ToInt32(g.MeasureString(sinicio, fuente, new PointF(0, 0), stringformat).Height);
                    int anchof = Convert.ToInt32(g.MeasureString(sfinal, fuente, new PointF(0, 0), stringformat).Width);
                    int altof = Convert.ToInt32(g.MeasureString(sfinal, fuente, new PointF(0, 0), stringformat).Height);
                    
                    if (pinicio != -1)
                    {
                        double rel = pinicio - marca;                           
                        double p1 = (100 * rel) / sec;
                        double p2 = (ancho * p1) / 100;

                        x_inicial = (int)Math.Round(p2);
                        g.DrawLine(new Pen(new SolidBrush(inicio), 1), x_inicial, 0, x_inicial, alto);

                        if (ActualTime)
                        {

                            int xs = x_inicial - anchoi - 6;
                            int ys = alto - (altoi * 2);
                            anchoi += 6;
                            g.FillRectangle(new SolidBrush(inicio), new Rectangle(xs - 1, ys - 1, anchoi + 2, altoi + 2));
                            g.FillRectangle(new SolidBrush(fondotiempo), new Rectangle(xs, ys, anchoi, altoi));
                            g.DrawString(sinicio, fuente, new SolidBrush(Color.Black), new Point(xs, ys));
                        }
                    }
                    if (pfinal != -1)
                    {
                        double rel = pfinal - marca;
                            
                        double p1 = (100 * rel) / sec;
                        double p2 = (ancho * p1) / 100;

                        x_final = (int)Math.Round(p2);
                        g.DrawLine(new Pen(new SolidBrush(final), 1), x_final, 0, x_final, alto);

                        if (ActualTime)
                        {
                            int xs = x_final + 1;
                            int ys = alto - (altof * 2);

                            anchof += 6;
                            g.FillRectangle(new SolidBrush(final), new Rectangle(xs - 1, ys - 1, anchof + 2, altof + 2));
                            g.FillRectangle(new SolidBrush(fondotiempo), new Rectangle(xs, ys, anchof, altof));
                            g.DrawString(sfinal, fuente, new SolidBrush(Color.Black), new Point(xs, ys));
                        }

                    }
                    if (pinicio != -1 && pfinal != -1)
                    {
                        if (x_inicial < x_final)
                        {

                            //rectangulo

                            Rectangle r = new Rectangle(x_inicial + 1, 0, (x_final - x_inicial) - 1, alto);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(alphaselected, selected)), r);

                            // texto
                            if (DrawText)
                            {
                                r.Height = alto - 10;
                                StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = (ActualTime) ? StringAlignment.Center : StringAlignment.Far;


                                Font f = (ActualTime) ? new Font("Tahoma", 9, FontStyle.Regular) : new Font("Tahoma", 9, FontStyle.Italic);

                                string texto = lineaASS.cleanText(lass.texto);
                                g.DrawString(texto, f, new SolidBrush((lass.IsComment()) ? Color.Yellow : Color.White), r, sf);
                            }
                        }
                    }
                }
            }
            return b;
        }
        public static Bitmap GenerateAudioGridDisplayActual(Bitmap b, double pinicio,double marca, double pxsec)
        {

            int ancho = b.Width;
            int alto = b.Height;

            double sec = (double)ancho / (double)pxsec;
            double rel = pinicio - marca;
            double p1 = (100 * rel) / sec;
            double p2 = (ancho * p1) / 100;

            int x_inicial = (int)Math.Round(p2);

            using (Graphics g = Graphics.FromImage((Image)b)){
                g.DrawLine(new Pen(new SolidBrush(Color.Red)), x_inicial, 0, x_inicial, alto);
            }

            return (Bitmap)b;
        }

        public static double CompararImagenes(Image image1, Image image2)
        {
            Bitmap img1 = new Bitmap(image1);
            Bitmap img2 = new Bitmap(image2);

            if (img1.Size != img2.Size) throw new Exception("Los tamaños de la imágen han de ser el mismo");

            int max_X = img1.Width;
            int max_Y = img1.Height;

            BitmapData data1 = img1.LockBits(new Rectangle(0, 0, max_X, max_Y), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData data2 = img2.LockBits(new Rectangle(0, 0, max_X, max_Y), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int ptr = 0;
            
            Int64 pixels = img1.Height * img1.Width;
            Int64 diff = 0;

            unsafe
            {
                byte* imgPtr1 = (byte*)(data1.Scan0);
                byte* imgPtr2 = (byte*)(data2.Scan0);

                for (int i = 0; i < data1.Height; i++)
                {
                    for (int j = 0; j < data1.Width; j++)
                    {
                        int r1 = imgPtr1[ptr];
                        int g1 = imgPtr1[ptr + 1];
                        int b1 = imgPtr1[ptr + 2];
                        int r2 = imgPtr2[ptr];
                        int g2 = imgPtr2[ptr + 1];
                        int b2 = imgPtr2[ptr + 2];

                        // comparamos
                        if (r1 != r2 || g1 != g2 || b1 != b2)
                            diff++;

                        ptr += 3;
                    }
                    
                    ptr += data1.Stride - data1.Width * 3;
                }

            }
            img1.UnlockBits(data1);
            img2.UnlockBits(data2);
            img1.Dispose();
            img2.Dispose();

            return (double)((double)diff/(double)pixels);
        }

    }
}
