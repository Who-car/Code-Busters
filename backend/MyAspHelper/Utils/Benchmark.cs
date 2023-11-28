using System.Diagnostics;

namespace MyAspHelper.Utils;

public static class Benchmark
{
    public static void TestTask(int repetitions, Action taskToRun)
    {
        var watch = new Stopwatch();
        watch.Start();
        for (var i = 0; i < repetitions; i++)
        {
            taskToRun.Invoke();
        }
        watch.Stop();
        
        Console.WriteLine($"Total time: {watch.ElapsedMilliseconds}ms\nAverage time: {(double)watch.ElapsedMilliseconds/repetitions}ms");
    }
}