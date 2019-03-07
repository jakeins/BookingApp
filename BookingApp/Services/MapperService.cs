using AutoMapper;
using BookingApp.Services.Interfaces;
using System;

namespace BookingApp.Services
{
    public class MapperService : IMapperService
    {
        readonly IMapper autoMapperInstance;

        public MapperService()
        {
            autoMapperInstance = new Mapper(new MapperConfiguration(cfg => {
            }));
        }

        public object Map(object source, Type sourceType, Type destinationType)
        {
            return autoMapperInstance.Map(source, sourceType, destinationType);
        }

        public TDestination Map<TSource, TDestination>(TSource source) => (TDestination)Map(source, typeof(TSource), typeof(TDestination));

        public TDestination Map<TDestination>(object source) => (TDestination)Map(source, source.GetType(), typeof(TDestination));
    }
}