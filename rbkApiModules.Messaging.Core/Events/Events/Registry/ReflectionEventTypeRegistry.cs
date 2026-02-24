using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace rbkApiModules.Commons.Core;

public sealed class ReflectionEventTypeRegistry : IEventTypeRegistry
{
    private readonly Dictionary<(string name, short version), Type> _map;

    public ReflectionEventTypeRegistry()
    {
        var assemblies = LoadAllAssemblies();

        _map = assemblies
            .SelectMany(x => SafeGetTypes(x))
            .Select(x => (t: x, attr: x.GetCustomAttribute<EventNameAttribute>()))
            .Where(x => x.attr is not null)
            .GroupBy(x => (x.attr!.Name, x.attr!.Version))
            .ToDictionary(x => x.Key, x => x.Select(y => y.t).First());
    }

    public bool TryResolve(string name, short version, out Type type) => _map.TryGetValue((name, version), out type!);

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch
        {
            return Array.Empty<Type>();
        }
    }

    private static IReadOnlyCollection<Assembly> LoadAllAssemblies()
    {
        // Start with all assemblies already loaded into the default context
        var known = new HashSet<string>(
            AssemblyLoadContext.Default.Assemblies.Select(x => x.FullName),
            StringComparer.OrdinalIgnoreCase);

        var set = AssemblyLoadContext.Default.Assemblies
            .Where(x => !x.IsDynamic && x.FullName is not null)
            .ToDictionary(x => x.FullName!, x => x, StringComparer.OrdinalIgnoreCase);

        // Walk the full reference graph from a stable root
        var root = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        AddGraph(set, root, known);

        // Include everything from DependencyContext when available
        var dc = DependencyContext.Default;
        if (dc is not null)
        {
            foreach (var x in dc.RuntimeLibraries)
            {
                foreach (var name in x.GetDefaultAssemblyNames(dc))
                {
                    if (known.Contains(name.FullName))
                    {
                        continue;
                    }
                    TryLoadByName(set, name, known);
                }
            }
        }

        // Scan base directory for assemblies not already loaded
        var baseDir = AppContext.BaseDirectory;
        if (Directory.Exists(baseDir))
        {
            foreach (var path in Directory.EnumerateFiles(baseDir, "*.dll"))
            {
                try
                {
                    var name = AssemblyName.GetAssemblyName(path);
                    if (known.Contains(name.FullName))
                    {
                        continue;
                    }
                    TryLoadByPath(set, path, name, known);
                }
                catch
                {
                    // Ignore invalid files or native libraries
                }
            }
        }

        // Exclude dynamic and resource satellite assemblies
        return set.Values
            .Where(x => !x.IsDynamic && !x.GetName().Name!.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private static void AddGraph(IDictionary<string, Assembly> set, Assembly root, ISet<string> known)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(known);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var stack = new Stack<Assembly>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.FullName is null || !set.TryAdd(current.FullName, current))
            {
                continue;
            }

            foreach (var x in current.GetReferencedAssemblies().Where(x => seen.Add(x.FullName)))
            {
                try
                {
                    var loaded = AssemblyLoadContext.Default.LoadFromAssemblyName(x);
                    if (loaded.FullName is not null && known.Add(loaded.FullName))
                    {
                        stack.Push(loaded);
                    }
                }
                catch
                {
                    // Ignore load failures to keep discovery resilient
                }
            }
        }
    }

    private static void TryLoadByName(IDictionary<string, Assembly> set, AssemblyName name, ISet<string> known)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(known);

        try
        {
            var asm = AssemblyLoadContext.Default.LoadFromAssemblyName(name);
            if (asm.FullName is not null && known.Add(asm.FullName))
            {
                set[asm.FullName] = asm;
            }
        }
        catch
        {
            // Ignore if not loadable
        }
    }

    private static void TryLoadByPath(IDictionary<string, Assembly> set, string path, AssemblyName name, ISet<string> known)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(known);

        try
        {
            // Guard against race with another load
            if (known.Contains(name.FullName))
            {
                return;
            }

            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            if (asm.FullName is not null && known.Add(asm.FullName))
            {
                set[asm.FullName] = asm;
            }
        }
        catch
        {
            // Ignore load failures or duplicates
        }
    }
}
