using Newtonsoft.Json;
using PetClinic.Data;
using PetClinic.DataProcessor.Dto.Export;
using PetClinic.XMLHelper;
using System;
using System.Globalization;
using System.Linq;

namespace PetClinic.DataProcessor
{
    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var result = context
                .Animals
                .Where(animal => animal.Passport.OwnerPhoneNumber == phoneNumber)
                .Select(animal => new
                {
                    OwnerName = animal.Passport.OwnerName,
                    AnimalName = animal.Name,
                    Age = animal.Age,
                    SerialNumber = animal.Passport.SerialNumber,
                    RegisteredOn = animal.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                })
                .OrderBy(animal => animal.Age)
                .ThenBy(animal => animal.SerialNumber)
                .ToList();


            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return json;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var result = context
                .Procedures
                 .OrderBy(procedure=>procedure.DateTime)
                .Select(procedure => new ExportProcedureDto
                {
                    PassportNumber = procedure.Animal.PassportSerialNumber,
                    OwnerNumber = procedure.Animal.Passport.OwnerPhoneNumber,
                    ProcedureDate = procedure.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = procedure.ProcedureAnimalAids.Select(x => new ExportAninalAidDto
                    {
                        AidName = x.AnimalAid.Name,
                        AidPrice = x.AnimalAid.Price
                    }).ToList(),
                    TotalPrice = procedure.ProcedureAnimalAids.Sum(z=>z.AnimalAid.Price)
                })
               // .ToList()

                .OrderBy(y => y.PassportNumber)
                .ToList();


            var xml = XMLConverter.Serialize(result, "Procedures");

            return xml;
        }
    }
}