namespace MusicHub
{
    using AutoMapper;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using System;
    using System.Globalization;

    public class MusicHubProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public MusicHubProfile()
        {
            this.CreateMap<ImportWriterDto, Writer>();

            this.CreateMap<ImportProducerDto, Producer>();
            this.CreateMap<ImportAlbumsProducerDto, Album>()
                .ForMember(x => x.ReleaseDate, y => y.MapFrom(
                    a => DateTime.ParseExact(a.ReleaseDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture)));

            this.CreateMap<ImportSongDto, Song>()
                .ForMember(x => x.Genre, y => y.MapFrom(
                    s => Enum.Parse(typeof(Genre), s.Genre)))
                .ForMember(x => x.Duration, y => y.MapFrom(
                    s => TimeSpan.ParseExact(s.Duration, @"c", CultureInfo.InvariantCulture)))
                //@"c" = @"hh\:mm\:ss"
                .ForMember(x => x.CreatedOn, y => y.MapFrom(
                    s => DateTime.ParseExact(s.CreatedOn, @"dd/MM/yyyy", CultureInfo.InvariantCulture)));

            this.CreateMap<ImportPerformerDto, Performer>();
            this.CreateMap<ImportPerformerSongsDto, Song>();
        }
    }
}
