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
            public int Cost { get; set; }
            public Junction From { get; set;}
            public Junction To { get; set; }
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
        }

        class Car
        {
            public List<Junction> TakenJunctions { get; set; }
            public List<Street> TakenStreets { get; set; }
            public Junction CurrentJunction { get { return TakenJunctions.Last(); } }
            public int CurrentDistance { get; set; }
            public int CurrentTimer { get; set; }

            public Car(Junction first)
            {
                TakenJunctions = new List<Junction> { first };
                TakenStreets = new List<Street>();
                CurrentDistance = 0;
                CurrentTimer = 0;
            }

            public void AddJunction(Junction junction)
            {
                var street = CurrentJunction.Neighbours[junction];
                CurrentDistance += street.AlreadyUsed ? 0 : street.Length;
                CurrentTimer += street.Cost;
                street.AlreadyUsed = true;
                street.Length = 0;
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

            public Junction GetNextMove()
            {
                var current = CurrentJunction;
                var possibleMoves = current.Neighbours.Where(p => CurrentTimer + p.Value.Cost <= _timeAlloted);
                var maxDistances = possibleMoves.Where(p => p.Value.Length == possibleMoves.Max(max => max.Value.Length));
                var res = maxDistances.Where(p => p.Value.Cost == maxDistances.Min(min => min.Value.Cost)).Select(p => p.Key).FirstOrDefault();
                
                return res;
            }
        }

        private static void ExtractData(string file)
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

            // Car creation
            for (int i = 0; i < _nbCars; i++)
            {
                Cars.Add(new Car(Junctions[_startJunction]));
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
                car.Dump();
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

        private static void Main()
        {
            const string filename = "paris_54000.txt";
            ExtractData(filename);

            Run();
            FillResults();

            using (var sw = new StreamWriter(filename + ".out"))
            {
                sw.Write(Sb.ToString());
            }

            Console.ReadLine();
        }

        private static void Run()
        {
            //Cars[1].AddJunction(Junctions[1]);
            //Cars[1].AddJunction(Junctions[2]);
            Parallel.For(0, Cars.Count,
                         i =>
                             {
                                 var car = Cars[i];

                                 // Single car logic
                                 while (true)
                                 {
                                     // Single move logic
                                     Junction nextMove = car.GetNextMove();

                                     if (nextMove == null)
                                     {
                                         break;
                                     }

                                     car.AddJunction(nextMove);
                                 }
                             });
        }
    }
}
