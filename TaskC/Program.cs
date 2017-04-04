using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskA
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateTestData(@"..\..\test5000.txt");
            //return;

            //TaskExecutor te = new TaskExecutor(@"..\..\test100000.txt", 2);
            TaskExecutor te = new TaskExecutor(@"..\..\test5000.txt");
            //TaskExecutor te = new TaskExecutor(@"..\..\testSimple.txt");

            //te.Run(Run, false);
            te.Run(Run);
        }
        static void CreateTestData(string fileName)
        {
            Console.WriteLine("[INFO] GenData ...");
            //Run("3", "2 5 3");
            int limit = 50000;
            List<int> valuesL = new List<int>();

            int start = -limit / 2;
            for (int i = 0; i < limit; i++)
                valuesL.Add(start++);

            Shuffle(valuesL);

            string values = string.Join(" ", valuesL);

            Console.WriteLine("[INFO] GenData END");

            File.WriteAllText(fileName, limit.ToString() + "\r\n");
            File.AppendAllText(fileName, values);
        }


        private static Random rng = new Random();

        public static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Run(InputContext cin, OutputContext cout)
        {
            Stopwatch test = Stopwatch.StartNew();

            int total = cin.ReadLineInt().Value;
            for (int testIndex = 0; testIndex < total; testIndex++)
            {
                int testItems = cin.ReadLineInt().Value;
                List<int[]> testData = new List<int[]>(testItems);
                for (int i = 0; i < testItems; i++)
                    testData.Add(cin.ReadLineIntArr());

                testData.Sort((a, b) =>
                {
                    double ac = a[2] == 0 ? int.MaxValue : (a[0] - a[1]) / 10000000.0 * a[2];
                    double bc = b[2] == 0 ? int.MaxValue : (b[0] - b[1]) / 10000000.0 * b[2];

                    return ac.CompareTo(bc);
                });

                cout.WriteLine(CalcTestResult(testData).ToString().Replace(",", "."));
            }
        }

        private static double CalcTestResult(List<int[]> testData)
        {
            long successSumm = 0;
            double mult = 1 / 10000000.0;

            for (int i = 0; i < testData.Count; i++)
                successSumm += testData[i][1];

            long diffSumm = 0;
            double result = 0;

            for (int i = 0; i < testData.Count; i++)
            {
                diffSumm += testData[i][0] - testData[i][1];

                result += diffSumm * testData[i][2] * mult;
            }

            result += successSumm;

            return result;
        }

        #region executor

        public class TaskExecutor
        {
            private string _testFileName;
            private int _readInputLimit;
            public bool IsTestMode { get; private set; }

            public TaskExecutor(string testFileName, int readInputLimit = int.MaxValue)
            {
                IsTestMode = File.Exists(testFileName);
                _testFileName = testFileName;
                _readInputLimit = readInputLimit;
            }

            public void Run(Action<InputContext, OutputContext> action, bool wait = true)
            {
                var cout = new OutputContext(IsTestMode);
                var cin = new InputContext(IsTestMode, _testFileName, cout, _readInputLimit);

                Stopwatch sw = Stopwatch.StartNew();

                cout.Info("Start task");
                action(cin, cout);
                cout.Info("End task. Elapsed: " + sw.Elapsed);

                if (IsTestMode && wait)
                    Console.ReadKey();
            }
        }

        public class InputContext
        {
            private StreamReader _reader = null;
            private OutputContext _cout = null;
            private int _readCount = 0;
            private int _readLimit = int.MaxValue;
            public bool IsTestMode { get; private set; }

            public InputContext(bool isTestMode, string testFileName, OutputContext cout, int readLimit = int.MaxValue)
            {
                _readLimit = readLimit;
                _cout = cout;
                IsTestMode = isTestMode;
                if (isTestMode)
                    _reader = new StreamReader(testFileName);
            }


            public string ReadLine()
            {
                if (IsTestMode)
                {
                    if (_readCount == _readLimit)
                        _cout.Error("Read limit exeed!");

                    string result = null;

                    result = _reader.EndOfStream ? null : _reader.ReadLine();

                    if (_readCount == 0)
                        _cout.Info("First read from StdIn");

                    _readCount++;
                    if (_readCount == _readLimit)
                        _cout.Info("Last read from StdIn");

                    if (string.IsNullOrEmpty(result))
                        _cout.Warning("Empty result from StdIn");

                    return result;
                }
                else
                    return Console.ReadLine();
            }

            public int? ReadLineInt()
            {
                string data = ReadLine();
                if (string.IsNullOrEmpty(data)) return null;

                return int.Parse(data);
            }

            public int[] ReadLineIntArr(char sep = ' ')
            {
                string data = ReadLine();
                if (string.IsNullOrEmpty(data)) return null;

                string[] items = data.Split(sep);
                int[] result = new int[items.Length];

                for (int i = 0; i < items.Length; i++)
                    result[i] = int.Parse(items[i]);

                return result;
            }
        }
        public class OutputContext
        {
            public bool IsTestMode { get; private set; }

            public OutputContext(bool isTestMode)
            {
                IsTestMode = isTestMode;
            }

            public void Debug(string data)
            {
                WriteLine("[DEBUG] " + data, ConsoleColor.Green);
            }
            public void Error(string data)
            {
                WriteLine("[ERROR] " + data, ConsoleColor.Red);
            }
            public void Info(string data)
            {
                WriteLine("[INFO] " + data, ConsoleColor.Cyan);
            }
            public void Warning(string data)
            {
                WriteLine("[WARN] " + data, ConsoleColor.Yellow);
            }

            public void WriteLine(string data)
            {
                Console.WriteLine(data);
            }

            public void Write(string data)
            {
                Console.Write(data);
            }

            private void WriteLine(string data, ConsoleColor color)
            {
                if (!IsTestMode) return;

                var prev = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(DateTime.Now.TimeOfDay + " " + data);
                Console.ForegroundColor = prev;
            }
        }

        #endregion
    }
}
