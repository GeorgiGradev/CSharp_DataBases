namespace MusicHub.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using MusicHub.DataProcessor.ExportDtos;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var result = context
                .Albums
                .Where(album => album.Producer.Id == producerId)
                .Select(album => new
                {
                    AlbumName = album.Name,
                    ReleaseDate = album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = album.Producer.Name,
                    Songs = album.Songs.Select(song => new
                    {
                        SongName = song.Name,
                        Price = song.Price.ToString("f2"),
                        Writer = song.Writer.Name
                    })
                    .OrderByDescending(song => song.SongName)
                    .ThenBy(x => x.Writer)
                    .ToList(),
                    AlbumPrice = album.Songs.Sum(song => song.Price).ToString("f2")
                })
                .ToList()
                .OrderByDescending(album => album.AlbumPrice)
                .ToList();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return json;
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {

            var result = context
                .Songs
                .Where(song => song.Duration.TotalSeconds > duration)
                .Select(song => new ExportSongDto
                {
                    SongName = song.Name,
                    WriterName = song.Writer.Name,
                    PerformerName = song.SongPerformers.Select(perf=>perf.Performer.FirstName + " " + perf.Performer.LastName).FirstOrDefault(),
                    ProducerName = song.Album.Producer.Name,
                    Duration = song.Duration.ToString("c", CultureInfo.InvariantCulture)
                })
                .OrderBy(song=>song.SongName)
                .ThenBy(song=>song.WriterName)
                .ThenBy(song=>song.WriterName)
                .ToList();

            var xml = XMLConverter.Serialize(result, "Songs");

            return xml;
        }
    }
}