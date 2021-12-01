using System;

namespace FordFalkerson
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph FiveVertex = new Graph();
            FiveVertex.AddLink(1, 2, 20, 0);
            FiveVertex.AddLink(1, 3, 30, 0);
            FiveVertex.AddLink(1, 4, 10, 0);
            FiveVertex.AddLink(2, 3, 40, 0);
            FiveVertex.AddLink(2, 5, 30, 0);
            FiveVertex.AddLink(3, 4, 10, 0);
            FiveVertex.AddLink(4, 5, 20, 0);
            FiveVertex.AddLink(3, 5, 20, 0);
            Console.WriteLine(FiveVertex.FordFalkesrson(1, 5));
        }
    }
}
