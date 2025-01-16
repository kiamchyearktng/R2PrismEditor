using lib.remnant2.saves.Model.Parts;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prismeditor.definitions
{
    public class Segment(PropertyBag properties)
    {
        internal PropertyBag properties = properties;

        public string Name {
            get { return (properties["RowName"].Value as FName)!.Name; }
            set { (properties["RowName"].Value as FName)!.Name = value; }
        }

        public int Level {
            get { return properties["Level"].Get<int>(); }
            // no set
        }
    }

    public class Fragment(PropertyBag properties)
    {
        internal PropertyBag properties = properties;

        public string Name {
            get { return (properties["RowName"].Value as FName)!.Name; }
            set { (properties["RowName"].Value as FName)!.Name = value; }
        }

        public int Level {
            get { return properties["FedLevel"].Get<int>(); }
            set { properties["FedLevel"].Value = value; }
        }
    }
}
