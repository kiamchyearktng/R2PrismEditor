using System.Text;
using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;

namespace lib.remnant2.saves.Navigation;
public class Node
{
    private readonly ModelBase _object;
    public Node? Parent { get; set; }
    public List<Segment> Path { get; set; }

    private List<Node>? _children;
    private readonly object _lock = new ();
    private readonly Navigator _navigator;

    public List<Node> Children
    {
        get
        {
            if (_children == null)
            {
                lock (_lock)
                {
                    _children ??= GetChildren().ToList();
                }
            }
            return _children;
        }
    }

    public ModelBase Object => _object;

    public T GetObject<T>() where T : ModelBase
    {
        return (T)_object;
    }

    public string Name => Path[^1].Name;

    public Node(ModelBase obj, Navigator navigator)
    {
        _navigator = navigator;
        _object = obj;
        Path = [new() { Name = GetName(obj), Type = obj.GetType().Name }];
    }

    public Node(ModelBase obj, int? index, Node parent, Navigator navigator)
    {
        _navigator = navigator;
        _object = obj;
        Parent = parent;
        Path = [..parent.Path, new() { Name = GetName(obj), Type = obj.GetType().Name, Index = index }];
    }

    private static string GetName(ModelBase item)
    {
        return item switch
        {
            Variable x => x.Name.Name,
            Variables x => x.Name.Name,
            UObject x => GetUObjectName(x),
            Component x => x.ComponentKey,
            TextPropertyData0 x => x.Key,
            Property x => x.Name.Name,
            ObjectProperty x => x.ClassName,
            EnumProperty x => x.EnumType.Name,
            ByteProperty x => x.EnumName.Name,
            Actor x => x.DynamicData?.ClassPath.Name,
            ArrayStructProperty x=> x.ElementType.Name,
            _ => ""
        } ?? "";
    }

    private static string? GetUObjectName(UObject o)
    {
        if (o.Name != o.ObjectPath)
        {
            o.ToString();
        }

        
        if (o.Name == "PersistenceContainer")
        {
            return $"pc:{o.KeySelector}";
        }
        if (o.Name == "ZoneActor" && (o.Properties?.Contains("Label") ?? false))
        {
            return "za:" + o.Properties["Label"].Get<TextProperty>();
        }
        return o.Name;
    }

    private IEnumerable<Node> GetChildren()
    {
        foreach ((ModelBase obj, int? index) in _object.GetChildren())
        {
            yield return new(obj, index, this, _navigator);
        }
    }
    public string DisplayPath
    {
        get
        {
            StringBuilder sb = new();
            foreach (Segment segment in Path)
            {
                string name = string.IsNullOrEmpty(segment.Name) ? "" : $"({segment.Name})";
                string index = segment.Index == null ? "" : $"[{segment.Index}]";
                sb.Append($"{index}{segment.Type}{name}.");
            }
            return sb.ToString()[..(sb.Length - 1)];
        }
    }

    public override string ToString()
    {
        return $"[{_object.GetType().Name}]{_object}";
    }

    // The Nav* properties below make it easier to navigate in the debugger
    public List<Dictionary<string, Node>>? NavPersistenceContainerChildren
    {
        get
        {
            if (_object is not PersistenceContainer container)
            {
                return null;
            }

            return container.Actors.Select(
                    x => x.Value.Archive.Objects
                        .Select((input, index) => new { index, input })
                        .ToDictionary(o => o.index + "|" + (o.input.ObjectPath ?? ""), o => _navigator.Lookup(o.input)))
                .ToList();
        }
    }
    public List<Node>? NavPersistenceContainerFlattenedChildren
    {
        get
        {
            return NavPersistenceContainerChildren?.SelectMany(x => x.Values).ToList();
        }
    }

    public List<Dictionary<string, Node>>? NavObjectChildren
    {
        get
        {
            if (_object is not UObject o)
            {
                return null;
            }

            if (o.Properties is not { Properties.Count: > 1 })
            {
                return null;
            }

            KeyValuePair<string, Property> property = o.Properties.Properties[1];
            if (property.Key != "Blob")
            {
                return null;
            }

            if (property.Value.Value is not StructProperty s)
            {
                return null;
            }

            return s.Value is not PersistenceContainer p ? null : _navigator.Lookup(p).NavPersistenceContainerChildren;
        }
    }
    public List<Node>? NavObjectFlattenedChildren
    {
        get
        {
            return NavObjectChildren?.SelectMany(x => x.Values).ToList();
        }
    }

    public Node? NavActorZoneActorProperties
    {
        get
        {
            if (_object is not Actor a)
            {
                return null;
            }

            PropertyBag? result = a.GetZoneActorProperties();
            return result == null ? null : _navigator.Lookup(result);
        }
    }
    
    public Node? NavActorFirstObjectProperties
    {
        get
        {
            if (_object is not Actor a)
            {
                return null;
            }

            PropertyBag? result = a.GetFirstObjectProperties();
            return result == null ? null : _navigator.Lookup(result);
        }
    }
    public Node? NavObjectPropertyObject
    {
        get
        {
            if (_object is not ObjectProperty a)
            {
                return null;
            }

            return a.ObjectIndex == -1 ? null : _navigator.Lookup(a.Object!);
        }
    }
}
