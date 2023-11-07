using System.ComponentModel.DataAnnotations;
using CodeBusters.Models;

namespace CodeBusters.Utils;

public static class CustomValidator
{
    public static (bool isSuccess, ValidationResult[] results) Validate<T>(T obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj);

        return (Validator.TryValidateObject(obj, context, results, false), results.ToArray());
    }
}