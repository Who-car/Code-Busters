using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CodeBusters.Models;

namespace CodeBusters.Utils;

public class UserValidationAttribute : ValidationAttribute
{
    private static readonly Regex Email = new Regex(
        @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\." +
        "[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*" +
        @"@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)" +
        "+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");

    private static readonly Regex Password = new Regex(
        "^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-_]).{8,}$");

    protected override ValidationResult IsValid(object? value, ValidationContext context)
    {
        var errors = new List<string>();
        if (value is not User user) return new ValidationResult("error while transferring data");
        
        if (string.IsNullOrEmpty(user.Name)) errors.Add("user name can not be null");
        
        if (string.IsNullOrEmpty(user.Email)) errors.Add("user email can not be null");
        
        if (string.IsNullOrEmpty(user.Password)) errors.Add("user password can not be null");

        if (!Email.IsMatch(user.Email)) errors.Add("invalid email");
        
        if (!Password.IsMatch(user.Password)) errors.Add("password must be at least 8 characters length, " +
                                                         "contain at least 1 latin symbol, " +
                                                         "1 number, " +
                                                         "1 #?!@$%^&*-_ symbol");

        if (errors.Any())
            return new ValidationResult(string.Join(';', errors));
        
        return ValidationResult.Success;
    }
}