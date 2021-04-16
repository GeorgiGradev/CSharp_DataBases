using Newtonsoft.Json;
using PetClinic.Data;
using PetClinic.DataProcessor.Dto.Import;
using PetClinic.Models;
using PetClinic.XMLHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PetClinic.DataProcessor
{
    public class Deserializer
    {
        private const string SuccessMessage = "Record {0} successfully imported.";
        private const string SuccessMessageAnimal = "Record {0} Passport №: {1} successfully imported.";
        private const string SuccessMessageProcedure = "Record successfully imported.";
        private const string ErrorMessage = "Error: Invalid data.";



        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var importAnimalAidDtos = JsonConvert.DeserializeObject<List<ImportAnimalAidDto>>(jsonString);
            var animalAidsToAdd = new List<AnimalAid>();

            foreach (var importAnimalAidDto in importAnimalAidDtos)
            {
                if (!IsValid(importAnimalAidDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (animalAidsToAdd.Any(x=>x.Name == importAnimalAidDto.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var animalAid = new AnimalAid()
                { 
                 Name = importAnimalAidDto.Name,
                 Price = importAnimalAidDto.Price
                };

                animalAidsToAdd.Add(animalAid);
                sb.AppendLine(string.Format(SuccessMessage, importAnimalAidDto.Name));
            }

            context.AnimalAids.AddRange(animalAidsToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {

            var deserializedAnimals = JsonConvert.DeserializeObject<ImportAnimalDto[]>(jsonString);

            var sb = new StringBuilder();

            var validAnimals = new List<Animal>();
            var passports = new HashSet<Passport>();
            foreach (var dto in deserializedAnimals)
            {
                DateTime regDate;
                var isRegDateValid = DateTime.TryParse(dto.Passport.RegistrationDate, out regDate);

                if (!IsValid(dto) || !IsValid(dto.Passport) ||
                    validAnimals.Any(x => x.PassportSerialNumber == dto.Passport.SerialNumber))
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var validPasport = new Passport
                {
                    SerialNumber = dto.Passport.SerialNumber,
                    OwnerName = dto.Passport.OwnerName,
                    OwnerPhoneNumber = dto.Passport.OwnerPhoneNumber,
                    RegistrationDate = regDate
                };

                passports.Add(validPasport);

                var animal = new Animal
                {
                    Name = dto.Name,
                    Type = dto.Type,
                    Age = dto.Age,
                    PassportSerialNumber = dto.Passport.SerialNumber,
                    Passport = validPasport
                };

                validAnimals.Add(animal);

                sb.AppendLine($"Record {animal.Name} Passport №: {animal.PassportSerialNumber} successfully imported.");
            }

            context.Animals.AddRange(validAnimals);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var importVetDtos = XMLConverter.Deserializer<ImportVetDto>(xmlString, "Vets");
            var vetsToAdd = new List<Vet>();


            foreach (var importVetDto in importVetDtos)
            {
                if (!IsValid(importVetDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (vetsToAdd.Any(x=>x.PhoneNumber == importVetDto.PhoneNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var vet = new Vet()
                {
                    Name = importVetDto.Name,
                    Age = importVetDto.Age,
                    PhoneNumber = importVetDto.PhoneNumber,
                    Profession = importVetDto.Profession
                };

                vetsToAdd.Add(vet);
                sb.AppendLine(string.Format(SuccessMessage, vet.Name));

            }

            context.Vets.AddRange(vetsToAdd);
            context.SaveChanges();
            
            return sb.ToString().TrimEnd();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var importProcedureDtos = XMLConverter.Deserializer<ImportProcedureDto>(xmlString, "Procedures");
            var proceduresToAdd = new List<Procedure>();
            int counter = 0;
            foreach (var procedureDto in importProcedureDtos)
            {
                if (!IsValid(procedureDto))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var vet = context.Vets.FirstOrDefault(v => v.Name == procedureDto.VetName);

                var animal = context.Animals.FirstOrDefault(a => a.PassportSerialNumber == procedureDto.AnimalSerialNumber);

                DateTime dateTime;

                var isDateTimeValid = DateTime.TryParseExact(procedureDto.DateTime, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

                if (vet == null || animal == null | !isDateTimeValid)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var procedure = new Procedure()
                {
                    Animal = animal,
                    Vet = vet,
                    DateTime = dateTime,
                };

                foreach (var animalAidDto in procedureDto.AnimalAids)
                {
                    if (!IsValid(animalAidDto))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }

                    var animalAid = context.AnimalAids.FirstOrDefault(aa => aa.Name == animalAidDto.Name);

                    var isAnimalAidExisting = procedure.ProcedureAnimalAids.Any(p => p.AnimalAid.Name == animalAid.Name);

                    if (animalAid == null || isAnimalAidExisting)
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }

                    var procedureAnimalAid = new ProcedureAnimalAid()
                    {
                        Procedure = procedure,
                        AnimalAid = animalAid
                    };

                    procedure.ProcedureAnimalAids.Add(procedureAnimalAid);
                }

                proceduresToAdd.Add(procedure);

                sb.AppendLine(string.Format(SuccessMessageProcedure));
            }

            context.Procedures.AddRange(proceduresToAdd);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}