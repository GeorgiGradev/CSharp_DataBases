using P01_StudentSystem.Data;
using System;

namespace P01_StudentSystem
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var db = new StudentSystemContext();
            db.Database.EnsureDeleted();
            Console.WriteLine("deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("created");
            Console.WriteLine("done");
        }
    }
}
