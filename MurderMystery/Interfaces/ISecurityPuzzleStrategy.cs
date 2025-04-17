using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurderMystery.Models;

namespace MurderMystery.Interfaces
{
    public interface ISecurityPuzzleStrategy
    {
        string Name { get; }
        SecurityInfo GeneratePuzzle(Random random, TimelineContext context);
    }
}
