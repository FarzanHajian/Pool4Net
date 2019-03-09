using Pool4Net;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pool4NetTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LoadTest();
            LoadTestDefault();
            Instantiation();

            Console.ReadLine();
        }

        private static void LoadTest()
        {
            const int TASK_COUNT = 200;
            const int GROUP_COUNT = 5;
            const int OBJECT_COUNT_PER_TASK = 1500;

            string[] groups = new string[GROUP_COUNT] { "0", "1", "2", "3", "4" };
            Pool4Net<TestData> pool = new Pool4Net<TestData>(Generator, null, groups);
            Task[] tasks = new Task[TASK_COUNT];

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < TASK_COUNT; i++)
            {
                tasks[i] = Task.Run(
                    () =>
                    {
                        string g = (i % GROUP_COUNT).ToString();
                        for (int j = 0; j < OBJECT_COUNT_PER_TASK; j++)
                        {
                            var data = pool.Get(g);
                            pool.Release(g, data);
                        }
                    }
                );
            }
            Task.WaitAll(tasks);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            TestData Generator()
            {
                return new TestData();
            }
        }

        private static void LoadTestDefault()
        {
            const int TASK_COUNT = 2000;
            const int OBJECT_COUNT_PER_TASK = 15000;

            Pool4Net<TestData> pool = new Pool4Net<TestData>(Generator, null);
            Task[] tasks = new Task[TASK_COUNT];

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < TASK_COUNT; i++)
            {
                tasks[i] = Task.Run(
                    () =>
                    {
                        for (int j = 0; j < OBJECT_COUNT_PER_TASK; j++)
                        {
                            var data = pool.Get();
                            pool.Release(data);
                        }
                    }
                );
            }
            Task.WaitAll(tasks);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            TestData Generator()
            {
                return new TestData();
            }
        }

        private static void Instantiation()
        {
            Stopwatch sw = Stopwatch.StartNew();

            TestData[] data = new TestData[300000];

            for (int i = 0; i < 300000; i++)
            {
                data[i] = new TestData();
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        private class TestData
        {
            public int Value1 { get; set; }
            public string Value2 { get; set; }
            public decimal Value3 { get; set; }
        }
    }
}