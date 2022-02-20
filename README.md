# Parallel Thread Test Result

## Test Data
**Width:4098 pixel  
Height:4177 pixel  
Bits Per Pixel:24** 

![image](https://user-images.githubusercontent.com/20869743/154795486-eff783c6-91f0-4ace-8f37-9bfc6920ac53.png) =====>
![image](https://user-images.githubusercontent.com/20869743/154795492-aaba6fca-f6db-4eff-9628-7168d731842f.png)

## Test Method

主要測試三種方法
1. 手動前處理後，切割成N個Thread平行處理
2. 系統自動分派Thread平行處理
3. 一般執行Main thread處理

檢查運算後每一個pixel的RGB是否都一樣，確保每一個Method的operation量是一樣的。

## Function

### ParallelNThreadAsync
```c#
        private async Task ParallelThreadAsync(int threadSplit)
        {
            List<Task> tasks = new List<Task>();
            // 前處理 切割成n等份
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
```
### ParallelFor

```c#
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
```


### NormalExec
```C#
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
```
## Performance

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1526 (20H2/October2020Update)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4470.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4470.0), X86 LegacyJIT


```
|                Method |     Mean |   Error |   StdDev | Ratio | Allocated |
|---------------------- |---------:|--------:|---------:|------:|----------:|
|  Parallel1ThreadAsync | 903.8 ms | 2.52 ms |  1.97 ms |  0.99 |         - |
|  Parallel2ThreadAsync | 460.0 ms | 2.38 ms |  2.22 ms |  0.50 |         - |
|  Parallel4ThreadAsync | 232.5 ms | 1.40 ms |  1.17 ms |  0.25 |         - |
|  Parallel8ThreadAsync | 181.1 ms | 4.68 ms | 13.81 ms |  0.20 |         - |
| Parallel16ThreadAsync | 120.0 ms | 0.11 ms |  0.10 ms |  0.13 |   3,277 B |
| Parallel32ThreadAsync | 122.1 ms | 2.35 ms |  2.62 ms |  0.13 |   6,144 B |
|           ParallelFor | 121.8 ms | 0.33 ms |  0.31 ms |  0.13 |  27,853 B |
|            NormalExec | 915.4 ms | 6.23 ms |  5.82 ms |  1.00 |         - |



### Parallel8ThreadAsync
![image](https://user-images.githubusercontent.com/20869743/154796433-fcf34033-143e-4a94-9611-22deedd94a09.png)

### ParallelFor
![image](https://user-images.githubusercontent.com/20869743/154796814-899d48e6-268d-4bf6-b973-10caa223a574.png)

### NormalExec
![image](https://user-images.githubusercontent.com/20869743/154796784-24c93583-cd0e-4fd4-ac20-5da1d2305e98.png)

