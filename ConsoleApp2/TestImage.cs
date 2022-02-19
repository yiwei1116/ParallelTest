using BenchmarkDotNet.Attributes;
using FastBitmap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    [MemoryDiagnoser]
    public class TestImage
    {
        private readonly Bitmap bmp;

        string path = Path.Combine
          (System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Images","temp.bmp");
        public TestImage()
        {
            bmp = new Bitmap(path);
        }
        [Benchmark]
        public async Task Parallel1ThreadAsync() => await ParallelThreadAsync(1);
        [Benchmark]
        public async Task Parallel2ThreadAsync() => await ParallelThreadAsync(2);
        [Benchmark]
        public async Task Parallel4ThreadAsync() => await ParallelThreadAsync(4);
        [Benchmark]
        public async Task Parallel8ThreadAsync() => await ParallelThreadAsync(8);
        [Benchmark]
        public async Task Parallel16ThreadAsync() => await ParallelThreadAsync(16);
        [Benchmark]
        public async Task Parallel32ThreadAsync() => await ParallelThreadAsync(32);

        private async Task ParallelThreadAsync(int threadSplit)
        {
            List<Task> tasks = new List<Task>();
            var paras = Preprocess(bmp.Height, threadSplit);
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
            while (width > count)
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
        [Benchmark]
        public void ParallelFor()
        {
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
        }
        [Benchmark(Baseline = true)]
        public void NormalExec()
        {
            int width = bmp.Width;
            int height = bmp.Height;
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
        }
       
    }
}
