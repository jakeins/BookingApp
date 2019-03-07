using System;

namespace BookingApp.Services.Interfaces
{
    public interface IMapperService
    {
        object Map(object source, Type sourceType, Type destinationType);
        TDestination Map<TSource, TDestination>(TSource source);
        TDestination Map<TDestination>(object source);
    }
}