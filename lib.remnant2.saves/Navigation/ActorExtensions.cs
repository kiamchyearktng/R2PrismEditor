using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;

namespace lib.remnant2.saves.Navigation;
public static class ActorExtensions
{
    public static PropertyBag? GetZoneActorProperties(this Actor actor)
    {
        UObject? zoneActorObject = actor.Archive.Objects.FirstOrDefault(x => x.Name == "ZoneActor");
        if (zoneActorObject != null)
        {
            return zoneActorObject.Properties;
        }
        return null;
    }
    public static PropertyBag? GetFirstObjectProperties(this Actor actor)
    {
        UObject? firstObject = actor.Archive.Objects.FirstOrDefault();
        if (firstObject != null)
        {
            return firstObject.Properties;
        }
        return null;
    }
}

