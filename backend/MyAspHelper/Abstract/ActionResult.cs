namespace MyAspHelper.Abstract;

public class ActionResult
{
    public bool IsSuccess { get; set; }
    public List<string> ErrorInfo { get; set; } = new();

    public ActionResult(bool isSuccess, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        if (errors != null) ErrorInfo.AddRange(errors);
    }
    
    public ActionResult(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        ErrorInfo.Add(error);
    }
}