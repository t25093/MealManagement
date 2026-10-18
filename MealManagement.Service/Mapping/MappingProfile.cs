using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;

namespace MealManagement.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===== משתמשות =====
            CreateMap<User, UserDto>();

            // ===== מאגר המאכלים =====
            CreateMap<Dish, DishDto>();
            CreateMap<CreateDishDto, Dish>(MemberList.Source);

            // ===== הקצאות =====
            CreateMap<UserDishAssignment, AssignmentDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName));

            // ===== מנה בארוחה (המשאב המוגבל) =====
            CreateMap<MealDish, MealDishDto>()
                .ForMember(d => d.DishName, o => o.MapFrom(s =>
                    s.Dish != null ? s.Dish.Name : "טרם נבחר"))
                .ForMember(d => d.Unit, o => o.MapFrom(s =>
                    s.Dish != null ? s.Dish.Unit : (MeasureUnit?)null))
                .ForMember(d => d.RequiredQuantity, o => o.MapFrom(s =>
                    s.DishId != null ? s.RequiredQuantity : (double?)null))
                .ForMember(d => d.RemainingQuantity, o => o.MapFrom(s =>
                    s.DishId != null ? s.RequiredQuantity - s.AssignedQuantity : (double?)null))
                .ForMember(d => d.IsFullyAssigned, o => o.MapFrom(s =>
                    s.DishId != null && s.AssignedQuantity >= s.RequiredQuantity));

            // ===== ארוחות =====
            CreateMap<Meal, MealSummaryDto>()
                .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.Owner.FullName))
                .ForMember(d => d.CoveragePercent, o => o.Ignore());

            CreateMap<Meal, MealDetailsDto>()
                .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.Owner.FullName))
                .ForMember(d => d.MenuItems, o => o.MapFrom(s => s.MealDishes))
                .ForMember(d => d.PendingEmails, o => o.MapFrom(s => s.Invitations.Select(i => i.Email)))
                .ForMember(d => d.IsOwner, o => o.Ignore());

            CreateMap<CreateMealDto, Meal>(MemberList.Source)
              .ForMember(d => d.MealDishes, o => o.Ignore())
              .ForSourceMember(s => s.MenuItems, o => o.DoNotValidate());

            // ===== צ'אט =====
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(d => d.SenderName, o => o.MapFrom(s => s.Sender.FullName));
            CreateMap<SendMessageDto, ChatMessage>(MemberList.Source);

            // ===== מתכונים =====
            CreateMap<Recipe, RecipeDto>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.FullName))
                .ForMember(d => d.DishName, o => o.MapFrom(s =>
                    s.Dish != null ? s.Dish.Name : null));
            CreateMap<CreateRecipeDto, Recipe>(MemberList.Source);
        }
    }
}