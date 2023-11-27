using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CodeBusters.Models;
using FluentValidation;

namespace CodeBusters.Utils;
public class UserValidator : AbstractValidator<User>
{
    private UserValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty()
            .WithMessage("Field 'Name' is required");
        RuleFor(m => m.Email)
            .NotEmpty()
            .WithMessage("Field 'Email' is required");
        RuleFor(m => m.Password)
            .NotEmpty()
            .WithMessage("Field 'Password' is required");
        
        RuleFor(m => m.Email)
            .EmailAddress()
            .WithMessage("Invalid email address");
        RuleFor(m => m.Password)
            .Must(s => Regex.IsMatch(s, "^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-_]).{8,}$"))
            .WithMessage("password must be at least 8 characters length, " +
                         "contain at least 1 latin symbol, " + 
                         "1 number, " + 
                         "1 #?!@$%^&*-_ symbol");
    }
    public static FluentValidation.Results.ValidationResult TryValidate(User user)
    {
        var validator = new UserValidator();
        return validator.Validate(user);
    }
}