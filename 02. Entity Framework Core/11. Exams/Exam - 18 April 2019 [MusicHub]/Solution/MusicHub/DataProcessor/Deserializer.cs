namespace MusicHub.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter 
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone 
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong 
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var importWriterDtos = JsonConvert.DeserializeObject<List<ImportWriterDto>>(jsonString);
            var sb = new StringBuilder();
            var writersToAdd = new List<Writer>();

            foreach (var importWriterDto in importWriterDtos)
            {
                if (!IsValid(importWriterDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var writer = new Writer()
                {
                    Name = importWriterDto.Name,
                    Pseudonym = importWriterDto.Pseudonym
                };

                writersToAdd.Add(writer);
                sb.AppendLine(string.Format(SuccessfullyImportedWriter, writer.Name));
            }
            context.Writers.AddRange(writersToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var importProducerDtos = JsonConvert.DeserializeObject<List<ImportProducerDto>>(jsonString);
            var sb = new StringBuilder();
            var producersToAdd = new List<Producer>();

            foreach (var importProducerDto in importProducerDtos)
            {
                if (!IsValid(importProducerDto)
                    || !importProducerDto.Albums.All(IsValid)
                    || importProducerDto.Albums
                        .Any(x => DateTime.ParseExact(x.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) == null))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }

                var producer = new Producer()
                {
                    Name = importProducerDto.Name,
                    Pseudonym = importProducerDto.Pseudonym,
                    PhoneNumber = importProducerDto.PhoneNumber,
                    Albums = importProducerDto.Albums.Select(x => new Album
                    {
                        Name = x.Name,
                        ReleaseDate = DateTime.ParseExact(x.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
                    }).ToList()
                };

                producersToAdd.Add(producer);

                if (importProducerDto.PhoneNumber == null)
                {
                    sb.AppendLine(string.Format(SuccessfullyImportedProducerWithNoPhone, producer.Name, producer.Albums.Count()));
                }
                else
                {
                    sb.AppendLine(string.Format(SuccessfullyImportedProducerWithPhone, producer.Name, producer.PhoneNumber, producer.Albums.Count()));
                }    
            }

            context.Producers.AddRange(producersToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            var importSongDtos = XMLConverter.Deserializer<ImportSongDto>(xmlString, "Songs");
            var sb = new StringBuilder();
            var songsToAdd = new List<Song>();

            foreach (var importSongDto in importSongDtos)
            {
                TimeSpan duration = TimeSpan.ParseExact(importSongDto.Duration, "c", CultureInfo.InvariantCulture, TimeSpanStyles.None);

                DateTime releaseDate = DateTime.ParseExact(importSongDto.CreatedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

                Genre genre;
                bool isGenreValid = Enum.TryParse(importSongDto.Genre, out genre);

                var alnumId = context.Albums.SingleOrDefault(x => x.Id == importSongDto.AlbumId);
                var writerId = context.Writers.SingleOrDefault(x => x.Id == importSongDto.WriterId);


                if (!IsValid(importSongDto)
                    || duration == null
                    || releaseDate == null
                    || !isGenreValid
                    || alnumId == null
                    || writerId == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }



                var song = new Song()
                { 
                    Name = importSongDto.Name,
                    Duration = duration,
                    CreatedOn = releaseDate,
                    Genre = genre,
                    AlbumId = importSongDto.AlbumId.Value,
                    WriterId = importSongDto.WriterId,
                    Price = importSongDto.Price
                };

                songsToAdd.Add(song);
                sb.AppendLine(string.Format(SuccessfullyImportedSong, song.Name, song.Genre, song.Duration));
            }
            context.Songs.AddRange(songsToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            var importPerformerDtos = XMLConverter.Deserializer<ImportPerformerDto>(xmlString, "Performers");
            var sb = new StringBuilder();
            var performersToAdd = new List<Performer>();

            foreach (var importPerformerDto in importPerformerDtos)
            {

                if(!IsValid(importPerformerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var performer = new Performer()
                {
                    FirstName = importPerformerDto.FirstName,
                    LastName = importPerformerDto.LastName,
                    Age = importPerformerDto.Age,
                    NetWorth = importPerformerDto.NetWorth
                };


                bool isOk = true;
                foreach (var importSongPerformerDto in importPerformerDto.PerformersSongs)
                {
                    var isSongIdValid = context.Songs.Any(x => x.Id == importSongPerformerDto.SongId);

                    if (!IsValid(importSongPerformerDto)
                        || !isSongIdValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        isOk = false;
                        break;
                    }


                    var songPerformer = new SongPerformer()
                    {
                        SongId = importSongPerformerDto.SongId,
                        Performer = performer
                    };

                    performer.PerformerSongs.Add(songPerformer);

                }

                if (isOk == false)
                {
                   // sb.AppendLine(ErrorMessage);
                    continue;
                }

                performersToAdd.Add(performer);
                sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performer.FirstName, performer.PerformerSongs.Count));
            }

            context.Performers.AddRange(performersToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }


    }
}