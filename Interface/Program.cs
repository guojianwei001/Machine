﻿using System.Diagnostics;

namespace Emulator
{
    internal class MainClass
    {
        private static Machine m;
        private static readonly Stopwatch s = new();

        public static void Main(string[] args)
        {
            var (mem, delay) = ParseArgs(args);

            Console.WriteLine("Loading...");

            m = new Machine(new List<char>(mem), File.ReadAllLines(args[0]));
            //m.Cycle += OnCycle;
            m.Finish += OnFinish;

            Run(delay);
        }

        private static (string, int) ParseArgs(string[] args)
        {
            var mem = "_";
            var delay = 0;

            if (args.Length == 0)
            {
                throw new ArgumentException("No arguments provided.");
            }

            if (!File.Exists(args[0]))
            {
                throw new FileNotFoundException(args[0]);
            }

            switch (args.Length)
            {
                case 2:
                    mem = args[1];
                    break;
                case 3:
                    delay = Convert.ToInt32(args[2]);
                    break;
            }

            return (mem, delay);
        }

        private static void Run(int delay)
        {
            Console.WriteLine("Computing...");

            s.Start();
            m.Run("0", delay);
            s.Stop();

            Console.WriteLine("Done.");
        }

        private static void Print(Memory<char> memory, int cycles, TimeSpan elapsed)
        {
            Console.WriteLine();

            foreach (var c in memory.ToArray())
                Console.Write(c);

            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Cycles: {cycles} | Elapsed: {elapsed.TotalMilliseconds:F2}ms | Op/s: {cycles * 1000 / elapsed.TotalMilliseconds:F2}");
        }

        private static void OnFinish(object sender, MachineEventArgs e)
        {
            Print(e.Memory, e.Cycles, s.Elapsed);
        }

        private static void OnCycle(object sender, MachineEventArgs e)
        {
            Print(e.Memory, e.Cycles, s.Elapsed);
        }
    }
}