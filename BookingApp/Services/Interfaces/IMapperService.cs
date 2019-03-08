using System;

namespace BookingApp.Services.Interfaces
{
    public interface IMapperService
    {
        /// <summary>
        /// Execute a mapping from the source object to a new destination object. The source type is inferred from the source object.
        /// </summary>
        TDestination Map<TDestination>(object source);

        /// <summary>
        /// Execute a mapping from the source object to a new destination object.
        /// </summary>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object.
        /// </summary>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        /// <summary>
        /// Execute a mapping from the source object to a new destination object with explicit System.Type objects
        /// </summary>
        object Map(object source, Type sourceType, Type destinationType);

        /// <summary>
        /// Execute a mapping from the source object to existing destination object with explicit System.Type objects
        /// </summary>
        object Map(object source, object destination, Type sourceType, Type destinationType);
    }
}