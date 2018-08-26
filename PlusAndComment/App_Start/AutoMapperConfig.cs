using AutoMapper;
using PlusAndComment.Models.Entities;
using PlusAndComment.Models.ViewModel;
using PlusAndComment.Models.ViewModel.Entities;
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
                cfg.CreateMap<ProductVM, ProductEntity>().MaxDepth(3);
                cfg.CreateMap<ProductEntity, ProductVM>().MaxDepth(3);

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
                cfg.CreateMap<CategoryAttributeVM, AddProductAttributeVM>()
                .ForMember(x => x.AllAttributeTypes, opts => opts.Ignore())
                .ForMember(x => x.AllCategories, opts => opts.Ignore());

                cfg.CreateMap<AddProductAttributeVM, CategoryAttributeVM>();

                //ProductAttributeVM
                cfg.CreateMap<CategoryAttributeVM, CategoryAttributesEntity>();
                cfg.CreateMap<CategoryAttributesEntity, CategoryAttributeVM>();

                //PictureVM
                cfg.CreateMap<PictureVM, PictureEntity>();
                cfg.CreateMap<PictureEntity, PictureVM>();

                //AttributeValueList
                cfg.CreateMap<AttributeValueListVM, AttributeValueListEntity>();
                cfg.CreateMap<AttributeValueListEntity, AttributeValueListVM>();

                //ProductAttribute
                cfg.CreateMap<ProductAttributeVM, ProductAttributeEntity>().MaxDepth(3); ;
                cfg.CreateMap<ProductAttributeEntity, ProductAttributeVM>().MaxDepth(3); ;

                //ProductAttribute to AddProducTAttribute
                cfg.CreateMap<ProductAttributeVM, AddProductAttributeVM>().MaxDepth(3); ;
                cfg.CreateMap<AddProductAttributeVM, ProductAttributeVM>().MaxDepth(3); ;

            });

            //config.AssertConfigurationIsValid();
            //Mapper.Initialize(config);
            //var source = new HomeVM();
            //var dest = mapper.Map<HomeVM, ICollection<PostEntity>>(source);

        }
    }
}