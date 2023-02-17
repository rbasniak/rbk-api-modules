//using rbkApiModules.Commons.Core;
//using rbkApiModules.Commons.Core.CQRS;

//namespace rbkApiModules.Commons.Relational.CQRS;

//public class SimpleCqrsBehaviorOptions
//{
//    private readonly Dictionary<Type, Type> _configuration;
//    private Type _defaultProvider;
//    internal Dictionary<Type, Func<IServiceProvider, BaseEntity[]>> _initializationFunctions;

//    public SimpleCqrsBehaviorOptions()
//    {
//        _configuration = new Dictionary<Type, Type>();
//        _initializationFunctions = new Dictionary<Type, Func<IServiceProvider, BaseEntity[]>>();
//    }

//    public SimpleCqrsBehaviorOptions SetupType(Type entityType, Type storeProviderType, Func<IServiceProvider, BaseEntity[]> initialization = null)
//    {
//        _configuration.TryAdd(entityType, storeProviderType);

//        if (initialization != null)
//        {
//            _initializationFunctions.Add(entityType, initialization);
//        }

//        return this;
//    }

//    public SimpleCqrsBehaviorOptions UseDefaultProvider(Type storeProviderType)
//    {
//        _defaultProvider = storeProviderType;

//        return this;
//    }

//    public SimpleCqrsBehaviorOptions ForStore<TEntity>(Func<IServiceProvider, BaseEntity[]> initialization = null)
//    {
//        SetupType(typeof(TEntity), typeof(TProvider), initialization);

//        return this;
//    } 

//    public Type GetProvider(Type type)
//    {
//        Type provider;

//        if (_configuration.TryGetValue(type, out provider))
//        {
//            return provider;
//        }
//        else
//        {
//            return _defaultProvider != null ? _defaultProvider : null;
//        }
//    } 
//}