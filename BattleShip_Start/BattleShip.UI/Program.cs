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

namespace BattleShip.UI
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //Game game = setup();
            while (true)
            {
                Console.WriteLine(inputToCoordinate().ToString());
            }
            
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

        static void shipPlacement(Game game)
        {
            drawBoard(game);

            Console.WriteLine("{0}, place your ships.", game.Player1Name);
            

        }

        //Takes in a console line, validates it, and if successful returns a coord obj.
        static Coordinate inputToCoordinate()
        {
            string pattern = "^[A-Ja-j]{1}[1-9]{1}0?$";
            Regex regex = new Regex(pattern);
            
            Console.WriteLine("Please enter a letter (A-J) followed by a number 1-10.");

            char[] validLetters1 = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

            bool validInput = false;
            string input = "";
            int letterAJ = 0;

            while (!validInput)
            {

                input = Console.ReadLine();
                input = input.ToUpper();

                if (regex.IsMatch(input))
                    validInput = true;
                
                if (!validInput)
                    Console.WriteLine("not valid");
            }
            
            return new Coordinate(letterAJ, int.Parse(input.Substring(1)));
            
        }

        
    }
}
