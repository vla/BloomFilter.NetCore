using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PerformanceTest
{
    internal class Helper
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        private const string Base62 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateString(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException($"{nameof(length)} is {length}.");

            var random = new Random(Guid.NewGuid().GetHashCode());
            return new string(
                Enumerable.Repeat(Base62, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        public static byte[] GenerateBytes(int size = 4)
        {
            if (size < 0) throw new ArgumentOutOfRangeException($"{nameof(size)} is {size}.");

            var buff = new byte[size];
            Rng.GetBytes(buff);
            return buff;
        }

        public static IList<byte[]> GenerateData(int n)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var set = new HashSet<byte[]>(n);
            var list = new List<byte[]>(n);
            for (int i = 0; i < n; i++)
            {
                var data = GenerateBytes(random.Next(32, 512));
                if (!set.Contains(data))
                {
                    list.Add(data);
                }
                set.Add(data);
            }
            return list;
        }


        public static void Time(string name, Action<int> action, int iteration = 1)
        {
            TimeExecute(name, () =>
            {
                for (int i = 0; i < iteration; i++)
                {
                    action(i);
                }
            }, iteration);
        }

        public static void TimeWithThread(string name, Action<int, int> action, int task = 1, int iteration = 1)
        {
            TimeExecute(name, () =>
            {
                var tasks = new Task[task];
                var taskCount = iteration / tasks.Length;

                for (int i = 0; i < tasks.Length; i++)
                {
                    int t = i;
                    tasks[i] = Task.Run(() =>
                    {
                        for (int x = 0; x < taskCount; x++)
                        {
                            action(t, x);
                        }
                    });
                };

                Task.WaitAll(tasks);
            }, iteration);
        }

        public static void TimeWithParallel(string name, Action<int> action, int iteration = 1)
        {
            TimeExecute(name, () =>
            {
                Parallel.For(0, iteration, l =>
                {
                    int num = l;
                    action(num);
                });
            }, iteration);
        }

        private static void TimeExecute(string name, Action action, int iteration = 1)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            Stopwatch watch = new Stopwatch();

            ulong cycleCount = GetCycleCount();
            watch.Start();

            action();

            watch.Stop();
            ulong cpuCycles = GetCycleCount() - cycleCount;

            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tIterations:\t" + iteration);
            Console.WriteLine("\tTime Elapsed:\t" + watch.Elapsed.TotalMilliseconds + "ms");
            Console.WriteLine("\tPer Second:\t" + (iteration / watch.Elapsed.TotalSeconds).ToString("N0"));
            Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));
            Console.WriteLine("\tMemory:\t\t" + FormatBytesToString(Process.GetCurrentProcess().WorkingSet64));

            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t" + count);
            }

            Console.WriteLine();
        }

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentThread();

        public static void PrintMem()
        {
            Console.WriteLine("Memory:" + FormatBytesToString(Process.GetCurrentProcess().WorkingSet64));
        }

        public static string FormatBytesToString(double bytes)
        {
            const ulong
              KB = 1UL << 10,
              MB = 1UL << 20,
              GB = 1UL << 30,
              TB = 1UL << 40,
              PB = 1UL << 50,
              EB = 1UL << 60;

            if (bytes > EB)
                return string.Format("{0}EB", (bytes / EB).ToString("F2"));
            if (bytes > PB)
                return string.Format("{0}PB", (bytes / PB).ToString("F2"));
            if (bytes > TB)
                return string.Format("{0}TB", (bytes / TB).ToString("F2"));
            if (bytes > GB)
                return string.Format("{0}GB", (bytes / GB).ToString("F2"));
            if (bytes > MB)
                return string.Format("{0}MB", (bytes / MB).ToString("F2"));
            if (bytes > KB)
                return string.Format("{0}KB", (bytes / KB).ToString("F2"));
            return bytes + "Byte";
        }
    }
}