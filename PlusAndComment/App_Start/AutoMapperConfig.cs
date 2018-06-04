using AutoMapper;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
using System.Collections;
using System.Collections.Generic;

namespace PlusAndComment.App_Start
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            ConfigurePostMapping();
        }

        private static void ConfigurePostMapping()
        {
            Mapper.Initialize(cfg =>
            {
                //Category
                cfg.CreateMap<CategoryVM, CategoryEntity>()
                .MaxDepth(3);

                cfg.CreateMap<CategoryEntity, CategoryVM>()
                .MaxDepth(3);

                //Product
                cfg.CreateMap<ProductVM, ProductEntity>();
                cfg.CreateMap<ProductEntity, ProductVM>();

                //Cart
                cfg.CreateMap<CartVM, CartEntity>();
                cfg.CreateMap<CartEntity, CartVM>();

                //Order
                cfg.CreateMap<OrderVM, OrderEntity>();
                cfg.CreateMap<OrderEntity, OrderVM>();

                //Order
                cfg.CreateMap<CompanyInformationVM, CompanyInformationEntity>();
                cfg.CreateMap<CompanyInformationEntity, CompanyInformationVM>();

                //ProductAttributes
                cfg.CreateMap<ProductAttributeVM, AddProductAttributeVM>()
                .ForMember(x => x.AllAttributeTypes, opts => opts.Ignore())
                .ForMember(x => x.AllCategories, opts => opts.Ignore());

                cfg.CreateMap<AddProductAttributeVM, ProductAttributeVM>();

                //ProductAttributeVM
                cfg.CreateMap<ProductAttributeVM, ProductAttriutesEntity>();
                cfg.CreateMap<ProductAttriutesEntity, ProductAttributeVM>();

            });

            //config.AssertConfigurationIsValid();
            //Mapper.Initialize(config);
            //var source = new HomeVM();
            //var dest = mapper.Map<HomeVM, ICollection<PostEntity>>(source);

        }
    }
}