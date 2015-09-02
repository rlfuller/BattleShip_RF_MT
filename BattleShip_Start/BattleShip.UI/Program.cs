using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using BattleShip.BLL.GameLogic;
using System.Text.RegularExpressions;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Game game = setup();

            shipPlacement(game, true);
            

            Console.ReadLine();

        }


        static Game setup()
        {
            Console.WriteLine("Welcome to Battleship (start order will be randomized).");
            Console.WriteLine("Please enter the first name.");

            string p1Name = Console.ReadLine();

            Console.WriteLine("Please enter the second name.");

            string p2Name = Console.ReadLine();

            Random random = new Random();
            if (random.Next(2) == 1)
            {
                var temp = p1Name;
                p1Name = p2Name;
                p2Name = temp;
            }
            
            Game game = new Game(p1Name, p2Name);


            Console.WriteLine(p1Name + " will be Player 1. " + p2Name + " will be Player 2.");

            intializeBoard(game);

            return game;

        }
        
        //Creates 100 coordinates for each board
        static void intializeBoard(Game game)
        {
            
            //initialize dictionary inside game object, need to initialize for each board
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    Coordinate coord = new Coordinate(i, j);
                    game.p1Board.ShotHistory.Add(coord, ShotHistory.Unknown);
                    game.p2Board.ShotHistory.Add(coord, ShotHistory.Unknown);
                }
            }

            
        }

        //Draw the hit history on the console
        static void drawBoard(Game game)
        {
            //Take the board of the player whose turn it is.
            Dictionary<Coordinate, ShotHistory> currentHistory = game.p1Board.ShotHistory;
            if (game.isPlayer1Turn)
            {
                currentHistory = game.p2Board.ShotHistory;
            }
            

            for (int i = 1; i <= 10; i++)
            {
                int lineBreakCounter = 0;
                for (int j = 1; j <= 10; j++)
                {
                    Coordinate coord = new Coordinate(i, j);
                    switch (currentHistory[coord])
                    {
                        case ShotHistory.Hit:
                            Console.Write(" H ");
                            lineBreakCounter ++;
                            break;
                        case ShotHistory.Miss:
                            Console.Write(" M ");
                            lineBreakCounter ++;
                            break;
                        case ShotHistory.Unknown:
                            Console.Write(" X ");
                            lineBreakCounter ++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (lineBreakCounter >= 10)
                    {
                        Console.WriteLine("");
                    }
                }
            }
        }


        static void drawPlacementBoard(Game game, int counter)
        {
            //Take the board of the player whose turn it is.
            Board currentBoard = game.p2Board;
            if (game.isPlayer1Turn)
            {
                currentBoard = game.p1Board;
            }
            //new dictionary that holds ships positions
            List<Coordinate> shipPosition = new List<Coordinate>();

            for (int i = 0; i < counter; i++)
            {
                Coordinate[] ship = currentBoard.findShip(i);

                //get all the coordinates for each ship that has been placed
                foreach (Coordinate element in ship)
                {
                    shipPosition.Add(element); 
                }
            }

            //compare coordinate of placed ships to dictionary, if match,draw ship
            for (int i = 1; i <= 10; i++)
            {
                int lineBreakCounter = 0;
                for (int j = 1; j <= 10; j++)
                {
                    Coordinate coord = new Coordinate(i, j);
                    if (shipPosition.Contains(coord))
                    {
                        Console.Write(" S ");
                        lineBreakCounter++;
                    }
                    else
                    {
                        Console.Write( " X " );
                        lineBreakCounter++;
                    }

                    if (lineBreakCounter >= 10)
                    {
                        Console.WriteLine("");
                    }
                }
            }
        }

        static void shipPlacement(Game game, bool isPlayer1)
        {
            drawBoard(game);

            Console.WriteLine("{0}, place your ships.", game.Player1Name);

            int counter = 0;
            foreach (ShipType ship in Enum.GetValues(typeof(ShipType)))
            {
                Console.WriteLine("You will be placing a {0}",ship.ToString() );
                Console.WriteLine("Enter your coordinates for this ship.");

                Coordinate coord = inputToCoordinate();
                
                Console.WriteLine("Now enter the direction to place the {0}. Values are D, U, L, R.", ship);
                

                ShipDirection direction = directionInput();

                if (isPlayer1)
                {
                    game.p1Board.PlaceShip(new PlaceShipRequest(coord, direction, ship));
             //       Console.Clear();
                    drawPlacementBoard(game, counter);
                    counter++;
                }
                else
                {
                    game.p2Board.PlaceShip(new PlaceShipRequest(coord, direction, ship));
             //       Console.Clear();
                    drawPlacementBoard(game, counter);
                    counter++;
                }
            }
        }

        //validates ship direction from player
        private static ShipDirection directionInput()
        {
            string userDirection = "";
            bool isDirectionValid = false;
            ShipDirection result = ShipDirection.Down; 
          

            while (!isDirectionValid)
            {
                userDirection = Console.ReadLine();
                userDirection = userDirection.ToUpper();
                
                    switch (userDirection)
                    {
                        case "D":
                            result = ShipDirection.Down;
                            isDirectionValid = true;
                            break;
                        case "L":
                            result =  ShipDirection.Left;
                            isDirectionValid = true;
                            break;
                        case "R":
                            result = ShipDirection.Right;
                            isDirectionValid = true;
                            break;
                        case "U":
                            result =  ShipDirection.Up;
                            isDirectionValid = true;
                            break;
                    }
                   Console.WriteLine("Input not valid. Please type a single character: U D L R"); 
            }
            return result;
        }

        //Takes in a console line, validates it, and if successful returns a coord obj.
        static Coordinate inputToCoordinate()
        {
            string pattern1 = "^[A-Ja-j]{1}[1-9]$";

            string pattern2 = "^[A-Ja-j]{1}[1][0]$";

            Regex regex1 = new Regex(pattern1);
            Regex regex2 = new Regex(pattern2);
            

            Console.WriteLine("Please enter a letter (A-J) followed by a number 1-10.");

            char[] validLetters1 = new[] {'X', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

            bool validInput = false;
            string input = "";
            int letterAJ = 0;

            while (!validInput)
            {

                input = Console.ReadLine();
                input = input.ToUpper();

                if (regex2.IsMatch(input) || regex1.IsMatch(input))
                {
                    validInput = true;
                    for (int i = 1; i < validLetters1.Length; i++)
                    {
                        if (input[0] == validLetters1[i])
                        {
                            letterAJ = i;
                        }
                    }
                }

                else
                Console.WriteLine("not valid");
            }
            
            return new Coordinate(letterAJ, int.Parse(input.Substring(1)));
            
        }

        
        
    }
}
