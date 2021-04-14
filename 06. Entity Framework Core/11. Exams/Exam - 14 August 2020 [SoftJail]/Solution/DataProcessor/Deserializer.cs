namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using SoftJail.XMLHelper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        const string ErrorMessage = "Invalid Data";
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var departmentDtos = JsonConvert.DeserializeObject<List<DepartmentImportDto>>(jsonString);
            var departmentsToAdd = new List<Department>();

            foreach (var departmentDto in departmentDtos)
            {
                bool IsValidCell = true;

                if (!IsValid(departmentDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var department = new Department()
                {
                    Name = departmentDto.Name
                };

                foreach (var cellDto in departmentDto.Cells)
                {

                    if (!IsValid(cellDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        IsValidCell = false;
                        break;
                    }

                    var cell = new Cell()
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    };

                    department.Cells.Add(cell); 
                }

                if (IsValidCell == false)
                {
                    continue;
                }

                if (department.Cells.Count == 0)
                {
                    continue;
                }


                departmentsToAdd.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(departmentsToAdd);
            context.SaveChanges();


            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {

            var sb = new StringBuilder();
            var prisonerDtos = JsonConvert.DeserializeObject<List<PrisonerImportDto>>(jsonString);
            var prisonersToAdd = new List<Prisoner>();

            foreach (var prisonerDto in prisonerDtos)
            {
                var isValid = IsValid(prisonerDto) && prisonerDto.Mails.All(IsValid);

                if (isValid)
                {
                    var prisoner = new Prisoner()
                    {
                        FullName = prisonerDto.FullName,
                        Nickname = prisonerDto.Nickname,
                        Age = prisonerDto.Age,
                        IncarcerationDate 
                                    = DateTime.ParseExact(
                                    prisonerDto.IncarcerationDate,
                                    "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture
                                    ),
                        ReleaseDate 
                                    = prisonerDto.ReleaseDate == null ? 
                                       new DateTime?()
                                       : DateTime.ParseExact(
                                               prisonerDto.ReleaseDate,
                                               "dd/MM/yyyy",
                                               CultureInfo.InvariantCulture
                                               ),
                        Bail = prisonerDto.Bail,
                        CellId = prisonerDto.CellId,
                        Mails = prisonerDto.Mails
                                    .Select(x => new Mail
                                    {
                                        Description = x.Description,
                                        Sender = x.Sender,
                                        Address = x.Address
                                    })
                                    .ToList()
                    };
                    prisonersToAdd.Add(prisoner);
                    sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
            }

            context.Prisoners.AddRange(prisonersToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();



            //foreach (var prisonerDto in prisonerDtos)
            //{
            //    bool addressIsValid = true;

            //    if (!IsValid(prisonerDto))
            //    {
            //        sb.AppendLine(ErrorMessage);
            //        continue;
            //    }

            //    DateTime incarcerationDate;
            //    bool isIncarcerationDateValid = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy",
            //        CultureInfo.InvariantCulture, DateTimeStyles.None, out incarcerationDate);

            //    if (!isIncarcerationDateValid)
            //    {
            //        sb.AppendLine(ErrorMessage);
            //        continue;
            //    }

            //    DateTime? releaseDate = null;
            //    if (!String.IsNullOrEmpty(prisonerDto.ReleaseDate))
            //    {
            //        DateTime releaseDateValue;
            //        bool isReleaseDateValid = DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy",
            //            CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDateValue);

            //        if (!isReleaseDateValid)
            //        {
            //            sb.AppendLine(ErrorMessage);
            //            continue;
            //        }

            //        releaseDate = releaseDateValue;
            //    }


            //    var prisoner = new Prisoner()
            //    {
            //        FullName = prisonerDto.FullName,
            //        Nickname = prisonerDto.Nickname,
            //        Age = prisonerDto.Age,
            //        IncarcerationDate = incarcerationDate,
            //        ReleaseDate = releaseDate,
            //        Bail = prisonerDto.Bail,
            //        CellId = prisonerDto.CellId,
            //    };

            //    foreach (var mailDto in prisonerDto.Mails)
            //    {
            //        if (!IsValid(mailDto))
            //        {
            //            sb.AppendLine(ErrorMessage);
            //            addressIsValid = false;
            //            continue;
            //        }

            //        var mail = new Mail()
            //        { 
            //            Description = mailDto.Description,
            //            Sender = mailDto.Sender,
            //            Address = mailDto.Address
            //        };

            //        prisoner.Mails.Add(mail);
            //    }

            //    if (addressIsValid == false)
            //    {
            //        continue;
            //    }

            //    prisonersToAdd.Add(prisoner);
            //    sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            //}

            //context.AddRange(prisonersToAdd);
            //context.SaveChanges();

            //    return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var officerImportDtos = XMLConverter.Deserializer<OfficerImportDto>(xmlString, "Officers");

            var officersToAdd = new List<Officer>();

            foreach (var officerImportDto in officerImportDtos)
            {
                var isValid = IsValid(officerImportDto) && officerImportDto.Prisoners.All(IsValid);

                Position position;
                var isPositionValid = Enum.TryParse(officerImportDto.Position, out position);
                if (!isPositionValid)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                Weapon weapon;

                var isWeaponValid = Enum.TryParse(officerImportDto.Weapon, out weapon);

                if (!isWeaponValid)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }


                if (isValid)
                {
                    var officer = new Officer()
                    {
                        FullName = officerImportDto.FullName,
                        Salary = officerImportDto.Salary,
                        Position = position,
                        Weapon = weapon,
                        DepartmentId = officerImportDto.DepartmentId
                    };

                    foreach (var prisonerDto in officerImportDto.Prisoners)
                    {
                        var prisoner = new OfficerPrisoner()
                        {
                            Officer = officer,
                            PrisonerId = prisonerDto.Id
                        };

                        officer.OfficerPrisoners.Add(prisoner);
                    }

                    officersToAdd.Add(officer);
                    sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
            }

            context.Officers.AddRange(officersToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}