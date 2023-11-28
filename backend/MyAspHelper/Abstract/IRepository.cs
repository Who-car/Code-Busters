namespace MyAspHelper.Abstract;

public interface IRepository
{
    public static abstract void ConfigureDb(string connectionString);
}