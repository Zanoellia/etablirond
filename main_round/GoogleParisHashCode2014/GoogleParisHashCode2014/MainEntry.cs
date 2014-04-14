//#define RATIO // BETA
#define RANDOM
//#define PARALLEL

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleParisHashCode2014
{
    public class MainEntry
    {
        public class Junction
        {
            public int Id { get; private set; }
            public Dictionary<Junction, Street> Neighbours { get; private set; }

            private static int _idFactory;

            private double X { get; set; }
            private double Y { get; set; }

            public static Junction Parse(string input)
            {
                var res = new Junction();
                string[] coords = input.Split(' ');

                res.X = Double.Parse(coords[0], CultureInfo.InvariantCulture);
                res.Y = Double.Parse(coords[1], CultureInfo.InvariantCulture);
                res.Id = _idFactory++;
                res.Neighbours = new Dictionary<Junction, Street>();

                return res;
            }

            public void Populate()
            {
                Neighbours.Clear();

                IEnumerable<KeyValuePair<Junction, Street>> n =
                    Streets
                        .Where(s => s.From.Id == Id)
                        .ToDictionary(s => s.To, s => s)
                        .Union(Streets.Where(s => s.To.Id == Id && s.Direction == DirectionEnum.TwoWay).ToDictionary(s => s.From, s => s));

                foreach (KeyValuePair<Junction, Street> pair in n)
                {
                    Neighbours.Add(pair.Key, pair.Value);
                }
            }

            public new string ToString()
            {
                return String.Format("Id {0} ({1}, {2})", Id, X, Y);
            }
        }

        public enum DirectionEnum
        {
            OneWay = 1,
            TwoWay,
        }

        public class Street
        {
            public int OriginalLength { get; private set; }
            public int Cost { get; private set; }
            public Junction From { get; private set; }
            public Junction To { get; private set; }
            public DirectionEnum Direction { get; private set; }

            public int Length { get; set; }
            public int Handicap { get; set; }
            public double Bonus { get; set; }
            public bool AlreadyUsed { get; set; }

            public static Street Parse(string input)
            {
                var res = new Street();
                string[] coords = input.Split(' ');

                res.From = Junctions[Int32.Parse(coords[0])];
                res.To = Junctions[Int32.Parse(coords[1])];
                res.Direction = (DirectionEnum)Int32.Parse(coords[2]);
                res.Cost = Int32.Parse(coords[3]);
                res.Length = Int32.Parse(coords[4]);
                res.OriginalLength = res.Length;

                return res;
            }

            
            public void Update(int coef)
            {
                Bonus += From.Neighbours.Count(p => !p.Value.AlreadyUsed) * coef;

                if (Direction == DirectionEnum.TwoWay)
                {
                    Bonus += To.Neighbours.Count(p => !p.Value.AlreadyUsed) * coef;
                }
            }

            private new string ToString()
            {
                return String.Format("From {0} to {1} costing {2} len {3} dir {4}",
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

        public class Car
        {
            public List<Junction> TakenJunctions { get; private set; }
            public long CurrentDistance { get; private set; }
            public long ReusedDistance { get; private set; }

            public List<Street> TakenStreets { get; set; }
            private Junction CurrentJunction { get { return TakenJunctions.Last(); } }
            private int CurrentTimer { get; set; }

            public Car(Junction first)
            {
                TakenJunctions = new List<Junction> { first };
                TakenStreets = new List<Street>();
                CurrentDistance = 0;
                CurrentTimer = 0;
            }

            public void AddStreet(Street street, int handicap)
            {
                if (street != null)
                {
                    Junction nextJunction = street.From == CurrentJunction ? street.To : street.From;

                    AddJunction(nextJunction, handicap);
                }
            }

            public void AddJunction(Junction junction, int handicap)
            {
                lock (Lock)
                {
                    Street street = CurrentJunction.Neighbours[junction];

                    CurrentDistance += street.AlreadyUsed ? 0 : street.Length;
                    CurrentTimer += street.Cost;
                    ReusedDistance += street.AlreadyUsed ? street.OriginalLength : 0;
                    street.AlreadyUsed = true;
                    street.Length = 0;
                    street.Bonus = 0;
                    street.Handicap += handicap;
                    TakenStreets.Add(street);
                    TakenJunctions.Add(junction);
                }
            }

            public new string ToString()
            {
                return String.Format("Distance ran: {0} (+{2}), time consumed: {1}", CurrentDistance, CurrentTimer, ReusedDistance);
            }

            public void Dump()
            {
                Console.WriteLine(ToString());
            }

            private Street GetNextStreetRec(int n, ref List<Street> path, Junction taken, Street street, ref int origTimer, ref int origDistance, ref int origHandicap)
            {
                Street bestStreet = null;

                lock (Lock)
                {
                    if (street != null)
                    {
                        origTimer += street.Cost;
                        origHandicap += street.Handicap;
                        origDistance += street.AlreadyUsed || (path != null && path.Contains(street)) ? 0 : street.Length;
                    }

                    if (n == 0)
                    {
                        path = new List<Street>();
                        return street;
                    }

                    int localTimer = origTimer;
                    int bestTimer = int.MaxValue;
                    int bestDistance = int.MinValue;
                    //double bestRatio = -1;
                    var possibleMoves = taken.Neighbours.Where(p => CurrentTimer + localTimer + p.Value.Cost <= _timeAlloted).ToList();

                    foreach (KeyValuePair<Junction, Street> possibleMove in possibleMoves)
                    {
                        int currentTimer = origTimer;
                        int currentDistance = origDistance;
                        int currentHandicap = origHandicap;
                        Street test = GetNextStreetRec(n - 1, ref path, possibleMove.Key, possibleMove.Value, ref currentTimer, ref currentDistance, ref currentHandicap);

                        if (test == null)
                        {
                            continue;
                        }

                        //double ratio = currentDistance / (double)(currentTimer + currentHandicap);

                        //if (ratio > bestRatio)
                        if (currentDistance >= bestDistance)
                        {
                            if (currentTimer < bestTimer)
                            {
                                bestDistance = currentDistance;
                                bestTimer = currentTimer;
                                bestStreet = test;
                            }
                            //bestRatio = ratio;
                        }
                    }

                    if (path != null)
                    {
                        path.Add(bestStreet);
                    }

                    origTimer = bestTimer;
                    origDistance = bestDistance;
                }

                return bestStreet;
            }

            public List<Street> GetNextMoves(int n)
            {
                int currentDistance = 0;
                int currentTimer = 0;
                int handicap = 0;
                List<Street> path = null;

                GetNextStreetRec(n, ref path, CurrentJunction, null, ref currentTimer, ref currentDistance, ref handicap);

                return path;
            }

            public Junction GetNextMove()
            {
                Junction current = CurrentJunction;
                Junction res;

                lock (Lock)
                {
                    try
                    {
                        var possibleMoves = current.Neighbours.Where(p => CurrentTimer + p.Value.Cost <= _timeAlloted).ToList();
                        var oldMoves = possibleMoves.Where(p => p.Value.AlreadyUsed).ToList();
                        List<KeyValuePair<Junction, Street>> moves = possibleMoves.Where(p => !p.Value.AlreadyUsed).ToList();

                        if (oldMoves.Count == possibleMoves.Count)
                        {
                            var oldMinCost = oldMoves.Min(p => p.Value.Cost + p.Value.Handicap);
                            var oldMinCostJunctions = oldMoves.Where(p => p.Value.Cost + p.Value.Handicap == oldMinCost).ToList();
                            var oldMaxValue = oldMinCostJunctions.Max(p => (int)(p.Value.Length + p.Value.Bonus));
                            var oldMaxDistances = oldMinCostJunctions.Where(p => (int)(p.Value.Length + p.Value.Bonus) == oldMaxValue).ToList();
                            var oldTmp = oldMaxDistances.Select(p => p.Key).ToList();

                            int oldCount = oldTmp.Count();
                            return oldCount > 1 ? oldTmp.ToList()[Rand.Next(oldCount)] : oldTmp.FirstOrDefault();
                        }
                        
#if RATIO
                        int maxRatio = moves.Max(p => (int)((float)(p.Value.Length + p.Value.Bonus) / (float)(p.Value.Cost + p.Value.Handicap)));
                        var ratios = moves.Where(p => (int)((float)(p.Value.Length + p.Value.Bonus) / (float)(p.Value.Cost + p.Value.Handicap)) == maxRatio);
                        var tmp = ratios.Select(p => p.Key).ToList();
#else

                        int maxValue = moves.Max(p => (int)(p.Value.Length + p.Value.Bonus));
                        var maxDistances = moves.Where(p => (int)(p.Value.Length + p.Value.Bonus) == maxValue).ToList();
                        int minCost = maxDistances.Min(p => p.Value.Cost + p.Value.Handicap);
                        var tmp = maxDistances.Where(p => p.Value.Cost + p.Value.Handicap == minCost).Select(p => p.Key).ToList(); 
                        //var tmp = possibleMoves.OrderByDescending(p => p.Value.Length).Take(2).Select(p => p.Key);

                        /*var minCost = moves.Min(p => p.Value.Cost + p.Value.Handicap);
                      var minCostJunctions = moves.Where(p => p.Value.Cost + p.Value.Handicap == minCost).ToList();
                      var maxValue = minCostJunctions.Max(p => (int)(p.Value.Length + p.Value.Bonus));
                      var maxDistances = minCostJunctions.Where(p => (int)(p.Value.Length + p.Value.Bonus) == maxValue).ToList();
                      var tmp = maxDistances.Select(p => p.Key).ToList(); */
#endif

#if RANDOM
                        int count = tmp.Count();
                        res = count > 1 ? tmp.ToList()[Rand.Next(count)] : tmp.FirstOrDefault();
#else
                        res = tmp.FirstOrDefault();
#endif
                    }
                    catch
                    {
                        return null;
                    }
                }
                return res;
            }
        }

        /*
    private static void Promote(int bonus)
    {
        List<Street> toBePromoted = Streets.Where(s => !s.AlreadyUsed).OrderByDescending(s => s.Length).Take(10).ToList();

        foreach (Street street in toBePromoted)
        {
            street.Bonus += bonus;
        }

        bonus /= 2;

        foreach (Junction junction in toBePromoted.Select(s => s.From).Union(toBePromoted.Select(s => s.To)))
        {
            foreach (Street street in junction.Neighbours.Select(p => p.Value))
            {
                street.Bonus += bonus;
            }
        }
    }*/

        private static void ExtractData(string file, string preloadFile)
        {
            string[] lines = File.ReadAllLines(file);
            string[] dim = lines[0].Split(' ');

            _nbJunctions = Int32.Parse(dim[0]);
            //_nbStreets = Int32.Parse(dim[1]); // UNUSED
            _timeAlloted = Int32.Parse(dim[2]);
            _nbCars = Int32.Parse(dim[3]);
            _startJunction = Int32.Parse(dim[4]);

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
                string[] preload = File.ReadAllLines(preloadFile);
                int preloadIndex = 1;

                for (int i = 0; i < _nbCars; i++)
                {
                    int parseIndex = int.Parse(preload[preloadIndex]);
                    var junctions = new List<Junction>();

                    for (int j = 2; j < parseIndex; j++)
                    {
                        int junctionId = int.Parse(preload[preloadIndex + j]);
                        Junction junction = Junctions[junctionId];

                        junctions.Add(junction);
                    }

                    Preloader.Add(junctions);
                    preloadIndex += parseIndex + 1;
                }
            }

            Parallel.ForEach(Junctions.Values, junction => junction.Populate());
            
            /*
            int len = 0;
            List<Street> culDeSacs;
            do
            {
                culDeSacs = Streets.Where(p => p.Direction == DirectionEnum.TwoWay && (p.From.Neighbours.Count == 1 || p.To.Neighbours.Count == 1)).ToList();
                
                foreach (var culDeSac in culDeSacs)
                {
                    culDeSac.To.Neighbours.Remove(culDeSac.From);
                    culDeSac.From.Neighbours.Remove(culDeSac.To);
                    len += culDeSac.Length;
                }
                Streets.RemoveAll(s => s.To.Neighbours.Count == 0 || s.From.Neighbours.Count == 0);
                Parallel.ForEach(Junctions.Values, junction => junction.Populate());
                Console.WriteLine("nb cds: {0}, {1}", culDeSacs.Count(), len);
            } while (culDeSacs.Count() != 0);
            //Console.ReadLine();*/
        }

        static void FillResults()
        {
            Sb.AppendFormat("{0}\n", Cars.Count);

            foreach (Car car in Cars)
            {
                Sb.AppendFormat("{0}\n", car.TakenJunctions.Count);

                foreach (Junction move in car.TakenJunctions)
                {
                    Sb.AppendFormat("{0}\n", move.Id);
                }
            }
        }

        public static readonly Random Rand = new Random();

        //private static int _nbStreets;
        private static int _nbJunctions;
        private static int _timeAlloted;
        private static int _nbCars;
        private static int _startJunction;
        private static readonly object Lock = new object();
        private static readonly Dictionary<int, Junction> Junctions = new Dictionary<int, Junction>();
        private static readonly List<Street> Streets = new List<Street>();
        private static readonly List<Car> Cars = new List<Car>();
        private static readonly StringBuilder Sb = new StringBuilder();
        private static readonly List<List<Junction>> Preloader = new List<List<Junction>>();
        private static readonly Dictionary<string, long> Results = new Dictionary<string, long>();

        private static void HillClimbing(string filename)
        {
            var t = new Timer(o =>
            {
                lock (Lock)
                {
                    Cars.ForEach(car => car.Dump());
                    long curLen = Cars.Sum(c => c.CurrentDistance);
                    long reusedLen = Cars.Sum(c => c.ReusedDistance);
                    Console.WriteLine("{0} (+{1})", curLen, reusedLen);
                }
            });

            for (int i = 40; i <= 300; i++)
           // while (true)
            {
                //int i = Car.Rand.Next(20, 200);
                for (int j = 0; j <= 0; j++)
                {
                    string key = i  + "_" + j;

                    Init(i);
                    t.Change(1000, 2500);
                    Run(i, key);
                    FillResults();

                    if (Results.ContainsKey(key))
                    {
                        using (var sw = new StreamWriter(String.Format("{0}_{1}_{2}.out", Results[key], filename, key)))
                        {
                            sw.Write(Sb.ToString());
                        }
                    }
                   t.Change(0, Timeout.Infinite);
                   Console.ReadLine();
                }
            }

            try
            {
                KeyValuePair<string, long> maxRes = Results.First(p => p.Value == Results.Max(max => max.Value));
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

            foreach (Street street in Streets)
            {
                street.Reset();
            }

            for (int i = 0; i < Preloader.Count; i++)
            {
                List<Junction> junctions = Preloader[i];

                foreach (Junction junction in junctions)
                {
                    Cars[i].AddJunction(junction, handicap);
                }
            }
        }

        private static void Main()
        {
            const string filename = "paris_54000.txt";

            ExtractData(filename, null); //"preload.txt");
            HillClimbing(filename);

            //Console.ReadLine();
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
                    /*
                    // Single move logic
                    Junction nextMove = car.GetNextMove();

                    if (nextMove == null)
                    {
                        break;
                    }

                    car.AddJunction(nextMove, handicap);
                    // Promote
                    foreach (var street in Streets)
                    {
                        if (!street.AlreadyUsed)
                        {
                            street.Update(1);
                        }
                    }*/
                    List<Street> nextMoves = car.GetNextMoves(2);

                    if (nextMoves == null || (nextMoves.Count == 1 && nextMoves[0] == null))
                    {
                        break;
                    }

                    foreach (var street in nextMoves)
                    {
                        car.AddStreet(street, handicap);
                    }
                }
            }
#if PARALLEL
                );
#endif
            /*
            foreach (var car in Cars)
            {
                var streets = car.TakenStreets;

                foreach (var street in streets)
                {
                    street.Bonus += car.CurrentDistance / 10d;
                }
            }*/

            long curLen = Cars.Sum(c => c.CurrentDistance);
            long reusedLen = Cars.Sum(c => c.ReusedDistance);

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
