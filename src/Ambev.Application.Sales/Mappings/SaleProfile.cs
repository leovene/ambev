using Ambev.Application.Sales.Commands;
using Ambev.Application.Sales.DTOs;
using Ambev.Domain.Sales.Entities;
using AutoMapper;

namespace Ambev.Application.Sales.Mappings
{
    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            CreateMap<SaleRequestDto, CreateSaleCommand>();
            CreateMap<SaleItemRequestDto, CreateSaleItemCommand>();

            CreateMap<SaleRequestDto, UpdateSaleCommand>()
                .ForMember(dest => dest.Branch, opt => opt.Ignore());
            CreateMap<SaleItemRequestDto, UpdateSaleItemCommand>();

            CreateMap<SaleEntity, SaleResponseDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<SaleItemEntity, SaleItemResponseDto>();

            CreateMap<CreateSaleDocumentCommand, SaleDocumentEntity>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreateSaleItemDocumentCommand, SaleItemDocumentEntity>();

            CreateMap<SaleDocumentEntity, SaleResponseDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<SaleItemDocumentEntity, SaleItemResponseDto>();

            CreateMap<SaleResponseDto, CreateSaleDocumentCommand>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<SaleItemResponseDto, CreateSaleItemDocumentCommand>();
        }
    }
}
