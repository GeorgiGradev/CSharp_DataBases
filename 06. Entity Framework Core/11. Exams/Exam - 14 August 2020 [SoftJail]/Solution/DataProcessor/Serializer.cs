namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using SoftJail.XMLHelper;
    using System;
    using System.Globalization;
    using System.Linq;

    public class Serializer
    {

        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {

            var result = context
                    .Prisoners
                    .ToList()
                    .Where(x => ids.Contains(x.Id))
                    .Select(x => new
                    {
                        Id = x.Id,
                        Name = x.FullName,
                        CellNumber = x.Cell.CellNumber,
                        Officers = x.PrisonerOfficers.Select(y => new
                        {
                            OfficerName = y.Officer.FullName,
                            Department = y.Officer.Department.Name
                        })
                            .OrderBy(y => y.OfficerName)
                            .ToList(),
                        TotalOfficerSalary = x.PrisonerOfficers.Sum(z =>
                        z.Officer.Salary)
                    })
                    .ToList()
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
                    .ToList();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string jsonOutputData)
        {
            var prisonersList = jsonOutputData
                .Split(',')
                .ToList();

            var result = context
                .Prisoners
                .ToList()
                .Where(p => prisonersList.Contains(p.FullName))
                .Select(p => new ExportPrisonerDto()
                {
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Id = p.Id,
                    EncryptedMessages = p.Mails
                        .ToArray()
                        .Select(m => new ExportPrisonerMessageDto()
                        {
                            Description = ReverseString(m.Description)
                        })
                        .ToArray()
                })
                .ToList()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();



            string xml = XMLConverter.Serialize(result, "Prisoners");

            return xml;
        }

        private static string ReverseString(string s)
        {
            var charArray = s.ToCharArray();

            Array.Reverse(charArray);

            return new string(charArray);
        }
    }
}