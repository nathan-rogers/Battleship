using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid = new Grid();
            grid.PlayGame();
            Console.ReadKey();
        }

    }

    #region
    /// <summary>
    /// generates a type of spot on the board
    /// </summary>
    public class Point
    {
        //4 kinds of point
        public enum PointStatus
        {
            Empty,
            Ship,
            Hit,
            Miss
        }
        //coordinates
        public int X { get; set; }
        public int Y { get; set; }
        public PointStatus Status { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        public Point(int x, int y, PointStatus p)
        {
            this.X = x;
            this.Y = y;
            this.Status = p;
        }


    }
    #endregion
    #region
    /// <summary>
    /// ship type
    /// </summary>
    public class Ship
    {
        //MAY THE 4th BE WITH YOU!!!
        //Star wars themed battleship!
        public enum ShipType
        {
            //5hits
            StarCruiser,
            //4
            MilleniumFalcon,
            //3
            XWing,
            //3
            YWing,
            //2
            AWing
        }

        //properties of ships
        public ShipType Type { get; set; }
        public List<Point> OccupiedPoints { get; set; }
        public int Length { get; set; }
        public bool IsDestroyed
        {
            get
            {
                return OccupiedPoints.Count(x => x.Status == Point.PointStatus.Hit) == OccupiedPoints.Count();
            }
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="typeOfShip"></param>
        public Ship(ShipType typeOfShip)
        {
            this.OccupiedPoints = new List<Point>();
            this.Type = typeOfShip;
            switch (typeOfShip)
            {
                case ShipType.AWing:
                    Length = 2;
                    break;
                case ShipType.XWing:
                    Length = 3;
                    break;
                case ShipType.YWing:
                    Length = 3;
                    break;
                case ShipType.MilleniumFalcon:
                    Length = 4;
                    break;
                case ShipType.StarCruiser:
                    Length = 5;
                    break;
            }
        }
    }
    #endregion
    #region
    /// <summary>
    /// creates the playing board
    /// </summary>
    public class Grid
    {
        //face the ship being placed
        public enum PlaceShipDirection
        {
            Vertical,
            Horizontal
        }
        //create multi-dimensional array for display and storage of point types
        public Point[,] Ocean { get; set; }
        //list of ships that have been placed
        public List<Ship> ListOfShips { get; set; }
        //when all ships are destroyed
        public bool AllShipsDestroyed
        {
            get
            {
                return ListOfShips.Where(x => x.IsDestroyed == true).Count() == 5;
            }
        }
        //count number of combat rounds
        public int CombatRound { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public Grid()
        {
            //set size of array
            this.Ocean = new Point[10, 10];
            //x axis of array
            for (int i = 0; i < Ocean.GetLength(0); i++)
            {
                //y axis
                for (int j = 0; j < Ocean.GetLength(1); j++)
                {
                    //generate each empty point of array
                    Ocean[i, j] = new Point(i, j, Point.PointStatus.Empty);
                }
            }
            //generate 5 ships to shoot at
            this.ListOfShips = new List<Ship>() 
            { 
                new Ship(Ship.ShipType.AWing), 
                new Ship(Ship.ShipType.XWing),
                new Ship(Ship.ShipType.YWing),
                new Ship(Ship.ShipType.MilleniumFalcon),
                new Ship(Ship.ShipType.StarCruiser)
            };
            Random rng = new Random();
            int placeX = 0;
            int placeY = 0;
            for (int i = 0; i < ListOfShips.Count(); i++)
            {

                int rotation = 0;
                //while a direction has not been selected
                while (rotation == 0)
                {
                    //select starting x coord
                    placeX = rng.Next(0, 11);
                    //starting y coord
                    placeY = rng.Next(0, 11);
                    //horizontal or vertical
                    rotation = rng.Next(1, 2);
                    
                    switch (rotation)
                    {   //vertical
                        case 1:
                            //while starting X + ship length DOES NOT fit on the board
                            while (!((placeX + ListOfShips[i].Length) < 10)) 
                            {
                                //try a new x coord
                                placeX = rng.Next(0, 11);
                                
                            }
                            //if it fits on the board
                            if ((placeX + ListOfShips[i].Length) < 10)
                            {
                                //make sure ships do not intersect
                                for (int j = 0; j < ListOfShips[i].Length; j++)
                                {
                                    //if they do intersect
                                    if (Ocean[placeX + j, placeY].Status == Point.PointStatus.Ship)
                                    {
                                        //start randomization section over
                                        rotation = 0;
                                        break;
                                    }
                                }
                            }
                            //if it passes everything, place a ship calling PlaceShip with randomized numbers
                            PlaceShip(ListOfShips[i], (PlaceShipDirection)rotation, placeX, placeY);
                            break;

                        case 2:
                            while (!((placeY + ListOfShips[i].Length) < 10))
                            {
                                placeY = rng.Next(0, 11);
                                
                            }
                            if ((placeX + ListOfShips[i].Length) < 10)
                            {
                                for (int j = 0; j < ListOfShips[i].Length; j++)
                                {
                                    if (Ocean[placeX, placeY + j].Status == Point.PointStatus.Ship)
                                    {
                                        rotation = 0;
                                        break;
                                    }
                                }
                            }

                            PlaceShip(ListOfShips[i], (PlaceShipDirection)rotation, placeX, placeY);
                            break;
                        default:
                            rotation = 0;
                            break;

                    }
                }
            }

        }

        public void PlaceShip(Ship placeShip, PlaceShipDirection direction, int startX, int startY)
        {

            for (int i = 0; i < placeShip.Length; i++)
            {


                Ocean[startX, startY].Status = Point.PointStatus.Ship;
                placeShip.OccupiedPoints.Add(Ocean[startX, startY]);
                if (direction == PlaceShipDirection.Horizontal)
                {
                    startX++;
                }
                else if (direction == PlaceShipDirection.Vertical)
                {
                    startY++;
                }
            }

        }
        //Prints out array
        public void DisplayOcean()
        {
            //x axis
            for (int i = 0; i < Ocean.GetLength(0); i++)
            {
                //yaxis
                for (int j = 0; j < Ocean.GetLength(1); j++)
                {
                    //selects grid square per array index
                    switch (Ocean[i, j].Status)
                    {
                        case Point.PointStatus.Empty:
                            StringColorizer("[ ]", ConsoleColor.DarkBlue);
                            break;
                        case Point.PointStatus.Ship:
                            StringColorizer("[ ]", ConsoleColor.DarkBlue);
                            break;
                        case Point.PointStatus.Hit:
                            StringColorizer("[X]", ConsoleColor.DarkRed);
                            break;
                        case Point.PointStatus.Miss:
                            StringColorizer("[O]", ConsoleColor.Blue);
                            break;
                    }
                }
                //print grid numbers
                Console.Write("X");
                Console.Write(i + 1);
                Console.WriteLine();
            }
            Console.WriteLine("Y1 Y2 Y3 Y4 Y5 Y6 Y7 Y8 Y9 Y10 ");
            Console.WriteLine("Ships Destroyed: {0}", ListOfShips.Count(x => x.IsDestroyed == true));

        }
        public bool Target(int x, int y)
        {
            // get count before move
            int destroyedShipsBefore = ListOfShips.Count(ship => ship.IsDestroyed == true);

            if (Ocean[x, y].Status == Point.PointStatus.Ship)
            {
                Ocean[x, y].Status = Point.PointStatus.Hit;

                //destroyedShipAfter = ListOfShips.Where(a => a.IsDestroyed == true).Count();

            }
            else if (Ocean[x, y].Status == Point.PointStatus.Empty)
            {
                Ocean[x, y].Status = Point.PointStatus.Miss;
            }

            // get the count after the user move
            int destroyedShipAfter = ListOfShips.Count(ship => ship.IsDestroyed == true);
            if (destroyedShipsBefore < destroyedShipAfter)
            {
                //destroyedShipsBefore = destroyedShipAfter;
                return true; // user sunk a ship this turn
            }
            return false;
        }
        public void PlayGame()
        {
            while (ListOfShips.Where(x => x.IsDestroyed == true).Count() != 5)
            {

                int xCoord = 0;
                int yCoord = 0;
                DisplayOcean();
                while (xCoord == 0)
                {

                    Console.WriteLine("Please enter an x coordinate 1 - 10: ");
                    int.TryParse(Console.ReadLine(), out xCoord);
                    if (xCoord == 0)
                    {
                        Console.Clear();
                        DisplayOcean();
                        Console.WriteLine("Please enter a legitimate coordinate!");
                    }
                }

                xCoord -= 1;
                while (yCoord == 0)
                {

                    Console.WriteLine("Please enter a y coordinate 1 - 10: ");
                    int.TryParse(Console.ReadLine(), out yCoord);
                    if (yCoord == 0)
                    {

                        Console.Clear();
                        DisplayOcean();
                        Console.WriteLine("Please enter a legitimate coordinate!");
                    }
                }

                yCoord -= 1;
                Target(xCoord, yCoord);
                Console.Clear();
                CombatRound += 1;
            }
        }
        public void StringColorizer(string symbols, ConsoleColor color)
        {
            Console.BackgroundColor = color;

            Console.Write(symbols);
            Console.ResetColor();

        }
    }
    #endregion
}
