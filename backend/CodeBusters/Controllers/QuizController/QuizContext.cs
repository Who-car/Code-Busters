﻿namespace CodeBusters.Controllers.QuizController;

public class QuizContext
{
    public Guid LastSentQuizId { get; set; }
    public int? Count { get; set; }
}