using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.BLL.Ships;

namespace BattleShip.BLL.Requests
{
    public class PlaceShipRequest
    {
        public PlaceShipRequest(Coordinate coord, ShipDirection direction, ShipType type)
        {
            Coordinate = coord;
            Direction = direction;
            ShipType = type;
        }

        
        public Coordinate Coordinate { get; set; }
        public ShipDirection Direction { get; set; }
        public ShipType ShipType { get; set; }
    }
}
