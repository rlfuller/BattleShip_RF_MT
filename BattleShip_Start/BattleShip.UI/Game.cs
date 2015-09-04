using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    public class Game
    {
        private string player1Name;
        private string player2Name;

        private readonly Board p1Board = new Board();
        private readonly Board p2Board = new Board();

        private bool isPlayer1Turn;

        public Game()
        {
            isPlayer1Turn = true;
        }

        //

        //Switches turn between the two players.
        public void TurnToggle()
        {
            isPlayer1Turn = !isPlayer1Turn;
        }

        //Displays a glorious splash page
        public static void SpectacularIntro()
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(@"                             .  o ..                  ");
            Console.WriteLine(@"                             o . o o.o                ");
            Console.WriteLine(@"                                  ...oo               ");
            Console.WriteLine(@"                                    __[]__            ");
            Console.WriteLine(@"                                 __|_o_o_o\__         ");
            Console.WriteLine(@"                                 \**********/         ");
            Console.WriteLine(@"                                  \. ..  . /          ");
            Console.WriteLine(@"                             ^^^^^^^^^^^^^^^^^^^^     ");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(@"                          WELCOME TO BATTLESHIP DELUXE");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"                                    press START button");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ReadLine();

            Console.Clear();
        }

        //Inits the game object
        public void Setup()
        {
            Console.WriteLine(" - Battleship Deluxe -");
            Console.WriteLine("2 players required. Starting order will be randomized.");
            Console.WriteLine("Please enter the first name.");

            string name1 = Console.ReadLine();

            Console.WriteLine("Please enter the second name.");

            string name2 = Console.ReadLine();

            Random random = new Random();
            if (random.Next(2) == 1)
            {
                var temp = name1;
                name1 = name2;
                name2 = temp;
            }

            player1Name = name1;
            player2Name = name2;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(name1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" will be Player 1. ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(name2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" will be Player 2.");
            Console.WriteLine();


            Console.WriteLine("Any key to continue.");

            Console.ReadLine();
            Console.Clear();

        }

        //Starts the ship placement phase
        public void DeployShips()
        {
            //This counter follows the amount of ships.
            //0 is used in order to display the grid before the ship array in the Board obj is initialized.
            //1 means the player is placing the first ship, and has none on the board.

            int counter = 0;
            drawPlacementBoard(counter);
            counter++;

            Console.WriteLine();
            Console.Write("Place your ships, ");
            PlayerNameColor();
            Console.WriteLine();

            foreach (ShipType ship in Enum.GetValues(typeof(ShipType)))
            {
                Board board = getCurrentPlayer(true);
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
                            Console.WriteLine("Not enough space to place the ship in that direction. Try elsewhere.");
                            break;
                        case ShipPlacement.Overlap:
                            Console.WriteLine("This placement would overlap another ship. Please enter a new coordinate.");
                            break;
                        case ShipPlacement.Ok:
                            validPlacement = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Console.Clear();
                drawPlacementBoard(counter);
                counter++;
            }
        }

        //Runs one turn and checks the result of each shot. Returns true if gg.
        public bool RunOneTurn()
        {
            Console.Clear();

            Board board = getCurrentPlayer();

            drawBoard();

            Console.WriteLine();
            PlayerNameColor();
            Console.Write(" enter a coordinate for your shot.");

            bool validShot = false;

            while (!validShot)
            {
                Coordinate coord = inputToCoordinate();
                Console.Clear();

                FireShotResponse currentShot = board.FireShot(coord);

                drawBoard();

                switch (currentShot.ShotStatus)
                {
                    case ShotStatus.Invalid:
                        throw new Exception("Invalid should be handled by the coordinate input. FIX THIS");
                    case ShotStatus.Duplicate:
                        Console.WriteLine();
                        Console.WriteLine("You have already tried that coordinate!");
                        Console.Beep(1000, 400);
                        break;
                    case ShotStatus.Miss:
                        Console.WriteLine();
                        Console.WriteLine("You Missed!");
                        Console.Beep(2000, 200);
                        Console.ReadLine();
                        validShot = true;
                        break;
                    case ShotStatus.Hit:
                        Console.WriteLine();
                        Console.WriteLine("You hit a something!");
                        Console.Beep(10000, 300);
                        Console.ReadLine();
                        validShot = true;
                        break;
                    case ShotStatus.HitAndSunk:
                        Console.WriteLine();
                        Console.WriteLine("You hit and sunk a {0}!", currentShot.ShipImpacted);
                        Console.Beep(10000, 200);
                        Console.Beep(10000, 200);
                        Console.Beep(10000, 200);
                        Console.ReadLine();
                        validShot = true;
                        break;
                    case ShotStatus.Victory:
                        Console.WriteLine();
                        Console.Beep(10000, 300);
                        Console.Beep(1000, 500);
                        Console.Beep(2000, 300);
                        Console.Beep(5000, 1200);
                        Console.Beep(2000, 300);
                        Console.Beep(11000, 100);
                        Console.Beep(7000, 300);
                        Console.Beep(2000, 300);
                        Console.Beep(10000, 300);
                        Console.WriteLine("You hit and sunk a {0}! That was your opponent's last ship.", currentShot.ShipImpacted);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Good job, ");
                        PlayerNameColor();
                        Console.Write("!");
                        Console.WriteLine();
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }
        
        //

        //Draw the hit history on the console
        private void drawBoard()
        {
            Console.WriteLine();

            Dictionary<Coordinate, ShotHistory> currentHistory1 = p2Board.ShotHistory;
            Dictionary<Coordinate, ShotHistory> currentHistory2 = p1Board.ShotHistory;

            Console.WriteLine(@"        " + "{0,-30}{1,31}", player2Name + "'s ships", player1Name + "'s ships");
            Console.WriteLine(@"        A  B  C  D  E  F  G  H  I  J  +  A  B  C  D  E  F  G  H  I  J ");
            for (int i = 1; i <= 10; i++)
            {
                //Starts looking at p1's history.
                Dictionary<Coordinate, ShotHistory> currentHistory = currentHistory1;

                //Halfpage counter checks what board has been printed. Each unit corresponds to 1 coord.
                //1-10 is the first board.
                //11-20 is the second board.
                //i represents columns
                //j represents rows (j resets halfwaythrough to allow the second set of coords to be drawn)
                //Halfpage counter resets on every row.
                int halfPageCounter = 0;
                for (int j = 1; j <= 10; j++)
                {

                    Coordinate coord = new Coordinate(i, j);

                    if (!currentHistory.ContainsKey(coord))
                    {
                        //Battleship spacer is a simple method used to add some space,
                        //in order to center the grid.
                        //Resizing the console will mess with this but it's not something
                        //we can prevent AFAIK.
                        if (j == 1 && halfPageCounter == 0)
                            BattleShipSpacer(" . ");
                        else
                            Console.Write(" . ");
                        halfPageCounter++;
                    }
                    else
                    {
                        switch (currentHistory[coord])
                        {
                            case ShotHistory.Hit:
                                Console.ForegroundColor = ConsoleColor.Red;
                                if (j == 1 && halfPageCounter == 0)
                                    BattleShipSpacer(" H ");
                                else
                                    Console.Write(" H ");
                                Console.ForegroundColor = ConsoleColor.White;
                                halfPageCounter++;
                                break;
                            case ShotHistory.Miss:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                if (j == 1 && halfPageCounter == 0)
                                    BattleShipSpacer(" M ");
                                else
                                    Console.Write(" M ");
                                Console.ForegroundColor = ConsoleColor.White;
                                halfPageCounter++;
                                break;
                            default:
                                if (j == 1 && halfPageCounter == 0)
                                    BattleShipSpacer(" . ");
                                else
                                    Console.Write(" . ");
                                halfPageCounter++;
                                break;
                        }
                    }

                    if (halfPageCounter == 10)
                    {
                        //If the row is 10, adjust the spacing so that the extra character
                        //in 10 does not skew the whole line.
                        if (i == 10)
                            Console.Write(" " + i + "");
                        else
                            Console.Write(" " + i + " ");
                        currentHistory = currentHistory2;
                        j = 0;
                    }
                    if (halfPageCounter == 20)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }

        //Draw a placement grid. Used only when placing ships (only one grid is needed)
        //Uses special rules to display ships from the Board object (active player only).
        private void drawPlacementBoard(int counter)
        {
            Console.WriteLine();
            //Take the board of the player whose turn it is.
            Board currentBoard = p2Board;
            if (isPlayer1Turn)
            {
                currentBoard = p1Board;
            }
            //new dictionary that holds ships positions
            List<Coordinate> shipPosition = new List<Coordinate>();

            //Only look for ships if there is any on the board!
            if (counter > 0)
            {
                for (int i = 0; i < counter; i++)
                {
                    Coordinate[] ship = currentBoard.findShip(i);

                    //get all the coordinates for each ship that has been placed
                    foreach (Coordinate element in ship)
                    {
                        shipPosition.Add(element);
                    }
                }
            }

            //Print the name of the player who is placing ships at the top of the grid
            Console.Write(@"          ");
            PlayerNameColor();
            Console.WriteLine();

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

        //Returns the board of the current player. Add opt. parameter as 'true' if used during placement,
        //since the boards are swapped.
        private Board getCurrentPlayer(bool placement = false)
        {
            if (!placement)
            {
                Board board = p2Board;
                if (!isPlayer1Turn)
                    board = p1Board;
                return board;
            }
            else
            {
                Board board = p2Board;
                if (isPlayer1Turn)
                    board = p1Board;
                return board;
            }
            
        }

        //Validates ship direction from player
        private ShipDirection directionInput()
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
        private Coordinate inputToCoordinate()
        {
            string pattern = "^[A-Ja-j]{1}([1-9]{1}|10)$";
            
            Regex regex = new Regex(pattern);

            Console.WriteLine();
            Console.WriteLine("Please enter a letter (A-J) followed by a number 1-10.");

            char[] validLetters1 = new[] { 'X', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

            bool validInput = false;
            string input = "";
            int letterAJ = 0;

            while (!validInput)
            {
                input = Console.ReadLine();
                input = input.ToUpper();

                if (regex.IsMatch(input))
                {
                    validInput = true;
                    for (int i = 1; i < validLetters1.Length; i++)
                    {
                        if (input[0] == validLetters1[i])
                        {
                            letterAJ = i;
                            break;
                        }
                    }
                }

                else
                    Console.WriteLine("Input not valid.");
            }

            return new Coordinate(int.Parse(input.Substring(1)), letterAJ);
        }

        //Prints the name of the player whose turn it is, in the correct color
        public void PlayerNameColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            string playerName = player2Name;
            if (isPlayer1Turn)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                playerName = player1Name;
            }
            Console.Write(playerName);
            Console.ForegroundColor = ConsoleColor.White;
        }

        //Minor utility used for adding padding here and there
        private void BattleShipSpacer(string x)
        {
            string Span = @"       ";
            Console.Write(Span + x);
        }
    }
}
