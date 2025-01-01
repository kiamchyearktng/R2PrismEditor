using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;
using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Navigation;

public class Navigator
{
    private readonly ItemRegistry _registry = new();
    private readonly Dictionary<ModelBase, Node> _lookup = [];
    private readonly Node _root;
    public Navigator(SaveFile saveFile)
    {
        _root = new Node(saveFile.SaveData, this);
        Queue<Node> q = new();
        q.Enqueue(_root);
        while (q.Count > 0)
        {
            Node n = q.Dequeue();
            _lookup.Add(n.Object, n);
            string type = n.Path[^1].Type;
            switch (type)
            {
                case "Property":
                case "Variable":
                case "Actor":
                case "UObject":
                case "Component":
                case "ArrayStructProperty":
                    _registry.Add(n);
                    break;
            }

            foreach (Node c in n.Children)
            {
                q.Enqueue(c);
            }
        }
    }

    public Node Root => _root;

    public Node Lookup(ModelBase o)
    {
        return _lookup[o];
    }

    private IEnumerable<T> Filter<T>(List<T> items, ModelBase? parent) where T : ModelBase
    {
        if (parent == null) return items;

        static bool IsParent(Span<Segment> obj, Span<Segment> filter)
        {
            if (filter.Length > obj.Length) return false;
            for (int i = 0; i < filter.Length; i++)
            {
                if (obj[i] != filter[i]) return false;
            }

            return true;
        }

        return items.Where(x => IsParent(CollectionsMarshal.AsSpan(_lookup[x].Path), CollectionsMarshal.AsSpan(_lookup[parent].Path)));
    }

    private IEnumerable<T> GetRegistryItems<T>(string name, ModelBase? parent = null) where T : ModelBase
    {
        return Filter(_registry.Get<T>(name), parent);
    }

    private IEnumerable<T> FindRegistryItems<T>(string namePattern, ModelBase? parent = null) where T : ModelBase
    {
        return Filter(_registry.Find<T>(namePattern), parent);
    }

    private T? GetRegistryItem<T>(string name, ModelBase? parent = null) where T : ModelBase
    {
        var l = GetRegistryItems<T>(name, parent).ToList();
        if (l.Count == 0)
        {
            return null;
        }

        if (l.Count == 1)
        {
            return l.ElementAt(0);
        }

        throw new InvalidOperationException("there are more than one item");
    }
    public List<T> GetPropertiesValues<T>(string name, ModelBase? parent = null)
    {
        IEnumerable<Property> list = Filter(_registry.Get<Property>(name), parent);
        return list.Select(x => (T)x.Value!).ToList();
    }

    public List<Property> GetProperties(string name, ModelBase? parent = null)
    {
        return GetRegistryItems<Property>(name, parent).ToList();
    }

    public List<Property> FindProperties(string namePattern, ModelBase? parent = null)
    {
        return FindRegistryItems<Property>(namePattern, parent).ToList();
    }

    public List<Property> GetAllProperties()
    {
        return _registry.GetAll<Property>();
    }

    public Property? GetProperty(string name, ModelBase? parent = null)
    {
        return GetRegistryItem<Property>(name, parent);
    }

    public List<T> GetVariablesValues<T>(string name, ModelBase? parent = null)
    {
        IEnumerable<Variable> list = Filter(_registry.Get<Variable>(name), parent);
        return list.Select(x => (T)x.Value!).ToList();
    }

    public List<Variable> GetVariables(string name, ModelBase? parent = null)
    {
        return GetRegistryItems<Variable>(name, parent).ToList();
    }

    public List<Variable> FindVariables(string namePattern, ModelBase? parent = null)
    {
        return FindRegistryItems<Variable>(namePattern, parent).ToList();
    }

    public List<Variable> GetAllVariables()
    {
        return _registry.GetAll<Variable>();
    }

    public Variable? GetVariable(string name, ModelBase? parent = null)
    {
        return GetRegistryItem<Variable>(name, parent);
    }

    public List<Actor> GetActors(string name, ModelBase? parent = null)
    {
        return GetRegistryItems<Actor>(name, parent).ToList();
    }

    public List<Actor> FindActors(string namePattern, ModelBase? parent = null)
    {
        return FindRegistryItems<Actor>(namePattern, parent).ToList();
    }

    public List<Actor> GetAllActors()
    {
        return _registry.GetAll<Actor>();
    }

    public Actor? GetActor(string name, ModelBase? parent = null)
    {
        return GetRegistryItem<Actor>(name, parent);
    }

    public List<UObject> GetObjects(string name, ModelBase? parent = null)
    {
        return GetRegistryItems<UObject>(name, parent).ToList();
    }

    public List<UObject> FindObjects(string namePattern, ModelBase? parent = null)
    {
        return FindRegistryItems<UObject>(namePattern, parent).ToList();
    }

    public List<UObject> GetAllObjects()
    {
        return _registry.GetAll<UObject>();
    }

    public UObject? GetObject(string name, ModelBase? parent = null)
    {
        return GetRegistryItem<UObject>(name, parent);
    }

    public List<Component> GetComponents(string name, ModelBase? parent = null)
    {
        return GetRegistryItems<Component>(name, parent).ToList();
    }

    public List<Component> FindComponents(string namePattern, ModelBase? parent = null)
    {
        return FindRegistryItems<Component>(namePattern, parent).ToList();
    }

    public List<Component> GetAllComponents()
    {
        return _registry.GetAll<Component>();
    }

    public Component? GetComponent(string name, ModelBase? parent = null)
    {
        return GetRegistryItem<Component>(name, parent);
    }

    public List<ArrayStructProperty> GetArrayStructProperties(string name, ModelBase? parent = null)
    {
        return GetRegistryItems<ArrayStructProperty>(name, parent).ToList();
    }

    public List<ArrayStructProperty> FindArrayStructProperties(string namePattern, ModelBase? parent = null)
    {
        return FindRegistryItems<ArrayStructProperty>(namePattern, parent).ToList();
    }

    public List<ArrayStructProperty> GetAllArrayStructProperties()
    {
        return _registry.GetAll<ArrayStructProperty>();
    }

    public ArrayStructProperty? GetArrayStructProperty(string name, ModelBase? parent = null)
    {
        return GetRegistryItem<ArrayStructProperty>(name, parent);
    }
    
    public Dictionary<string, List<string>> GetSearchableNames()
    {
        return _registry.GetNames();
    }
}
