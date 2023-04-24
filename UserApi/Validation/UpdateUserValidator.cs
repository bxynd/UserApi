using System.Text.RegularExpressions;
using FluentValidation;
using UserApi.Entities;

namespace UserApi.Validation;

public class UpdateUserValidator: AbstractValidator<UpdateDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().Matches(new Regex("^[а-яА-ЯёЁ_a-zA-Z ]*")); 
        RuleFor(x => x.Gender).InclusiveBetween(0,2);
    }
}