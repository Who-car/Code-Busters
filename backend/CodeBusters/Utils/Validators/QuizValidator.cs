using System.ComponentModel.DataAnnotations;
using CodeBusters.Models;
using FluentValidation;

namespace CodeBusters.Utils;

public class QuizValidator : AbstractValidator<Quiz>
{
    private QuizValidator()
    {
        RuleFor(m => m.Questions.Count)
            .NotEmpty()
            .WithMessage("Quiz must contain at least one question");
    }
    public static FluentValidation.Results.ValidationResult TryValidate(Quiz quiz)
    {
        var validator = new QuizValidator();
        return validator.Validate(quiz);
    }
}