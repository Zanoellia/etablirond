using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GoogleParisHashCode2014
{
    internal class Program
    {
        private static int _height;
        private static int _width;
        private const int Sjump = 1;
        private const int Rjump = 1;
        private const int Cjump = 1;

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

            public bool NotFound()
            {
                return R == -1 && C == -1;
            }

            public bool IsValid()
            {
                return R >= 0 && R < _height && C >= 0 && C < _width && S >= 0 && R - S >= 0 && C - S >= 0 &&
                       R + S < _height && C + S < _width;
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

        private static List<char[]> ExtractData(string file)
        {
            var lines = File.ReadAllLines(file);
            var res = new List<char[]>();
            var dim = lines[0].Split(' ');
            _height = int.Parse(dim[0]);
            _width = int.Parse(dim[1]);

            for (int index = 1; index < lines.Length; index++)
            {
                var line = lines[index];
                res.Add(line.ToCharArray());
            }

            return res;
        }

        private static List<char[]> GetPlayground(IEnumerable<char[]> model)
        {
            var res = new List<char[]>();

            foreach (var list in model)
            {
                var toAdd = (char[])list.Clone();

                for (int i = 0; i < toAdd.Length; i++)
                {
                    toAdd[i] = '.';
                }

                res.Add(toAdd);
            }

            return res;
        }

        private static List<char[]> PaintSq(List<char[]> playground, Move m, bool output)
        {
            if (output)
            {
                Sb.AppendFormat("PAINTSQ {0} {1} {2}\n", m.R, m.C, m.S);
                _opCounter++;
            }
            var res = new List<char[]>();

            for (int y = m.R - m.S; y <= m.R + m.S; y++)
            {
                var chars = new char[m.S * 2 + 1];

                for (int x = m.C - m.S; x <= m.C + m.S; x++)
                {
                    chars[x - (m.C - m.S)] = playground[y][x];
                    playground[y][x] = '#';
                }

                res.Add(chars);
            }
            return res;
        }

        private static Move GetFirstCellToBePrint(List<char[]> expected, List<char[]> playground)
        {
            for (int i = _startSearch; i < expected.Count; i++)
            {
                var exp = expected[i];
                _startSearch = Math.Max(_startSearch, i);
                for (int j = 0; j < exp.Length; j++)
                {
                    if (expected[i][j] != playground[i][j] && expected[i][j] == '#')
                    {
                        return new Move(i, j);
                    }
                }
            }
            return new Move(-1, -1);
        }

        private static void EraseCell(List<char[]> playground, Move m, bool output)
        {
            if (output)
            {
                Sb.AppendFormat("ERASECELL {0} {1}\n", m.R, m.C);
                _opCounter++;
            }
            playground[m.R][m.C] = '.';
        }

        private static long DiffCount(List<char[]> expected, List<char[]> playground)
        {
            long res = 0;
            for (int i = 0; i < expected.Count; i++)
            {
                var exp = expected[i];
                for (int j = 0; j < exp.Length; j++)
                {
                    if (expected[i][j] != playground[i][j])
                    {
                        res += expected[i][j] == '.' ? 2 : 1;
                    }
                }
            }
            return res;
        }

        private static void Reset(List<char[]> src, List<char[]> dst, Move m)
        {
            for (int y = m.R - m.S; y <= m.R + m.S; y++)
            {
                for (int x = m.C - m.S; x <= m.C + m.S; x++)
                {
                    dst[y][x] = src[y - (m.R - m.S)][x - (m.C - m.S)];
                }
            }
        }

        private static IEnumerable<Move> BuildMoves(Move move)
        {
            var res = new List<Move>
            {
                new Move(move.R, move.C, move.S),
                new Move(move.R, move.C, move.S + Sjump),
                new Move(move.R, move.C, move.S - Sjump),
                new Move(move.R, move.C + Cjump, move.S),
                new Move(move.R, move.C - Cjump, move.S),
                new Move(move.R, move.C + Cjump, move.S + Sjump),
                new Move(move.R, move.C + Cjump, move.S - Sjump),
                new Move(move.R, move.C - Cjump, move.S + Sjump),
                new Move(move.R, move.C - Cjump, move.S - Sjump),
                new Move(move.R + Rjump, move.C, move.S),
                new Move(move.R - Rjump, move.C, move.S),
                new Move(move.R + Rjump, move.C, move.S + Sjump),
                new Move(move.R + Rjump, move.C, move.S - Sjump),
                new Move(move.R - Rjump, move.C, move.S + Sjump),
                new Move(move.R - Rjump, move.C, move.S - Sjump),
                new Move(move.R + Rjump, move.C + Cjump, move.S + Sjump),
                new Move(move.R + Rjump, move.C + Cjump, move.S - Sjump),
                new Move(move.R + Rjump, move.C - Cjump, move.S + Sjump),
                new Move(move.R + Rjump, move.C - Cjump, move.S - Sjump),
                new Move(move.R - Rjump, move.C + Cjump, move.S + Sjump),
                new Move(move.R - Rjump, move.C + Cjump, move.S - Sjump),
                new Move(move.R - Rjump, move.C - Cjump, move.S + Sjump),
                new Move(move.R - Rjump, move.C - Cjump, move.S - Sjump),
            };

            return res.Where(m => m.IsValid()).ToList();
        }

        private static void HillClimbing(List<char[]> expected, List<char[]> playground)
        {
            while (!GetFirstCellToBePrint(expected, playground).NotFound())
            {
                for (int i = 0; i < expected.Count; i++)
                {
                    var exp = expected[i];
                    for (int j = 0; j < exp.Length; j++)
                    {
                        if (expected[i][j] != playground[i][j] && expected[i][j] == '#')
                        {
                            var res = new Move(i, j);
                            Move bestMove = res;
                            long bestHeuristic = DiffCount(playground, expected);

                            var alreadyTried = new List<Move>();

                            while (true)
                            {
                                var moves = BuildMoves(bestMove).Where(m => !alreadyTried.Contains(m)).ToList();

                                if (!moves.Any())
                                {
                                    break;
                                }

                                foreach (var m in moves)
                                {
                                    alreadyTried.Add(m);
                                    var bak = PaintSq(playground, m, false);
                                    long heuristic = DiffCount(expected, playground);

                                    if (heuristic < bestHeuristic)
                                    {
                                        bestMove = m;
                                        bestHeuristic = heuristic;
                                    }
                                    Reset(bak, playground, m);
                                }
                            }

                            PaintSq(playground, bestMove, true);
                            Console.WriteLine(_opCounter);
                        }
                    }
                }
            }
        }

        private static void HillCleaning(List<char[]> expected, List<char[]> playground)
        {
            for (int i = 0; i < expected.Count; i++)
            {
                var exp = expected[i];
                for (int j = 0; j < exp.Length; j++)
                {
                    if (expected[i][j] != playground[i][j])
                    {
                        EraseCell(playground, new Move(i, j), true);
                    }
                }
            }
        }

        private static readonly StringBuilder Sb = new StringBuilder();
        private static int _opCounter;
        private static int _startSearch;

        private static void Main()
        {
            var expected = ExtractData("simple.txt");
            List<char[]> playground = GetPlayground(expected);

            HillClimbing(expected, playground);
            HillCleaning(expected, playground);
            Console.WriteLine(DiffCount(playground, expected));
            Sb.Insert(0, string.Format("{0}\n", _opCounter));

            using (var sw = new StreamWriter("simple.out"))
            {
                sw.Write(Sb.ToString());
            }
        }
    }
}
