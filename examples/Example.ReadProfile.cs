using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;

namespace examples;
internal partial class Example
{
    public static void ReadProfile()
    {
        Console.WriteLine("Profile Data===========");

        string folder =  Utils.GetSteamSavePath();
        string path = Path.Combine(folder, "profile.sav");

        SaveFile sf = SaveFile.Read(path);

        KeyValuePair<string, Property> charactersProp = sf.SaveData.Objects[0].Properties!.Properties.Single(x => x.Key == "Characters");
        KeyValuePair<string, Property> activeProp = sf.SaveData.Objects[0].Properties!.Properties.Single(x => x.Key == "ActiveCharacterIndex");
        //KeyValuePair<string, Property> awardedProp = sf.SaveData.Objects[0].Properties!.Properties.Single(x => x.Key == "AccountAwards");

        ArrayProperty ap = (charactersProp.Value.Value as ArrayProperty)!;
        List<ObjectProperty> op = ap.Items.Select(x => (ObjectProperty)x!).ToList();
        int count = op.Count(x => x.Object != null);
        Console.WriteLine($"You have {count} characters");

        int activeIndex = (int)activeProp.Value.Value!;
        string[] numbers = [
            "first",
            "second",
            "third",
            "fourth",
            "fifth"
        ];

        Console.WriteLine($"Your active character's index is {activeIndex}, which means it's the {numbers[activeIndex]} character on the screen");

        for (int i = 0,charCount=0; i < op.Count; i++)
        {
            if (op[i].Object == null) continue;

            Console.WriteLine($"Your {numbers[charCount]} character (save slot {i}) has:");
            Property traitRank = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "TraitRank").Value;
            Property archetype = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "Archetype").Value;
            Property secondaryArchetype = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "SecondaryArchetype").Value;
            Property gender = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "Gender").Value;
            Property characterHardcoreType = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "CharacterType").Value;
            Property characterData = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "CharacterData").Value;
            Property powerLevel = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "PowerLevel").Value;
            Property itemLevel = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "ItemLevel").Value;
            Property lastSavedTraitPoints = op[i].Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "LastSavedTraitPoints").Value;

            if (powerLevel != null)
            {
                Console.WriteLine($"  Power level: {(int)powerLevel.Value!}");
            }
            if (itemLevel != null)
            {
                Console.WriteLine($"  Item level: {(int)itemLevel.Value!}");
            }
            if (traitRank != null)
            {
                Console.WriteLine($"  Trait rank: {(int)traitRank.Value!}");
            }
            if (lastSavedTraitPoints != null)
            {
                string message = "";
                if (traitRank != null)
                {
                    int unallocated = (int)lastSavedTraitPoints.Value! - (int)traitRank.Value!;
                    message = $", unallocated trait points: {unallocated}";
                }
                Console.WriteLine($"  Total trait points: {(int)lastSavedTraitPoints.Value!}{message}");

            }
            if (gender != null)
            {
                // Male gender is default and is not included
                Console.WriteLine($"  Gender: {((EnumProperty)gender.Value!).EnumValue.Name}");
            }
            if (characterHardcoreType != null)
            {
                // Non-hardcore  is default and is not included
                Console.WriteLine($"  Character Type: {((EnumProperty)characterHardcoreType.Value!).EnumValue.Name}");
            }
            if (archetype != null)
            {
                Console.WriteLine($"  Archetype: {Utils.GetShortenedAssetPath((string)archetype.Value!)}");
            }
            if (secondaryArchetype != null)
            {
                Console.WriteLine($"  Secondary archetype: {Utils.GetShortenedAssetPath((string)secondaryArchetype.Value!)}");
            }

            StructProperty sp = (StructProperty)characterData.Value!;
            SaveData inner = (SaveData)sp.Value!;
            UObject master =  inner.Objects.Single(x => x.Name == "Character_Master_Player_C");
            Component inventory = master.Components!.Single(x => x.ComponentKey == "Inventory");
            Property items = inventory.Properties!.Properties.Single(x => x.Key == "Items").Value;
            ArrayStructProperty asp = (ArrayStructProperty)items.Value!;
            Console.WriteLine("  You have following inventory:");

            foreach (object? o in asp.Items)
            {
                PropertyBag itemProperties = (PropertyBag)o!;

                Property item = itemProperties.Properties.Single(x => x.Key == "ItemBP").Value;
                Property hidden = itemProperties.Properties.Single(x => x.Key == "Hidden").Value;
                Property slot = itemProperties.Properties.Single(x => x.Key == "EquipmentSlotIndex").Value;

                ObjectProperty instanceData = itemProperties["InstanceData"].Get<ObjectProperty>();

                if ((byte)hidden.Value! != 0)
                {
                    //Console.WriteLine($"    **************HIDDEN:");
                    continue;
                }

                string message = "";
                if (instanceData.Object!.Properties!.Contains("Quantity"))
                {
                    message += $" x{instanceData.Object.Properties["Quantity"].ToStringValue()}";
                }
                if ((int)slot.Value! != -1)
                {
                    message += $" ***********************EQUIPPED in slot {(int)slot.Value}";
                }

                string itemName = ((ObjectProperty)item.Value!).ClassName!;
                if (!itemName.Contains("HiddenContainer"))
                {
                    Console.WriteLine($"    {Utils.GetShortenedAssetPath(itemName)}{message}");
                }
            }

            Component traitsComponent = master.Components!.Single(x => x.ComponentKey == "Traits");
            Property traits = traitsComponent.Properties!.Properties.Single(x => x.Key == "Traits").Value;
            ArrayStructProperty aspTraits = (ArrayStructProperty)traits.Value!;
            Console.WriteLine("  You have following Traits:");

            foreach (object? o in aspTraits.Items)
            {
                PropertyBag traitProperties = (PropertyBag)o!;

                Property item = traitProperties.Properties.Single(x => x.Key == "TraitBP").Value;
                Property transient = traitProperties.Properties.Single(x => x.Key == "Transient").Value;
                Property slot = traitProperties.Properties.Single(x => x.Key == "SlotIndex").Value;
                Property level = traitProperties.Properties.Single(x => x.Key == "Level").Value;

                if ((byte)transient.Value! != 0)
                {
                    //Console.WriteLine($"    **************TRANSIENT:");
                    continue;
                }

                string message = "";
                if ((int)slot.Value! != -1)
                {
                    message = $" ***********************EQUIPPED in slot {(int)slot.Value}";
                }
                Console.WriteLine($"    {Utils.GetShortenedAssetPath(((ObjectProperty)item.Value!).ClassName!)} level {(int)level.Value!}{message}");

            }

            charCount++;
        }

    }
}
