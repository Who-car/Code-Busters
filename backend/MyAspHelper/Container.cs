namespace MyAspHelper;

public class Container
{
    private readonly Dictionary<Type, List<Type>> RegisteredObjects = new();
    
    public void Register<TInterface, TImplementation>() where TImplementation : TInterface
    {
        if (!RegisteredObjects.ContainsKey(typeof(TInterface)))
            RegisteredObjects.Add(typeof(TInterface), new List<Type>());
        
        RegisteredObjects[typeof(TInterface)].Add(typeof(TImplementation));
    }

    public TInterface Resolve<TInterface>()
    {
        if (!RegisteredObjects.ContainsKey(typeof(TInterface)))
            throw new InvalidOperationException($"No registration found for type: {typeof(TInterface).Name}");

        //Если к одному интерфейсу зарегестрировано несколько зависимостей, но требуется лишь разрешение одной,
        //по умолчанию возвращается последняя заргестрированная зависимость
        var implementationType = RegisteredObjects[typeof(TInterface)].Last();
        if (Activator.CreateInstance(implementationType) is not TInterface instance)
            throw new NullReferenceException();
        
        return instance;
    }

    public List<TInterface> ResolveAll<TInterface>()
    {
        if (!RegisteredObjects.ContainsKey(typeof(TInterface)))
            throw new InvalidOperationException($"No registration found for type: {typeof(TInterface).Name}");
        
        var implementationsType = RegisteredObjects[typeof(TInterface)];
        var instances = new List<TInterface>();
        
        foreach (var type in implementationsType)
        {
            if (Activator.CreateInstance(type) is not TInterface instance)
                throw new NullReferenceException();
            instances.Add(instance);
        }
        
        return instances;
    }
}