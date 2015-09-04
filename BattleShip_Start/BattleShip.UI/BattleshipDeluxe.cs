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
    public class BattleshipDeluxe
    {

        public static void Main(string[] args)
        {
            //Splash screen
            Game.SpectacularIntro();

            bool loop = true;
            //Game starts.
            while (loop)
            {
                //Input names.
                Game game = new Game();
                game.Setup();

                //Place ships.
                for (int i = 0; i < 2; i++)
                {
                    game.DeployShips();
                    game.TurnToggle();
                    Console.Clear();
                }

                Console.Write("Ship placement done. ");
                game.PlayerNameColor();
                Console.Write(" will now begin his turn.");
                Console.WriteLine();

                //Start looping turns.
                bool isGameOver = false;
                while (!isGameOver)
                {
                    isGameOver = game.RunOneTurn();
                    game.TurnToggle();
                }
                Console.ReadLine();

                //gg.
                Console.WriteLine("Would you like to play again? Type \"yes\" to play again. Any other input to quit");

                if (Console.ReadLine().ToLower() != "yes")
                {
                    loop = false;
                }
                Console.Clear();

            }

        }
    }
}




