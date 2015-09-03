using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using BattleShip.BLL.GameLogic;
using System.Text.RegularExpressions;
using System.Threading;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine(@"     .  o ..                  ");
            Console.WriteLine(@"     o . o o.o                ");
            Console.WriteLine(@"          ...oo               ");
            Console.WriteLine(@"            __[]__            ");
            Console.WriteLine(@"         __|_o_o_o\__         ");
            Console.WriteLine(@"         \**********/         ");
            Console.WriteLine(@"          \. ..  . /          ");
            Console.WriteLine(@"     ^^^^^^^^^^^^^^^^^^^^     ");
            Console.WriteLine();
            Console.WriteLine(@"     WELCOME TO BATTLESHIP     ");
            Console.WriteLine(@"       press START button      ");

            Console.ReadLine();

            Console.Clear();

            bool loop = true;

            while (loop)
            {
                Game game = setup();

                for (int i = 0; i < 2; i++)
                {
                    shipPlacement(game, true);
                    game.TurnToggle();
                    Console.Clear();
                }

                Console.WriteLine("Ship placement done. " + game.Player1Name + " will now begin his turn.");

                bool gameOver = false;
                while (!gameOver)
                {
                    gameOver = runOneTurn(game);
                    game.TurnToggle();
                }
                Console.ReadLine();

                Console.WriteLine("Would you like to play again? Type \"yes\" to play again. Any other input to quit");

                if (Console.ReadLine().ToLower() != "yes")
                    loop = false;
            }

        }


        private static Game setup()
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

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(p1Name);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write( " will be Player 1. ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(p2Name);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" will be Player 2.");
            Console.WriteLine();


            Console.WriteLine("Any key to continue.");

            Console.ReadLine();
            Console.Clear();
            return game;

        }

       //Draw the hit history on the console
        private static void drawBoard(Game game)
        {
            Console.WriteLine();
            //Take the board of the player whose turn it is.
            Dictionary<Coordinate, ShotHistory> currentHistory = game.p2Board.ShotHistory;
            if (game.isPlayer1Turn)
            {
                currentHistory = game.p1Board.ShotHistory;
            }

            Console.WriteLine(" A  B  C  D  E  F  G  H  I  J ");
            for (int i = 1; i <= 10; i++)
            {
                int lineBreakCounter = 0;
                for (int j = 1; j <= 10; j++)
                {
                    Coordinate coord = new Coordinate(i, j);
                    if (!currentHistory.ContainsKey(coord))
                    {
                        Console.Write(" . ");
                        lineBreakCounter++;
                    }
                    else
                    {
                        switch (currentHistory[coord])
                        {
                            case ShotHistory.Hit:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" H ");
                                Console.ForegroundColor = ConsoleColor.White;
                                lineBreakCounter++;
                                break;
                            case ShotHistory.Miss:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write(" M ");
                                Console.ForegroundColor = ConsoleColor.White;
                                lineBreakCounter++;
                                break;
                            default:
                                Console.Write(" . ");
                                lineBreakCounter++;
                                break;
                        }
                    }

                    if (lineBreakCounter >= 10)
                    {
                        Console.WriteLine("  " + i);
                    }
                }
            }
        }

        private static void drawPlacementBoard(Game game, int counter)
        {
            Console.WriteLine();
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
            Console.WriteLine(" A  B  C  D  E  F  G  H  I  J ");
            for (int i = 1; i <= 10; i++)
            {
                int lineBreakCounter = 0;
                for (int j = 1; j <= 10; j++)
                {
                    Coordinate coord = new Coordinate(i, j);
                    if (shipPosition.Contains(coord))
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" S ");
                        Console.ForegroundColor = ConsoleColor.White;
                        lineBreakCounter++;
                    }
                    else
                    {
                        Console.Write(" . ");
                        lineBreakCounter++;
                    }

                    if (lineBreakCounter >= 10)
                    {
                        Console.WriteLine("  " + i);
                    }
                }
            }
        }

        private static Board getCurrentPlayer(Game game)
        {
            Board board = game.p1Board;
            if (!game.isPlayer1Turn)
                board = game.p2Board;
            return board;
        }

        private static void shipPlacement(Game game, bool isPlayer1)
        {
            drawBoard(game);

            Console.WriteLine();
            Console.Write("Place your ships, ");
            playerNameColor(game);
            Console.WriteLine();

            int counter = 1;
            foreach (ShipType ship in Enum.GetValues(typeof (ShipType)))
            {
                Board board = getCurrentPlayer(game);
                Console.WriteLine();
                Console.WriteLine("You will be placing a {0}", ship.ToString());
                Console.WriteLine("Enter your coordinates for this ship.");

                bool validPlacement = false;
                while (!validPlacement)
                {
                    Coordinate coord = inputToCoordinate();

                    Console.WriteLine("Now enter the direction to place the {0}. Values are D, U, L, R.", ship);

                    ShipDirection direction = directionInput();


                    PlaceShipRequest ShipRequest = new PlaceShipRequest(coord, direction, ship);
                    switch (board.PlaceShip(ShipRequest))
                    {
                        case ShipPlacement.NotEnoughSpace:
                            Console.WriteLine("Not enough space to place the ship. Please enter a new coordinate.");
                            break;
                        case ShipPlacement.Overlap:
                            Console.WriteLine(
                                "This placement would overlap another ship. Please enter a new coordinate.");
                            break;
                        case ShipPlacement.Ok:
                            validPlacement = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Console.Clear();
                drawPlacementBoard(game, counter);
                counter++;
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
                        result = ShipDirection.Left;
                        isDirectionValid = true;
                        break;
                    case "R":
                        result = ShipDirection.Right;
                        isDirectionValid = true;
                        break;
                    case "U":
                        result = ShipDirection.Up;
                        isDirectionValid = true;
                        break;
                    default:
                        Console.WriteLine("Input not valid. Please type a single character: U D L R");
                        break;
                }

            }
            return result;
        }

        //Takes in a console line, validates it, and if successful returns a coord obj.
        private static Coordinate inputToCoordinate()
        {
            string pattern1 = "^[A-Ja-j]{1}[1-9]$";

            string pattern2 = "^[A-Ja-j]{1}[1][0]$";

            Regex regex1 = new Regex(pattern1);
            Regex regex2 = new Regex(pattern2);

            Console.WriteLine();
            Console.WriteLine("Please enter a letter (A-J) followed by a number 1-10.");

            char[] validLetters1 = new[] {'X', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'};

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
                    Console.WriteLine("Input not valid.");
            }

            return new Coordinate(int.Parse(input.Substring(1)), letterAJ);
        }

        private static void playerNameColor(Game game)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (game.isPlayer1Turn)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(game.Player1Name);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static bool runOneTurn(Game game)
        {
            Console.Clear();
            
            Board board = getCurrentPlayer(game);

            drawBoard(game);

            Console.WriteLine();
            playerNameColor(game);
            Console.Write(" enter a coordinate for your shot.");

            bool validShot = false;

            while (!validShot)
            {
                Coordinate coord = inputToCoordinate();
                Console.Clear();

                FireShotResponse currentShot = board.FireShot(coord);

                drawBoard(game);

                switch (currentShot.ShotStatus)
                {
                    case ShotStatus.Invalid:
                        throw new Exception("Invalid should be handled by the coordinate input. FIX THIS");
                    case ShotStatus.Duplicate:
                        Console.WriteLine();
                        Console.WriteLine("You have already tried that coordinate!");
                        break;
                    case ShotStatus.Miss:
                        Console.WriteLine();
                        Console.WriteLine("You Missed!");
                        Console.ReadLine();
                        validShot = true;
                        break;
                    case ShotStatus.Hit:
                        Console.WriteLine();
                        Console.WriteLine("You hit a {0}!", currentShot.ShipImpacted);
                        Console.ReadLine();
                        validShot = true;
                        break;
                    case ShotStatus.HitAndSunk:
                        Console.WriteLine();
                        Console.WriteLine("You hit and sunk a {0}!", currentShot.ShipImpacted);
                        Console.ReadLine();
                        validShot = true;
                        break;
                    case ShotStatus.Victory:
                        Console.WriteLine();
                        Console.WriteLine("You hit and sunk a {0}! That was your opponent last ship.", currentShot.ShipImpacted);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Good job, ");
                        playerNameColor(game);
                        Console.Write("!");
                        Console.WriteLine();
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }
    }
}
