using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using System.Diagnostics;

namespace HW_L05
{
    class Program
    {
        public static readonly int Step = 2;
        private static TimeSpan swatch;
        private static object lockObject = new object();
        private static int count_threads = 0;

        static void Main(string[] args)
        {
            #region Ввод данных
            Console.Write("Введите число, не больше 100 000(запаритесь ждать): ");
            ulong s = 0;
            UInt64.TryParse(Console.ReadLine(), out s);
            #endregion

            swatch = new TimeSpan();
            
            //count_threads = (int)s / Step;

            var count_processors = Environment.ProcessorCount;
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(count_processors, count_processors);

            for (ulong i = (ulong)s; i > 1; i = i - (ulong)Step)
            {
                ThreadPool.QueueUserWorkItem(ShowFactorial, i);
                count_threads++;
            }
            


            lock (lockObject)
                while (count_threads > 0)
                {
                    
                    Monitor.Wait(lockObject);
                    Console.WriteLine($"Осталось потоков: {count_threads}\t Затраченное время: {swatch}");
                }
            
            Console.WriteLine($"Общее время выполнения: {swatch}");

            Console.ReadLine();
        }
        
        /// <summary>
        /// вывод на экран factorial(число), параметр = число
        /// </summary>
        /// <param name="f"></param>
        private static void ShowFactorial(object f)
        {
            lock (lockObject)
            {
                var startTime = Stopwatch.StartNew();

                //считаем факториал
                var result_F = factorial((ulong)f);
                if(result_F.ToString().Length > 100)
                    Console.WriteLine($"Факториал {(ulong)f} = число более 100 символов..");
                else
                    Console.WriteLine($"Факториал {(ulong)f} = {result_F}");

                startTime.Stop();
                swatch += startTime.Elapsed;
                count_threads--;
                Monitor.Pulse(lockObject);
            }
            
        }

        /// <summary>
        /// считает факториал, параметр = число
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static BigInteger factorial(ulong f)
        {
            BigInteger F = 1;

            for (ulong c = f; c > 1; c--)
                F = F * c;

            return F;
        }
    }
}
