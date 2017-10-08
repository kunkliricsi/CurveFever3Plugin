using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AForge.Imaging;
using System.Resources;

namespace PluginForm
{
    class Panel
    {
        private int x, y, width, height;
        private Bitmap image = null;
        private object syncObject = new object();

        public Bitmap Image
        {
            get
            {
                lock (syncObject)
                    return image;
            }

            set
            {
                lock (syncObject)
                    image = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }

            set
            {
                if (value <= 0)
                    width = 0;
                else
                    width = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }

            set
            {
                if (value <= 0)
                    height = 0;
                else
                    height = value;
            }
        }

        public void SetRect(Rectangle r)
        {
            X = r.X;
            Y = r.Y;
            Width = r.Width;
            Height = r.Height;
        }

        public Rectangle GetRect()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }
    
    public class Plugin
    {
        private Point topLeftPoint, bottomRightPoint;
        private int width, height, sideX, sideY;
        private Panel top, left, right, bottom, topleft, topright, bottomleft, bottomright;
        private Graphics formGraphics;
        private bool set = false;

        public Thread first;

        public Plugin(Graphics f)
        {
            formGraphics = f;
        }

        private void CaptureApp()
        {
            var bmp = new Bitmap(sideX, sideY, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);

            while (true)
            {
                graphics.CopyFromScreen(topLeftPoint.X, topLeftPoint.Y, 0, 0, new Size(sideX, sideY), CopyPixelOperation.SourceCopy);

                formGraphics.DrawImage(bmp, top.GetRect(), new Rectangle(0, sideY - height, sideX, height), GraphicsUnit.Pixel);
                formGraphics.DrawImage(bmp, bottom.GetRect(), new Rectangle(0, 0, sideX, height), GraphicsUnit.Pixel);
                formGraphics.DrawImage(bmp, left.GetRect(), new Rectangle(sideX - width, 0, width, sideY), GraphicsUnit.Pixel);
                formGraphics.DrawImage(bmp, right.GetRect(), new Rectangle(0, 0, width, sideY), GraphicsUnit.Pixel);

                formGraphics.DrawImage(bmp, bottomright.GetRect(), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                formGraphics.DrawImage(bmp, bottomleft.GetRect(), new Rectangle(sideX - width, 0, width, height), GraphicsUnit.Pixel);
                formGraphics.DrawImage(bmp, topright.GetRect(), new Rectangle(0, sideY - height, width, height), GraphicsUnit.Pixel);
                formGraphics.DrawImage(bmp, topleft.GetRect(), new Rectangle(sideX - width, sideY - height, width, height), GraphicsUnit.Pixel);


            }
        }

        private void DrawOverlay()
        {
            while (true)
            {
                formGraphics.DrawImage(top.Image, top.GetRect());
                formGraphics.DrawImage(bottom.Image, bottom.GetRect());
                formGraphics.DrawImage(left.Image, left.GetRect());
                formGraphics.DrawImage(right.Image, right.GetRect());
            }
        }

        /*private Point FindButton(Bitmap toFind, Bitmap screen)
        {
            ExhaustiveTemplateMatching templateMatching = new ExhaustiveTemplateMatching(0.941f);
            TemplateMatch[] matches = templateMatching.ProcessImage(screen, toFind);

            Point p = new Point(matches.ElementAt(0).Rectangle.X, matches.ElementAt(0).Rectangle.Y);
            return p;
        }

        private Bitmap ConvertImage(Bitmap b)
        {
            Bitmap bCopy = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format24bppRgb);

            return bCopy;
        }*/

        public void Initialize(Point topLeft, Point bottomRight)
        {
            if (set)
                return;

            set = true;

            top = new Panel();
            bottom = new Panel();
            left = new Panel();
            right = new Panel();
            topleft = new Panel();
            topright = new Panel();
            bottomleft = new Panel();
            bottomright = new Panel();

            topLeftPoint = topLeft;
            bottomRightPoint = bottomRight;

            sideX = bottomRightPoint.X - topLeftPoint.X;
            sideY = bottomRightPoint.Y - topLeftPoint.Y;
            
            width =  sideX / 6;
            height =  sideY / 6;

            top.X = topLeftPoint.X;
            top.Y = topLeftPoint.Y - height;
            top.Width = sideX;
            top.Height = height;

            bottom.X = topLeftPoint.X;
            bottom.Y = bottomRightPoint.Y;
            bottom.Width = sideX;
            bottom.Height = height;

            left.X = topLeftPoint.X - width;
            left.Y = topLeftPoint.Y;
            left.Width = width;
            left.Height = sideY;

            right.X = bottomRightPoint.X;
            right.Y = topLeftPoint.Y;
            right.Width = width;
            right.Height = sideY;

            topleft.X = left.X;
            topleft.Y = top.Y;
            topleft.Width = left.Width;
            topleft.Height = top.Height;

            topright.X = right.X;
            topright.Y = top.Y;
            topright.Width = right.Width;
            topright.Height = top.Height;

            bottomleft.X = left.X;
            bottomleft.Y = bottom.Y;
            bottomleft.Width = left.Width;
            bottomleft.Height = bottom.Height;

            bottomright.X = right.X;
            bottomright.Y = bottom.Y;
            bottomright.Width = right.Width;
            bottomright.Height = bottom.Height;
        }

        public void Loop()
        {
            first = new Thread(CaptureApp);
            first.IsBackground = true;
            first.Start();
        }

        public void Abort()
        {
            first.Abort();
        }
    }
}
