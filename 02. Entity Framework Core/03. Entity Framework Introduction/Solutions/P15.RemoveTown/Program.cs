using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var context = new SoftUniContext();

            var result = RemoveTown(context);
            Console.WriteLine(result);
        }

        //15. Remove Town

        public static string RemoveTown(SoftUniContext context)
        {
            var townNameToDelete = "Seattle";

            var townToDelete = context.Towns
                .Where(t => t.Name == townNameToDelete)
                .FirstOrDefault();

            var targetingAddresses = context.Addresses
                .Where(a => a.Town.Name == townNameToDelete)
                .ToList();

            var employeesLivingOnTargetingAddresses = context.Employees
                .Where(e => targetingAddresses.Contains(e.Address))
                .ToList();

            employeesLivingOnTargetingAddresses.ForEach(e => e.Address = null);
            targetingAddresses.ForEach(a => context.Remove(a));
            context.Remove(townToDelete);

            context.SaveChanges();

            return $"{targetingAddresses.Count} addresses in Seattle were deleted";

        }
    }
}
