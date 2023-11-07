using System.ComponentModel.DataAnnotations;
using CodeBusters.Models;

namespace CodeBusters.Utils;

public class QuizValidationAttribute : Attribute
{
    protected ValidationResult IsValid(object? value, ValidationContext context)
    {
        var errors = new List<string>();
        if (value is not Quiz quiz) 
            return new ValidationResult("error while transferring data");
        
        if (quiz.Questions is null || quiz.Questions.Count == 0) 
            return new ValidationResult("quiz must contain at least 1 question");
        
        if (quiz.Questions.Any(x => x.Answers.Count == 0))
            errors.Add("questions must contain at least 1 answer");
        
        if (quiz.Questions.Any(x => !x.Answers.Any(a => a.IsCorrect)))
            errors.Add("question must contain at least 1 correct answer");

        if (errors.Any())
            return new ValidationResult(string.Join(';', errors));
        
        return ValidationResult.Success;
    }
}