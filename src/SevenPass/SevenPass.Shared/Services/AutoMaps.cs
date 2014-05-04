using System;
using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.Mappers;
using SevenPass.Services.Cache;
using SevenPass.Services.Databases;
using SevenPass.ViewModels;

namespace SevenPass.Services
{
    internal static class AutoMaps
    {
        /// <summary>
        /// Initializes the mapping engine.
        /// </summary>
        /// <returns></returns>
        public static IMappingEngine Initialize()
        {
            // Initialize
            PlatformAdapter
                .Resolve<IPlatformSpecificMapperRegistry>()
                .Initialize();

            var config = new ConfigurationStore(
                new TypeMapFactory(), MapperRegistry.Mappers);

            // Maps
            config.CreateMap<DatabaseRegistration, DatabaseItemViewModel>();

            // Done
            return new MappingEngine(config);
        }
    }
}