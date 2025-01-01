using lib.remnant2.saves.Model.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prismeditor.definitions
{
    public class PrismData
    {
        public int charIndex;
        public int prismIndex;
        public PropertyBag properties;

        private List<Segment> segmentList = [];
        private List<Fragment> fragmentList = [];

        public PrismData(PropertyBag properties)
        {
            this.properties = properties;
            if (properties!.Lookup.TryGetValue("CurrentSegments", out var segmentPBag))
            {
                foreach (var segmentProperty in (segmentPBag!.Value as ArrayStructProperty)!.Items.Select(x => x as PropertyBag))
                {
                    segmentList.Add(new(segmentProperty!));
                }
            }
            if (properties!.Lookup.TryGetValue("CurrentFeedData", out var fragmentPBag))
            {
                foreach (var fragmentProperty in (fragmentPBag!.Value as ArrayStructProperty)!.Items.Select(x => x as PropertyBag))
                {
                    fragmentList.Add(new(fragmentProperty!));
                }
            }
        }

        public int? internalLevel { // ByteProperty
            get { return properties.Lookup.TryGetValue("Level", out var property) ? Convert.ToInt32((property.Value! as ByteProperty)!.EnumByte) : null; }
            // don't want to support setting this value
        }

        public float? experience { // single (float)
            get { return properties.Lookup.TryGetValue("PendingExperience", out var property) ? property.Get<float>()! : null; }
            set { if(properties.Lookup.TryGetValue("PendingExperience", out var property)) property.Value = value; }
        }

        public int? currentSeed { // Int32
            get { return properties.Lookup.TryGetValue("CurrentSeed", out var property) ? property.Get<int>()! : null; }
            set { if (properties.Lookup.TryGetValue("CurrentSeed", out var property)) property.Value = value; }
        }

        public bool pendingRoll { // byte, 0 = false, 1 = true
            get { return properties.Lookup.TryGetValue("PendingRoll", out var property) ? Convert.ToBoolean(property.Get<byte>()!) : false; }
            set { if (properties.Lookup.TryGetValue("PendingRoll", out var property)) property.Value = Convert.ToByte(value); }
        }

        public List<Segment> segments {
            get { return segmentList; }
        }

        public List<Fragment> fragments {
            get { return fragmentList; }
        }
    }
}
