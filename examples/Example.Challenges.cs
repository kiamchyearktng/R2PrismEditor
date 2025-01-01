using System.Buffers.Binary;
using lib.remnant2.saves.IO;
using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Memory;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;

namespace examples;
internal partial class Example
{
    public static void Challenges()
    {
        Console.WriteLine("Challenges===========");

        Dictionary<string, string> objectives = new()
        {
            // ReSharper disable StringLiteralTypo
            { "3E4C2F02-472BB871-BE76D7A8-E869D8C2", "Kill Jungle World Boss - Finish Campaign (Survivor)" },
            { "BC7301F3-4340EFA2-E55B6995-C77D97DB", "Kill Fae World Boss - Finish Campaign (Survivor)" },
            { "9EA9A018-4A8B1EC5-334D4DB5-9ACDC4DA", "Kill Nerud World Boss - Finish Campaign (Survivor)" },
            { "E8E2AE55-465512DA-20F7E2B8-97DC7A7F", "Kill Lab Boss - Finish Campaign (Survivor)" },
            { "B9BC305C-4A7A3D58-CFA0759F-C914A400", "Kill Final Boss - Finish Campaign (Survivor)" },
            { "CF080EC5-4624C541-27ED12B3-4266F172", "Kill Jungle World Boss - Finish Campaign (Veteran)" },
            { "EBA4EB4E-452F49A3-527DDC9A-8A784005", "Kill Fae World Boss - Finish Campaign (Veteran)" },
            { "F8E52CFE-4257CD33-40F2DF9B-A062A1EF", "Kill Nerud World Boss - Finish Campaign (Veteran)" },
            { "9EA379FB-416FB280-E03734A8-B0710CA7", "Kill Lab Boss - Finish Campaign (Veteran)" },
            { "C387CCC0-49E47FC8-982653A7-AF028280", "Kill Final Boss - Finish Campaign (Veteran)" },
            { "85714765-4385A092-269FC387-05F2CEED", "Kill Jungle World Boss - Finish Campaign (Nightmare)" },
            { "998893F4-4EDB91C4-062836A3-B96B45EB", "Kill Fae World Boss - Finish Campaign (Nightmare)" },
            { "80DC90B3-4E744298-7377EC8A-D200F4D4", "Kill Nerud World Boss - Finish Campaign (Nightmare)" },
            { "33CB9FB5-4C17EBD1-74603EB6-DC98C1DA", "Kill Lab Boss - Finish Campaign (Nightmare)" },
            { "591A3BBD-44DA5522-769D9A83-F7527CA5", "Kill Final Boss - Finish Campaign (Nightmare)" },
            { "395BA5BA-494A225A-046D668C-B9514B86", "Kill Jungle World Boss - Finish Campaign (Apocalypse)" },
            { "632D3390-43FDAC4C-E977EA94-11A6175A", "Kill Fae World Boss - Finish Campaign (Apocalypse)" },
            { "ADC39805-4881F728-5B706583-409157AA", "Kill Nerud World Boss - Finish Campaign (Apocalypse)" },
            { "3C213F97-44B0694E-9582EF82-F167080E", "Kill Lab Boss - Finish Campaign (Apocalypse)" },
            { "D7DC551D-4A2A05B4-44B79798-7EDC0A7D", "Kill Final Boss - Finish Campaign (Apocalypse)" },
            { "7D57C8EC-4C12B1A6-72737F90-80703BC3", "Kill Jungle World Boss - Finish Campaign (Hardcore)" },
            { "A96B0583-4AF213A8-E2134599-E5907BB1", "Kill Fae World Boss - Finish Campaign (Hardcore)" },
            { "C0791D74-4560C013-5609FC9A-C36D397C", "Kill Nerud World Boss - Finish Campaign (Hardcore)" },
            { "90C248AE-470530A0-29453DB7-C09E1DCB", "Kill Lab Boss - Finish Campaign (Hardcore)" },
            { "06099657-48EE26F3-B14160AE-69D540C5", "Kill Final Boss - Finish Campaign (Hardcore)" },
            { "20DB617A-448F41B9-F197B997-F7E835DE", "Kill Jungle World Boss - Finish Campaign (Hardcore, Veteran)" },
            { "D83F4DDB-4C1710D7-A4F02CBE-A1EFCA42", "Kill Fae World Boss - Finish Campaign (Hardcore, Veteran)" },
            { "EF395D15-4B2200BC-3A79C7B0-8F02FE51", "Kill Nerud World Boss - Finish Campaign (Hardcore, Veteran)" },
            { "CAF078B3-4B433782-7F1E9381-8A62BB97", "Kill Lab Boss - Finish Campaign (Hardcore, Veteran)" },
            { "14D89A62-43D6EDAB-D9CC37AB-FA537B7B", "Kill Final Boss - Finish Campaign (Hardcore, Veteran)" },
            { "88F5D110-4F8FC2A0-CBD23F87-096A8A69", "Finish 5 Biomes - Finish 5 Biomes" },
            { "82549C68-47A24E15-E18F158E-A4776318", "Finish 15 Biomes - Finish 15 Biomes" },
            { "ACE83068-450088C8-3D3FA8A8-4E009B9C", "Finish 30 Biomes - Finish 30 Biomes" },
            { "CD52F7DE-4618D4DD-A1991D94-E48E33C8", "Kill X Bosses - Defeat X World Bosses" },
            { "FA9E9656-47A99109-514867A8-317FD001", "Kill Yaesha World Boss - Defeat Yaesha World Boss Hardcore" },
            { "5D723181-43CF071E-D7D312A1-E599CFA8", "Kill Losomn World Boss - Defeat Losomn World Boss Hardcore" },
            { "4F01E4EF-4891C7E1-3F6A6D8A-8FBAA4F7", "Kill Nerud World Boss - Defeat Nerud World Boss Hardcore" },
            { "D01CBE33-4D00C490-4063FB9C-D2C14102", "Kill Labyrinth World Boss - Defeat Labyrinth World Boss Hardcore" },
            { "5BF22894-486F2872-1D9DCE98-03CE36B6", "Kill Many Faces - Defeat All Bosses" },
            { "6C1DCC18-4D8C8A04-BE98AEA5-0DEF0048", "Kill Ravager - Defeat All Bosses" },
            { "9913D482-413562BC-DCB03C8C-0E20BF21", "Kill Mother Mind - Defeat All Bosses" },
            { "21D2C28C-4081DFF6-73E1EEAE-0453659D", "Kill Root Horror - Defeat All Bosses" },
            { "8760D76F-429AE74E-67F6E89F-B7C0F901", "Kill Shrewd - Defeat All Bosses" },
            { "8F77948F-4BE95E4E-31522EB9-D2E4D352", "Kill Legion - Defeat All Bosses" },
            { "686A7F08-495846E7-1D2225B5-D35817AD", "Kill Nightweaver - Defeat All Bosses" },
            { "4E87628C-4B61F8F0-09E0F988-1D2E0B9A", "Kill Fae King - Defeat All Bosses" },
            { "6E0A0ACC-4D5BF6C0-8B0E14B8-D7117E00", "Kill Bloat King - Defeat All Bosses" },
            { "8A7975D1-4FA4D9D8-8CB78581-E1F2509B", "Kill Grenadier - Defeat All Bosses" },
            { "741B6FFA-43ABF22D-4A01449C-A280AC76", "Kill Archon - Defeat All Bosses" },
            { "DEB77A92-46375286-EBB6F080-8E3E82A0", "Kill Red Prince - Defeat All Bosses" },
            { "02D135CF-4D337EBE-C0BC8A8D-07C52DFB", "Kill Nerud Guardian - Defeat All Bosses" },
            { "83620A7C-406BF2EB-BC5CE9A4-2C41C6AF", "Kill Tal Ratha - Defeat All Bosses" },
            { "DED753A5-4DAEBDF5-0B81DCA3-7B8A0DB5", "Kill Abomination - Defeat All Bosses" },
            { "29985858-4F465A66-79B6B8BF-5A10D18E", "Kill Custodian's Eye - Defeat All Bosses" },
            { "16D6C192-41CE8B04-953577AB-9ACC7747", "Kill Hatchery - Defeat All Bosses" },
            { "10F756CA-4AFDF975-C6C317BE-F36B0705", "Kill Phantom - Defeat All Bosses" },
            { "13B32649-45C6BD47-CBC226A2-91E5FC25", "Kill Labyrinth World Boss - Defeat All Bosses" },
            { "38454571-49A3C942-36466797-0BBF4F34", "Kill Final Boss - Defeat All Bosses" },
            { "FFBA0676-4D3D4968-850D04B0-4A5CE89F", "High Five Someone - High Five" },
            { "A071DF92-4C794849-4A1002B0-DFF292DA", "Die 15 Times - Die 15 Times" },
            { "D8479145-44C2D683-74BE74A1-ADDF77F8", "Flop 100 Times - Flop Dodge a Bunch" },
            { "BCD29B16-4AD4F664-467B11B8-40B71EC6", "Revive Allies - No Soldier Left Behind" }
            // ReSharper restore StringLiteralTypo
        };

        Dictionary<string, string> achievements = new()
        {
            // ReSharper disable StringLiteralTypo
            { "95D6799E-449077F7-0969C7AB-8CD29A2C", "Blue Goddess - Meet Nimue" },
            { "F72E913A-47822819-53313CB6-C6581803", "Familiar Face - Meet the Flautist" },
            { "9DBA284E-4A76A082-E429C78A-45487DA2", "Equal Measures - Receive an Alignment Reading from Meidra" },
            { "B4F64A8D-4710E448-0EE98194-B4A0E6F1", "Not a Janitor - Meet the Custodian" },
            { "E708790E-489B86E3-8EAFCF87-59F69BDD", "Tall Tales - Listen to All of Mudtooth's Stories" },
            { "72C2A1C9-484E8F50-CA734F82-C6871F5C", "Am I Seeing This? - Defeat 10 Aberrations" },
            { "1615096B-44932A2A-BC6A57A0-4C4B87DC", "Ghost in the Machine - Defeat 25 Aberrations" },
            { "184257D9-4D0630D1-DC4502BD-123FA103", "Not So Special Now - Defeat 100 Special Enemies" },
            { "FE931C6A-4B2CDAF3-B231A8BB-D3F632B5", "Quest for Survival - Defeat a World Boss" },
            { "FCABEBB1-4B3613EE-BB089A8F-8573E7B8", "Only Human - Defeat a Boss in Single-Player Without Taking Any Damage" },
            { "A3070D9C-46E42A66-BFD69C84-73CB6281", "Chaos - Defeat the Ravager" },
            { "EAF448A2-4607B573-AC7FCAA0-8042E2F4", "Dark Designs - Defeat the Nightweaver" },
            { "3B3CBE2F-4D477AA6-6AE50CBA-BC21C500", "Forever is a Long Time Coming - Defeat the Final Boss" },
            { "2A0C2C43-4E1A012B-395F6180-CA24CECA", "Gleaming the Cube - Defeat the Labyrinth Boss" },
            { "DC3E4036-4090C37B-80E71DA1-1A6516C3", "Madman's Paradise - Defeat Tal'Ratha" },
            { "79F9C41C-43D8A77A-5D2891A0-12D290C7", "Power Surge - Defeat Guardian of N'Erud" },
            { "885FB008-4DAB8CA6-BBF1CC80-9E8AF2FC", "The God Gambit - Defeat Many Faces" },
            { "DFFAEB49-41BC43BF-87BAA59F-3C2DD9ED", "The Killing Jar - Defeat the Root Mantis" },
            { "83077A6D-40D1ED1F-0F32CAA9-6498FEBF", "Traitor - Defeat the Fae Imposter" },
            { "FA272F4C-4FB32A19-8EBEFAA2-4243317A", "Red Room - Discover a Blood Moon Room" },
            { "AC521D58-4781A434-63272FB6-DF1166EF", "The Agenda - Discover Leto's Lab" },
            { "26160E9D-43131C52-67C8DEB6-DCBBBCBC", "Boss'n Up - Craft a Boss Weapon" },
            { "BD0E1340-4F3F57D2-7939BAA1-2D81447C", "Edgelord - Acquire 10 Melee Weapons" },
            { "91CC492B-4F067DD1-84DF9184-787314B8", "Cutting Edge - Acquire 20 Melee Weapons" },
            { "DE9082AA-4C9B4A84-153734AD-A8E6C6FA", "First of Many - Choose Your First Archetype" },
            { "64C832B6-420D74CD-E13F1CB4-21D04D34", "Duality - Slot a Second Archetype" },
            { "86813184-4B0615D2-FC828C9A-155591F1", "Shhh...It's a Secret - Obtain a Secret Archetype" },
            { "DE45C8C6-439B3BB1-6105E7A0-CD631F55", "Top Performer - Reach Level 10 on Any Archetype" },
            { "90726A07-43EA8231-2A12738D-3405792F", "Not Your Average Trait - Obtain a Non-Starter Trait" },
            { "D547171B-45B2C9C3-91A721AB-9CF9C26C", "All These Traits... - Obtain 10 Traits" },
            { "552A24DD-42C4678B-DA1BB3A2-EA55066F", "Proving Grounds - Acquire 20 traits" },
            { "95A35103-463219BC-17A567AB-A0862766", "Scrap Collector - Acquire 50,000 Scrap" },
            { "F851C284-47B5EDED-44C27DBC-4D9ED445", "Scrap Hoarder - Acquire 100,000 Scrap" },
            { "DF8E588E-48DFB965-9F011780-FFABAB8A", "Strapped - Acquire 15 Guns" },
            { "EEF7A6A0-4167F755-5C985B9D-E6CEFFC0", "The Trigger - Acquire 30 Guns" },
            { "885CD8D5-403ED072-1DF21FAB-D2483B7C", "The Web - Obtain an Item From the Nightweaver's Web" },
            { "CD85A5B7-444B7E97-FD5688B9-272D203B", "Was This Supposed To Happen? - Acquire a World Boss's Alternate Reward" },
            { "CBCE064F-45D63F06-F9FC04B4-768EE5B1", "The Collector - Acquire 10 Relics" },
            { "95C93FBE-4DBC6996-EB27A391-C70EDB71", "Bad Moon Rising - Craft an Item at the Blood Moon Altar" },
            { "7E15429C-460DF284-F66BC6B4-38F089BA", "Carnage in C-Minor - Play a Secret Song on the Water Harp" },
            { "9031B5F7-4E1F92DB-C587D8BF-BC6EF13A", "Expanding Horizons - Craft a Non-Starter Weapon Mod" },
            { "F3116187-4AD71542-62DA6092-885F15B3", "Crafty - Craft 15 Weapon Mods" },
            { "AADC5BA0-4A9F70D1-65BF7DA7-A64A2F05", "Trait Chaser - Upgrade Any Trait to 10" },
            { "2891425D-4DD95346-0FD6588C-94D2269D", "Maxed Out! - Acquire the Max Number of Trait Points" },
            { "A4E4E22B-4FDABA45-7791FA9F-633C7633", "Almost There - Upgrade a Boss Weapon to +5" },
            { "273E57D6-40FEA7FA-176AE39E-D0D03A43", "The Ultimate Weapon - Upgrade a Boss Weapon to +10" },
            { "814BAE10-4FAABC5B-EA498AB7-9BF337ED", "Good, But Could Be Better! - Upgrade a Standard Weapon to +10" },
            { "20C802B3-4041ABF3-7AE9E2A1-EA18D5C5", "No One Should Have All That Power - Upgrade a Standard Weapon to +20" },
            { "07F74FB5-4494039A-4537F6A3-8CBB7C2F", "Make Some Room - Upgrade Relic Capacity" },
            { "15A2A6C2-46ADD690-33FDBDAB-BC80BE8E", "Transmutate - Upgrade a Weapon Mutator to +10" }
            // ReSharper restore StringLiteralTypo
        };

        string folder = Utils.GetSteamSavePath();
        string path = Path.Combine(folder, "profile.sav");

        SaveFile sf = SaveFile.Read(path);
        Navigator navigator = new(sf);

        KeyValuePair<string, Property> charactersProp = sf.SaveData.Objects[0].Properties!.Properties.Single(x => x.Key == "Characters");

        ArrayProperty ap = (charactersProp.Value.Value as ArrayProperty)!;
        List<ObjectProperty> op = ap.Items.Select(x => (ObjectProperty)x!).ToList();

        foreach (var character in op)
        {
            if (character.ObjectIndex < 0) continue;
            Console.WriteLine("Character starts");
            Property characterData = character.Object!.Properties!.Properties.SingleOrDefault(x => x.Key == "CharacterData").Value;
            ArrayStructProperty asp = (ArrayStructProperty)navigator.GetProperty("ObjectiveProgressList", characterData)!.Value!;

            foreach (object? obj in asp.Items)
            {
                PropertyBag item = (PropertyBag)obj!;
                FGuid objectiveId = item["ObjectiveID"].Get<FGuid>();
                int progress = item["Progress"].Get<int>();

                WriterBase w = new();
                w.Write(objectiveId);

                uint u1 = BinaryPrimitives.ReadUInt32LittleEndian(w.ToArray().AsSpan()[..4]);
                uint u2 = BinaryPrimitives.ReadUInt32LittleEndian(w.ToArray().AsSpan()[4..8]);
                uint u3 = BinaryPrimitives.ReadUInt32LittleEndian(w.ToArray().AsSpan()[8..12]);
                uint u4 = BinaryPrimitives.ReadUInt32LittleEndian(w.ToArray().AsSpan()[12..16]);
                string uu = $"{u1:X8}-{u2:X8}-{u3:X8}-{u4:X8}";

                //string r1 = BitConverter.ToStringValue(w.ToArray()).Replace("-", "");

                string message = objectives.TryGetValue(uu, out string? objective) ? $"[Challenge] {objective}" : uu;
                if (achievements.TryGetValue(uu, out string? achievement))
                {
                    message = $"[Achievement] {achievement}";
                }

                Console.WriteLine($"{message}: {progress}");
            }
        }
    }
}
