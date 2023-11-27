namespace VaporStore
{
	using AutoMapper;
    using System;
    using System.Globalization;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enum;
    using VaporStore.DataProcessor.ImportDtos;

    public class VaporStoreProfile : Profile
	{
		// Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
		public VaporStoreProfile()
		{
            this.CreateMap<ImportUserDto, User>();
            this.CreateMap<ImportCardDto, Card>()
                .ForMember(x => x.Cvc, y => y.MapFrom(
                           c => c.CVC))
                .ForMember(x => x.Type, y => y.MapFrom(
                           c => Enum.Parse(typeof(CardType), c.Type)));
		}
	}
}