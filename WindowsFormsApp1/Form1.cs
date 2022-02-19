using BenchmarkDotNet.Running;
using FastBitmap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string path = Path.Combine
            (System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName ,"temp.bmp");
        private PointBitmap bm1;
        private PointBitmap bm2;
        public Form1()
        {
            InitializeComponent();
         
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            List<Task> tasks = new List<Task>();
            int threadCount = 8;
            Benchmark.Start();
            var paras = Preprocess(bmp.Height, threadCount);
            var lockBitmap = new PointBitmap(bmp);
            lockBitmap.LockBits();
            for (int i = 0; i < paras.Count; i++)
            {
                int start = (i == 0) ? 0 : paras[i - 1];
                int end = paras[i];
                tasks.Add(Task.Run(() => ProcessBitmap(lockBitmap, start, end)));
            }
            await Task.WhenAll(tasks);
            lockBitmap.UnlockBits();
            Benchmark.End();
            double seconds = Benchmark.GetSeconds();
            bm2 = lockBitmap;
            if (bm1 != null && bm2 != null)
            {
                var isSame = Benchmark.CompareMemCmp(bm1, bm2);
            }
            pictureBox1.Image = lockBitmap.source;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            label1.Text = seconds.ToString() + "sec";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Benchmark.Start();
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            var lockBitmap = new PointBitmap(bmp);
            int width = bmp.Width;
            int height = bmp.Height;
            lockBitmap.LockBits();
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    var old = lockBitmap.GetPixel(x, y);
                    Color color = Color.FromArgb(old.A, old.R, old.G, 255);
                    lockBitmap.SetPixel(x, y, color);
                }
            });
            lockBitmap.UnlockBits();
            Benchmark.End();
            double seconds = Benchmark.GetSeconds();
            pictureBox1.Image = lockBitmap.source;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            label1.Text = seconds.ToString() + "sec";
        }

        private void ProcessBitmap(PointBitmap source, int strY, int endY)
        {
            for (int y = strY; y < endY; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    var old = source.GetPixel(x, y);
                    Color color = Color.FromArgb(old.A, old.R, old.G, 255);
                    source.SetPixel(x, y, color);
                }
              
            }
        }

        private List<int> Preprocess(int width, int split)
        {
            int perWidth = (width / split);
            if (width % split != 0)
            {
                perWidth++;
            }
            int count = 0;
            var result = new List<int>();
            while(width > count)
            {
                count += perWidth;
                if (count > width)
                {
                    count = width;
                }
                result.Add(count);
            }
            return result;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            int width = bmp.Width;
            int height = bmp.Height;
            Benchmark.Start();
            var lockBitmap = new PointBitmap(bmp);
            lockBitmap.LockBits();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var old = lockBitmap.GetPixel(x, y);
                    Color color = Color.FromArgb(old.A, old.R, old.G, 255);
                    lockBitmap.SetPixel(x, y, color);
                }
            }
            lockBitmap.UnlockBits();
            Benchmark.End();
            double seconds = Benchmark.GetSeconds();
            bm1 = lockBitmap;
            //RecordPixel(lockBitmap);
            pictureBox1.Image = lockBitmap.source;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            label1.Text = seconds.ToString() + "sec";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Default();
        }
        private void Default()
        {
            Bitmap tmpBmp = (Bitmap)Image.FromFile(@"C:\Users\sm880\OneDrive\圖片\temp.bmp");
            pictureBox1.Image = tmpBmp;
            label1.Text = "sec";
        }

       
    }
}
