﻿namespace MyAspHelper.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RouteAttribute : Attribute
{
    public string Path { get; }

    public RouteAttribute(string path)
    {
        Path = path;
    }
}