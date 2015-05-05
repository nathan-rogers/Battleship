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
            Horizontal,
            Vertical
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
            for (int i = 0; i < ListOfShips.Count(); i++)
            {
                for (int j = 0; j < ListOfShips[i].Length; j++)
                {
                    PlaceShip(ListOfShips[i], PlaceShipDirection.Horizontal, i, j);
                }
            }

        }

        public void PlaceShip(Ship placeShip, PlaceShipDirection direction, int startX, int startY)
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
                Console.Write(i + 1);
                Console.WriteLine();
            }
            Console.WriteLine(" 1  2  3  4  5  6  7  8  9  10");
            Console.WriteLine("Ships Destroyed: {0}", ListOfShips.Count(x=>x.IsDestroyed == true));

        }
        public bool Target(int x, int y)
        {
            // get count before move
            int destroyedShipsBefore = ListOfShips.Count(ship=>ship.IsDestroyed == true);
            
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
            int xCoord = 1;
            int yCoord = 1;
            while (ListOfShips.Where(x => x.IsDestroyed == true).Count() != 5)
            {

                DisplayOcean();
                Console.WriteLine("Please enter an x coordinate 1- 10: ");
                int.TryParse(Console.ReadLine(), out xCoord);
                xCoord -= 1;
                Console.WriteLine("Please enter a y coordinate 1-10: ");
                int.TryParse(Console.ReadLine(), out yCoord);
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
