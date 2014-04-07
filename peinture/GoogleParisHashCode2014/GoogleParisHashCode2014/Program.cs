using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleParisHashCode2014
{
    internal class Program
    {
        public struct Move
        {
            public int R;
            public int C;
            public int S;

            public Move(int r, int c, int s)
                : this()
            {
                R = r;
                C = c;
                S = s;
            }

            public Move(int r, int c)
                : this()
            {
                R = r;
                C = c;
                S = 0;
            }

            public bool IsDefault()
            {
                return R == 0 && C == 0 && S == 0;
            }

            public bool NotFound()
            {
                return R == -1 && C == -1;
            }

            public bool IsValid()
            {
                return R >= 0 && R < _height && C >= 0 && C < _width && S >= 0 && R - S >= 0 && C - S >= 0 && R + S < _height && C + S < _width;
            }

            public new string ToString()
            {
                return string.Format("Move: {0} {1} {2}", R, C, S);
            }

            public void Dump()
            {
                Console.WriteLine("Move: {0} {1} {2}", R, C, S);
            }
        }

        private static void ExtractData(string file)
        {
            var lines = File.ReadAllLines(file);
            var dim = lines[0].Split(' ');

            _height = int.Parse(dim[0]);
            _width = int.Parse(dim[1]);

            for (int i = 1; i < _height + 1; i++)
            {
                Expected.Add(lines[i].ToCharArray());
            }
        }

        private static void GetPlayground()
        {
            Playground.Clear();

            foreach (char[] list in Expected)
            {
                var toAdd = (char[])list.Clone();

                for (int i = 0; i < _width; i++)
                {
                    toAdd[i] = '.';
                }

                Playground.Add(toAdd);
            }
        }

        private static void PaintSq(Move m, bool output)
        {
            lock (Lock)
            {
                if (output)
                {
                    Sb.AppendFormat("PAINTSQ {0} {1} {2}\n", m.R, m.C, m.S);
                    _opCounter++;
                }
            }

            Parallel.For(m.R - m.S, m.R + m.S + 1, y =>
                Parallel.For(m.C - m.S, m.C + m.S + 1, x =>
                {
                    Playground[y][x] = '#';
                }));
        }

        private static Move GetFirstCellToBePrint()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (Expected[i][j] != Playground[i][j] && Expected[i][j] == '#')
                    {
                        return new Move(i, j);
                    }
                }
            }

            return new Move(-1, -1);
        }
        
        private static Move GetRandomCellToBePrint()
        {
            int k = Rand.Next(_globalCost);
            var res = new Move();

            while (true)
            {
                Parallel.For(0, _height, i =>
                    Parallel.For(0, _width, j =>
                    {
                        if (Expected[i][j] != Playground[i][j] && Expected[i][j] == '#')
                        {
                            lock (Lock)
                            {
                                k--;

                                if (k == 0)
                                {
                                    res.R = i;
                                    res.C = j;
                                }
                            }
                        }
                    }));

                if (!res.IsDefault())
                {
                    return res;
                }
            }
        }

        private static void EraseCell(Move m, bool output)
        {
            lock (Lock)
            {
                if (output)
                {
                    Sb.AppendFormat("ERASECELL {0} {1}\n", m.R, m.C);
                    _opCounter++;
                }
            }

            Playground[m.R][m.C] = '.';
        }

        private static int DiffCount()
        {
            int res = 0;

            Parallel.For(0, _height, i =>
                Parallel.For(0, _width, j =>
                {
                    if (Expected[i][j] != Playground[i][j])
                    {
                        res += 1;

                        if (Expected[i][j] == '.')
                        {
                            res += 5;
                        }
                    }
                }));

            return res;
        }

        private static int DiffCount(Move m)
        {
            int res = 0;

            Parallel.For(m.R - m.S, m.R + m.S + 1, y =>
                Parallel.For(m.C - m.S, m.C + m.S + 1, x =>
                {
                    if (Expected[y][x] == '.')
                    {
                        res += 6;
                    }
                    else if (Playground[y][x] == '.')
                    {
                        res -= 1;
                    }
                }));

            return res;
        }

        private static IEnumerable<Move> BuildMoves(Move move)
        {
            var res = new List<Move> {new Move(move.R, move.C, move.S)};

            for (int i = 1; i < 3; i++)
            {
                res.Add(new Move(move.R, move.C, move.S + i));
                res.Add(new Move(move.R, move.C, move.S - i));
                res.Add(new Move(move.R, move.C + i, move.S));
                res.Add(new Move(move.R, move.C - i, move.S));
                res.Add(new Move(move.R, move.C + i, move.S + i));
                res.Add(new Move(move.R, move.C + i, move.S - i));
                res.Add(new Move(move.R, move.C - i, move.S + i));
                res.Add(new Move(move.R, move.C - i, move.S - i));
                res.Add(new Move(move.R + i, move.C, move.S));
                res.Add(new Move(move.R - i, move.C, move.S));
                res.Add(new Move(move.R + i, move.C, move.S + i));
                res.Add(new Move(move.R + i, move.C, move.S - i));
                res.Add(new Move(move.R - i, move.C, move.S + i));
                res.Add(new Move(move.R - i, move.C, move.S - i));
                res.Add(new Move(move.R + i, move.C + i, move.S + i));
                res.Add(new Move(move.R + i, move.C + i, move.S - i));
                res.Add(new Move(move.R + i, move.C - i, move.S + i));
                res.Add(new Move(move.R + i, move.C - i, move.S - i));
                res.Add(new Move(move.R - i, move.C + i, move.S + i));
                res.Add(new Move(move.R - i, move.C + i, move.S - i));
                res.Add(new Move(move.R - i, move.C - i, move.S + i));
                res.Add(new Move(move.R - i, move.C - i, move.S - i));
            }

            return res.Where(m => m.IsValid()).ToList();
        }

        private static void HillClimbing(int globalLimit)
        {
            _opCounter = 0;
            Sb.Clear();
            _globalCost = DiffCount();

            while (!GetFirstCellToBePrint().NotFound())
            {
                if (_globalCost < globalLimit)
                {
                    break;
                }

                Move res = GetRandomCellToBePrint();
                FindBestSquare(res);
            }

            while (!GetFirstCellToBePrint().NotFound())
            {
                Parallel.For(0, _height, i =>
                    Parallel.For(0, _width, j =>
                    {
                        if (Expected[i][j] != Playground[i][j] && Expected[i][j] == '#')
                        {
                            var res = new Move(i, j);
                            FindBestSquare(res);
                        }
                    }));
            }
        }

        private static void FindBestSquare(Move res)
        {
            Move bestMove = res;
            int bestHeuristic = DiffCount(bestMove);
            var alreadyTried = new List<Move>();

            while (true)
            {
                List<Move> moves = BuildMoves(bestMove).Where(m => !alreadyTried.Contains(m)).ToList();

                if (!moves.Any())
                {
                    break;
                }

                Parallel.ForEach(moves, m =>
                {
                    lock (Lock)
                    {
                        alreadyTried.Add(m);
                    }

                    int heuristic = DiffCount(m);

                    if (heuristic < bestHeuristic)
                    {
                        bestMove = m;
                        bestHeuristic = heuristic;
                    }
                });
            }

            _globalCost += bestHeuristic;
            PaintSq(bestMove, true);
            //Console.WriteLine("{0}, {1}", _opCounter, _globalCost);
        }

        private static void HillCleaning()
        {
            Parallel.For(0, _height, i =>
                Parallel.For(0, _width, j =>
                {
                    if (Expected[i][j] != Playground[i][j])
                    {
                        EraseCell(new Move(i, j), true);
                    }
                }));
        }

        private static int _height;
        private static int _width;
        private static int _opCounter;
        private static int _globalCost;
        private static readonly Random Rand;
        private static readonly StringBuilder Sb;
        private static readonly List<char[]> Expected;
        private static readonly List<char[]> Playground;
        private static readonly object Lock;

        static Program()
        {
            Lock = new object();
            Rand = new Random();
            Expected = new List<char[]>();
            Playground = new List<char[]>();
            Sb = new StringBuilder();
        }

        private static void Main()
        {
            ExtractData("doodle.txt");

            for (int i = 14; i <= 20; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var key = i + "_" + j;

                    GetPlayground();
                    HillClimbing(i*10000);
                    HillCleaning();
                    Console.WriteLine("globalLimit: {0}, {1}", i, _opCounter);
                    Sb.Insert(0, string.Format("{0}\n", _opCounter));

                    if (_opCounter < 19333)
                    {
                        using (var sw = new StreamWriter(string.Format("{0}_doodle_{1}.out", _opCounter, key)))
                        {
                            sw.Write(Sb.ToString());
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}