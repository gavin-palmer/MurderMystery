using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery;
using MurderMystery.Generators;

namespace MysteryGame
{
    class Program
    {
        static Random rng = new Random();

        static void Main(string[] args)
        {
            var mystery = MysteryGenerator.GenerateMystery();
            mystery.PrintSummary();
            mystery.PrintNPCStatements();
        }
    }








}
