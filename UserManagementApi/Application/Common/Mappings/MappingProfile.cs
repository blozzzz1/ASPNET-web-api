using AutoMapper;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, AuthUserDto>();
        CreateMap<CreateUserDto, User>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.PasswordHash, opt => opt.Ignore())
            .ForMember(d => d.Role, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.Orders, opt => opt.Ignore());
        CreateMap<UpdateUserDto, User>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.PasswordHash, opt => opt.Ignore())
            .ForMember(d => d.Role, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.Orders, opt => opt.Ignore());

        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.OrderItems, opt => opt.Ignore());
        CreateMap<UpdateProductDto, Product>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.OrderItems, opt => opt.Ignore());

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.LineTotal, opt => opt.MapFrom(s => s.Quantity * s.UnitPrice));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.UserFullName,
                opt => opt.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"));
    }
}
