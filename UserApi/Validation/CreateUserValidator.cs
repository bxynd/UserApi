using System.Text.RegularExpressions;
using FluentValidation;
using UserApi.Entities;

namespace UserApi.Validation;

public class CreateUserValidator: AbstractValidator<CreateDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Login).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().Matches(new Regex("^[a-zA-Z0-9]*$")).MinimumLength(4);
        RuleFor(x => x.Password).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().Matches(new Regex("^[a-zA-Z0-9]*$")).MinimumLength(8);
        RuleFor(x => x.Name).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().Matches(new Regex("^[а-яА-ЯёЁ_a-zA-Z ]*"));
        RuleFor(x => x.Gender).InclusiveBetween(0,2);

    }
}