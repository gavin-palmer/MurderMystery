using System;
using System.Collections.Generic;
using System.Text;
using MurderMystery.Models;

namespace MurderMystery.Helpers
{
    public static class ProofHelper
    {
        public static bool RoomHasSecurityCameraFootage(string room)
        {
            var _random = new Random();
            
            //cameras only work some of the time 
            if ((room == "Kitchen" || room == "Conservatory" || room == "Ballroom" || room == "Wine Cellar") && _random.NextDouble() < 0.3)
            {
                return true;
            }
            return false;
        }

        public static bool IsOutside(string room)
        {
            var _random = new Random();

            if (room == "Garden" && _random.NextDouble() < 0.5)
            {
                return true;
            }
            return false;
        }

        public static bool LeftPhysicalEvidence(Models.Action action)
        {
            var _random = new Random();

            return action.PhysicalObject != PhysicalObject.None && _random.NextDouble() < 0.6;
        }
    }
}
