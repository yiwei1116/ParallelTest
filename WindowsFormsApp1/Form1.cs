using FastBitmap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(@"C:\Users\sm880\OneDrive\圖片\temp.bmp");
            List<Task> tasks = new List<Task>();
            int threadCount = 2;
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
            pictureBox1.Image = lockBitmap.source;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            label1.Text = seconds.ToString() + "sec";

        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(@"C:\Users\sm880\OneDrive\圖片\temp.bmp");
            int width = bmp.Width;
            int height = bmp.Height;
            Benchmark.Start();
            var lockBitmap = new PointBitmap(bmp);
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
            int perWidth = (width / split)+1;
            int count = 0;
            var result = new List<int>();
            while(true)
            {
                if (count + perWidth >= width)
                {
                    result.Add(width-1);
                    break;
                }
                count += perWidth;
                result.Add(count);
            }
            return result;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(@"C:\Users\sm880\OneDrive\圖片\temp.bmp");
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
