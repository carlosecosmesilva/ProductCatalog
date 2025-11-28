using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Mappers;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using Xunit;

namespace ProductCatalog.Tests.Fixtures
{
    public class MapperFixture
    {
        #region Properties
        public IMapper Mapper { get; private set; }
        #endregion

        #region Constructor
        public MapperFixture()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductProfile());
            });

            Mapper = config.CreateMapper();
        }
        #endregion
    }
}