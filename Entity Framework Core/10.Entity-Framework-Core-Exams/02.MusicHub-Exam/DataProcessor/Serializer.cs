namespace MusicHub.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using MusicHub.DataProcessor.ExportDtos;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .OrderByDescending(a => a.Songs.Sum(s => s.Price))
                .Select(a => new ExportAlbumDto
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString(@"MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs
                    .Select(s => new ExportSongDto
                    {
                        SongName = s.Name,
                        Price = $"{s.Price:f2}",
                        Writer = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.Writer)
                    .ToList(),
                    AlbumPrice = $"{a.Songs.Sum(s => s.Price):f2}"
                })
                .ToList();


            var json = JsonConvert.SerializeObject(albums, Formatting.Indented);

            return json;
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var xmlSerializer = new XmlSerializer(typeof(ExportSongsAboveDurationDto[]),
                new XmlRootAttribute("Songs"));

            var songs = context.Songs
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new ExportSongsAboveDurationDto
                {
                    SongName = s.Name,
                    Writer = s.Writer.Name,
                    Performer = s.SongPerformers
                    .Select(p => p.Performer.FirstName + " " + p.Performer.LastName)
                    .FirstOrDefault(),
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.Writer)
                .ThenBy(s => s.Performer)
                .ToArray();

            var result = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(result), songs, namespaces);

            return result.ToString().TrimEnd();
        }
    }
}