using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.BLL.GameLogic;

namespace BattleShip.UI
{
    public class Game
    {
        public string Player1Name { get; private set; }
        public string Player2Name { get; private set; }

        public Board p1Board = new Board();
        public Board p2Board = new Board();

        public bool isPlayer1Turn { get; private set; }

        public Game(string p1name, string p2name)
        {
            Player1Name = p1name;
            Player2Name = p2name;
            isPlayer1Turn = true;
        }

        public void TurnToggle()
        {
            isPlayer1Turn = !isPlayer1Turn;
        }
        

    }

}