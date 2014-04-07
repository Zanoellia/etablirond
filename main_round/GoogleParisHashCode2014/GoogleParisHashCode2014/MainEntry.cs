﻿//#define RATIO // BETA
#define RANDOM
//#define PARALLEL

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GoogleParisHashCode2014
{
    internal class MainEntry
    {
        class Junction
        {
            private static int _idFactory;
            public int Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public Dictionary<Junction, Street> Neighbours { get; set; }

            public static Junction Parse(string input)
            {
                var res = new Junction();
                var coords = input.Split(' ');
                res.X = double.Parse(coords[0], CultureInfo.InvariantCulture);
                res.Y = double.Parse(coords[1], CultureInfo.InvariantCulture);
                res.Id = _idFactory++;
                res.Neighbours = new Dictionary<Junction, Street>();
                return res;
            }

            public void Populate()
            {
                var n = Streets.Where(s => s.From.Id == Id).ToDictionary(s => s.To, s => s).Union(Streets.Where(s => s.To.Id == Id && s.Direction == DirectionEnum.TwoWay).ToDictionary(s => s.From, s => s));
                foreach (var pair in n)
                {
                    Neighbours.Add(pair.Key, pair.Value);
                }  
            }

            public new string ToString()
            {
                return string.Format("Id {0} ({1}, {2})", Id, X, Y);
            }

            public void Dump()
            {
                Console.WriteLine(ToString());
            }
        }

        enum DirectionEnum
        {
            OneWay = 1,
            TwoWay,
        }

        class Street
        {
            public DirectionEnum Direction { get; set; }
            public int Length { get; set; }
            public int OriginalLength { get; set; }
            public int Cost { get; set; }
            public Junction From { get; set;}
            public Junction To { get; set; }
            public int Handicap { get; set; }
            public bool AlreadyUsed { get; set; }

            public static Street Parse(string input)
            {
                var res = new Street();
                var coords = input.Split(' ');
                res.From = Junctions[int.Parse(coords[0])];
                res.To = Junctions[int.Parse(coords[1])];
                res.Direction = (DirectionEnum)int.Parse(coords[2]);
                res.Cost = int.Parse(coords[3]);
                res.Length = int.Parse(coords[4]);
                res.OriginalLength = res.Length;
                return res;
            }

            public new string ToString()
            {
                return string.Format("From {0} to {1} costing {2} len {3} dir {4}",
                                     From.ToString(), To.ToString(), Cost, Length, Direction);
            }

            public void Dump()
            {
                Console.WriteLine(ToString());
            }

            public void Reset()
            {
                Length = OriginalLength;
                Handicap = 0;
                AlreadyUsed = false;
            }
        }

        class Car
        {
            public List<Junction> TakenJunctions { get; set; }
            public List<Street> TakenStreets { get; set; }
            public Junction CurrentJunction { get { return TakenJunctions.Last(); } }
            public long CurrentDistance { get; set; }
            public long ReusedDistance { get; set; }
            public int CurrentTimer { get; set; }

            public Car(Junction first)
            {
                TakenJunctions = new List<Junction> { first };
                TakenStreets = new List<Street>();
                CurrentDistance = 0;
                CurrentTimer = 0;
            }

            public void AddJunction(Junction junction, int handicap)
            {
                var street = CurrentJunction.Neighbours[junction];
                CurrentDistance += street.AlreadyUsed ? 0 : street.Length;
                CurrentTimer += street.Cost;
                ReusedDistance += street.AlreadyUsed ? street.OriginalLength : 0;
                street.AlreadyUsed = true;
                street.Length = 0;
                street.Handicap = street.Handicap == 0 ? handicap : street.Handicap * 2;
                TakenStreets.Add(street);
                TakenJunctions.Add(junction);
            }

            public new string ToString()
            {
                return string.Format("Distance ran: {0}, time consumed: {1}", CurrentDistance, CurrentTimer);
            }

            public void Dump()
            {
                Console.WriteLine(ToString());
            }

            private static readonly object Lock = new object();
            public static readonly Random Rand = new Random();


            public Junction GetNextMove()
            {
                Junction current = CurrentJunction;
                Junction res = null;

                lock (Lock)
                {
                    try
                    {
                        var possibleMoves = current.Neighbours.Where(p => CurrentTimer + p.Value.Cost <= _timeAlloted);
#if RATIO
                        int maxRatio = possibleMoves.Max(p => (int)((float)p.Value.Length / (float)(p.Value.Cost + p.Value.Handicap)));
                        var ratios = possibleMoves.Where(p => (int)((float)p.Value.Length / (float)(p.Value.Cost + p.Value.Handicap)) == maxRatio);
                        var tmp = ratios.Select(p => p.Key);
#else
                       var maxValue = possibleMoves.Max(p => p.Value.Length);
                        var maxDistances = possibleMoves.Where(p => p.Value.Length == maxValue);
                        var minCost = maxDistances.Min(p => p.Value.Cost + p.Value.Handicap);
                        var tmp = maxDistances.Where(p => p.Value.Cost + p.Value.Handicap == minCost).Select(p => p.Key); 
                        //var tmp = possibleMoves.OrderByDescending(p => p.Value.Length).Take(2).Select(p => p.Key); 
#endif

#if RANDOM
                        var count = tmp.Count();
                        res = count > 1 ? tmp.ToList()[Rand.Next(count)] : tmp.FirstOrDefault();
#else
                        res = tmp.FirstOrDefault();
#endif
                    }
                    catch (Exception)
                    {
                        
                    }
                }
                return res;
            }
        }

        private static void ExtractData(string file, string preloadFile)
        {
            var lines = File.ReadAllLines(file);
            var dim = lines[0].Split(' ');

            _nbJunctions = int.Parse(dim[0]);
            _nbStreets = int.Parse(dim[1]);
            _timeAlloted = int.Parse(dim[2]);
            _nbCars = int.Parse(dim[3]);
            _startJunction = int.Parse(dim[4]);

            for (int i = 1; i < 1 + _nbJunctions; i++)
            {
                Junction junction = Junction.Parse(lines[i]);
                Junctions.Add(junction.Id, junction);
            }

            for (int i = 1 + _nbJunctions; i < lines.Length; i++)
            {
                Street street = Street.Parse(lines[i]);
                Streets.Add(street);
            }

            if (preloadFile != null)
            {
                var preload = File.ReadAllLines(preloadFile);
                var preloadIndex = 1;

                for (int i = 0; i < _nbCars; i++)
                {
                    var parseIndex = int.Parse(preload[preloadIndex]);
                    var junctions = new List<Junction>();

                    for (int j = 2; j < parseIndex; j++)
                    {
                        var junctionId = int.Parse(preload[preloadIndex + j]);
                        var junction = Junctions[junctionId];
                        junctions.Add(junction);
                    }

                    Preloader.Add(junctions);
                    preloadIndex += parseIndex + 1;
                }
            }

            /*
            foreach (var junction in TakenJunctions.Values)
            {
                junction.Dump();
            }

            foreach (var street in Streets)
            {
                street.Dump();
            }*/

            Parallel.For(0, Junctions.Values.ToArray().Length,
                         i =>
                             {
                                 var junction = Junctions.Values.ToList()[i];
                                 junction.Populate();
                             });
        }

        static void FillResults()
        {
            Sb.AppendFormat("{0}\n", Cars.Count);
            foreach (var car in Cars)
            {
                Sb.AppendFormat("{0}\n", car.TakenJunctions.Count);
                foreach (var move in car.TakenJunctions)
                {
                    Sb.AppendFormat("{0}\n", move.Id);
                }
            }
        }

        private static int _nbJunctions;
        private static int _nbStreets;
        private static int _timeAlloted;
        private static int _nbCars;
        private static int _startJunction;
        private static readonly Dictionary<int, Junction> Junctions = new Dictionary<int, Junction>();
        private static readonly List<Street> Streets = new List<Street>();
        private static readonly List<Car> Cars = new List<Car>();
        private static readonly StringBuilder Sb = new StringBuilder();
        private static readonly List<List<Junction>> Preloader = new List<List<Junction>>();
        private static readonly Dictionary<string, long> Results = new Dictionary<string, long>();

        private static void HillClimbing(string filename)
        {
            for (int i = 100; i <= 300; i++)
           // while (true)
            {
                //int i = Car.Rand.Next(20, 200);
                for (int j = 0; j <= 500; j++)
                {
                    string key = i  + "_" + j;

                    Init(i);
                    Run(i, key);
                    FillResults();

                    if (Results.ContainsKey(key))
                    {
                        using (var sw = new StreamWriter(string.Format("{0}_{1}_{2}.out", Results[key], filename, key)))
                        {
                            sw.Write(Sb.ToString());
                        }
                    }
                }
            }

            try
            {
                var maxRes = Results.First(p => p.Value == Results.Max(max => max.Value));
                Console.WriteLine("Max: handicap {0} : {1}", maxRes.Key, maxRes.Value);
            }
            catch (Exception)
            {
                Console.WriteLine("No max found");
            }
        }

        private static void Init(int handicap)
        {
            Cars.Clear();
            Sb.Clear();

            // Car creation
            for (int i = 0; i < _nbCars; i++)
            {
                Cars.Add(new Car(Junctions[_startJunction]));
            }

            foreach (var street in Streets)
            {
                street.Reset();
            }

            for (int i = 0; i < Preloader.Count; i++)
            {
                var junctions = Preloader[i];

                foreach (var junction in junctions)
                {
                    Cars[i].AddJunction(junction, handicap);
                }
            }
        }

        private static void Main()
        {
            const string filename = "paris_54000.txt";

            ExtractData(filename, "preload.txt");
            HillClimbing(filename);

            Console.ReadLine();
        }

        private static void Run(int handicap, string key)
        {
#if PARALLEL
            Parallel.ForEach(Cars, car =>
#else
            foreach (var car in Cars)
#endif
            {
                // Single car logic
                while (true)
                {
                    // Single move logic
                    Junction nextMove = car.GetNextMove();

                    if (nextMove == null)
                    {
                        break;
                    }

                    car.AddJunction(nextMove, handicap);
                }
            }
#if PARALLEL
            );
#endif
            var curLen = Cars.Sum(c => c.CurrentDistance);
            var reusedLen = Cars.Sum(c => c.ReusedDistance);
            if (curLen > 1475000)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            if (curLen > 1500000)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (curLen > 1525000)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("!");
            }
            if (curLen > 1547743)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!!!");
                Results.Add(key, curLen);
            }
            Console.WriteLine("Handicap {0} = {1} (+{2})", handicap, curLen, reusedLen);
            if (curLen > 1400000)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
