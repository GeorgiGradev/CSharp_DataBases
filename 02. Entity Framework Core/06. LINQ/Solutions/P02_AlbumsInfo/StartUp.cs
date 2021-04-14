namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //02.
            int producerId = 7;
            Console.WriteLine(ExportAlbumsInfo(context, producerId));


        }
        //02. All Albums Produced By Given Producer
        #region 
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var result = context
                .Producers
                .FirstOrDefault(x => x.Id == producerId)
                .Albums
                .Select(x => new
                {
                    AlbumName = x.Name,
                    ReleaseDate = x.ReleaseDate,
                    ProducerName = x.Producer.Name,
                    AlbumSong = x.Songs.Select(y=> new
                    { 
                        SongName = y.Name,
                        Price = y.Price,
                        Writer = y.Writer.Name
                    })
                    .OrderByDescending(y=>y.SongName)
                    .ThenBy(y=>y.Writer)
                    .ToList(),
                    AlbumPrice = x.Price
                })
                .OrderByDescending(x=>x.AlbumPrice)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in result)
            {
                int counter = 0;
                sb.AppendLine($"-AlbumName: {item.AlbumName}")
                .AppendLine($"-ReleaseDate: {item.ReleaseDate.ToString("MM/dd/yyyy")}")
                .AppendLine($"-ProducerName: {item.ProducerName}")
                .AppendLine($"-Songs:");
                foreach (var song in item.AlbumSong)
                {
                    counter++;
                    sb.AppendLine($"---#{counter}")
                    .AppendLine($"---SongName: {song.SongName}")
                    .AppendLine($"---Price: {song.Price:f2}")
                    .AppendLine($"---Writer: {song.Writer}");
                }
                sb.AppendLine($"-AlbumPrice: {item.AlbumPrice:f2}");
            }


            return sb.ToString().TrimEnd();
        }
        #endregion  

        //03.Songs Above Given Duration
        #region
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
