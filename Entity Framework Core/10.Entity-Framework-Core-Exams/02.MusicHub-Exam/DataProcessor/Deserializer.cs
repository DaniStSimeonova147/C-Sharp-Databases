namespace MusicHub.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

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
            var writerDtos = JsonConvert.DeserializeObject<ImportWriterDto[]>(jsonString);

            var validWriters = new List<Writer>();
            var result = new StringBuilder();

            foreach (var writerDto in writerDtos)
            {
                if (IsValid(writerDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var writer = Mapper.Map<Writer>(writerDto);

                    validWriters.Add(writer);

                    result.AppendLine(string.Format(SuccessfullyImportedWriter,
                                      writer.Name));
                }
            }

            context.Writers.AddRange(validWriters);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var producerDtos = JsonConvert.DeserializeObject<ImportProducerDto[]>(jsonString);

            var validProducers = new List<Producer>();
            var result = new StringBuilder();

            foreach (var producerDto in producerDtos)
            {
                if (IsValid(producerDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var producer = Mapper.Map<Producer>(producerDto);

                    var albumDtos = producerDto.AlbumDtos.Select(albumDto => Mapper.Map<Album>(albumDto)).ToList();
                    bool hasInvalidAlbum = albumDtos.Any(album => IsValid(album) == false);

                    if (hasInvalidAlbum == true)
                    {
                        result.AppendLine(ErrorMessage);
                    }
                    else
                    {
                        producer.Albums = albumDtos;

                        validProducers.Add(producer);

                        if (producer.PhoneNumber == null)
                        {
                            result.AppendLine(string.Format(SuccessfullyImportedProducerWithNoPhone,
                                              producer.Name,
                                              producer.Albums.Count));
                        }
                        else
                        {
                            result.AppendLine(string.Format(SuccessfullyImportedProducerWithPhone,
                                              producer.Name,
                                              producer.PhoneNumber,
                                              producer.Albums.Count));
                        }
                    }

                    //bool check = true;
                    //var producer = Mapper.Map<Producer>(producerDto);

                    //foreach (var albumDto in producerDto.AlbumDtos)
                    //{
                    //    if (IsValid(albumDto) == false)
                    //    {
                    //        result.AppendLine(ErrorMessage);
                    //        check = false;
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        var album = Mapper.Map<Album>(albumDto);

                    //        producer.Albums.Add(album);
                    //    }
                    //}
                    //if (check == true)
                    //{
                    //    validProducers.Add(producer);

                    //    if (producer.PhoneNumber == null)
                    //    {
                    //        result.AppendLine(string.Format(SuccessfullyImportedProducerWithNoPhone,
                    //                          producer.Name,
                    //                          producer.Albums.Count));
                    //    }
                    //    else
                    //    {
                    //        result.AppendLine(string.Format(SuccessfullyImportedProducerWithPhone,
                    //                          producer.Name,
                    //                          producer.PhoneNumber,
                    //                          producer.Albums.Count));
                    //    }
                    //}
                }
            }

            context.Producers.AddRange(validProducers);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSongDto[]),
                new XmlRootAttribute("Songs"));

            var songDtos = (ImportSongDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validSongs = new List<Song>();
            var result = new StringBuilder();

            foreach (var songDto in songDtos)
            {
                var isValidGenre = Enum.IsDefined(typeof(Genre), songDto.Genre);

                var writer = context.Writers.Find(songDto.WriterId);
                var album = context.Albums.Find(songDto.AlbumId);

                if (isValidGenre == false 
                    || writer == null
                    || album == null 
                    ||IsValid(songDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var song = Mapper.Map<Song>(songDto);

                    song.Writer = writer;
                    song.Album = album;

                    validSongs.Add(song);

                    result.AppendLine(string.Format(SuccessfullyImportedSong,
                                      song.Name,
                                      song.Genre,
                                      song.Duration));
                }
            }

            context.Songs.AddRange(validSongs);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPerformerDto[]),
                new XmlRootAttribute("Performers"));

            var performerDtos = (ImportPerformerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validPerformers = new List<Performer>();
            var result = new StringBuilder();

            foreach (var performerDto in performerDtos)
            {
                if (IsValid(performerDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var performer = Mapper.Map<Performer>(performerDto);

                    var songsIds = context.Songs.Select(s => s.Id).ToList();
                    var isSongExist = performerDto.PerformersSongs
                                      .Select(s => s.Id)
                                      .All(s => songsIds.Contains(s));

                    if (isSongExist == false)
                    {
                        result.AppendLine(ErrorMessage);
                    }
                    else
                    {
                        performer.PerformerSongs = performerDto.PerformersSongs
                            .Select(s => new SongPerformer
                            {
                                SongId = s.Id
                            })
                            .ToList();

                        validPerformers.Add(performer);

                        result.AppendLine(string.Format(SuccessfullyImportedPerformer,
                                          performer.FirstName,
                                          performer.PerformerSongs.Count));
                    }
                }
            }

            context.Performers.AddRange(validPerformers);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            return result;
        }
    }
}