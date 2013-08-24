using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;
using System.IO;

namespace npc_abilities_parser
{
    class Program
    {
        static string[] ignoreParam =
        {
	        "interval",
	        "cooldown",
	        "damage_delay",
	        "invuln",
	        "illusion_damage_in_pct",
	        "illusion_damage_out_pct",
	        "incoming_damage",
	        "outgoing_damage",
	        "images_take",
	        "images_do",
	        "fow",
	        "vision"
        };

        static string[] increaseParam =
        {
	        "damage",
	        "radius",
	        "distance",
	        "range",
	        "aoe",
	        "area",
	        "duration",
	        "amount",
	        "count",
	        "bonus",
	        "resistance",
	        "reduction",
	        "heal",
	        "hp",
	        "health",
	        "armor",
	        "regen",
	        "ministun",
	        "slow",
	        "threshold",
	        "min",
	        "max",
	        // Special
	        "mana_void_ministun",
	        "points_per_tick",
	        "per_stack",
	        "explosion",
	        "spirits"
        };

        static void Main(string[] args)
        {
            string path;
            StreamReader reader;

            string output_dmg = "", output_arr = "";

            int lvl = 0;
            List<string> parts;
            string abilityName = "";

            #region UnitAbilities

            path = "C:\\d2\\npc_abilities.txt";
            reader = new StreamReader(path);

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                if (line.IndexOf("{") != -1)
                {
                    lvl++;
                    continue;
                }

                if (line.IndexOf("}") != -1)
                {
                    lvl--;
                    continue;
                }

                parts = line.Split().ToList().FindAll(s => !s.Equals(""));
                for (int i = 0; i < parts.Count; i++)
                    parts[i] = parts[i].Replace("\"", String.Empty);

                if (lvl == 1 && parts.Count == 1)
                {
                    abilityName = parts[0];
                    continue;
                }

                if (lvl == 2 && parts.Count >= 2)
                {
                    var w = false;
                    for (int i = 1; i < parts.Count; i++)
                        if (parts[i] != "0")
                            w = true;

                    if (parts[0] == "AbilityDamage" && w)
                    {
                        output_dmg += "[" + "\"" + abilityName + "\"";

                        for (int i = 1; i < parts.Count; i++)
                            output_dmg += "," + parts[i];
                        
                        output_dmg += "],\n";
                    }
                }

                if (lvl == 4 && parts.Count >= 2)
                {
                    foreach (var e in ignoreParam)
                        if (parts[0].IndexOf(e) != -1)
                            goto END4;

                    foreach (var e in increaseParam)
                    {
                        if (parts[0].IndexOf(e) != -1)
                        {
                            output_arr += "\"" + abilityName + "." + parts[0] + "\",\n";
                            goto END4;
                        }
                    }
                END4:
                    continue;
                }
            }

            File.WriteAllText("C:\\d2\\abilities_damage.txt", output_dmg);
            //File.WriteAllText("C:\\d2\\abilities_array.txt", output_arr);

            #endregion

            #region ItemAbilities

            reader = new StreamReader("C:\\d2\\items.txt");

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                if (line.IndexOf("//") != -1)
                    continue;

                if (line.IndexOf("{") != -1)
                {
                    lvl++;
                    continue;
                }

                if (line.IndexOf("}") != -1)
                {
                    lvl--;
                    continue;
                }

                parts = line.Split().ToList().FindAll(s => !s.Equals(""));
                for (int i = 0; i < parts.Count; i++)
                    parts[i] = parts[i].Replace("\"", String.Empty);

                if (lvl == 1 && parts.Count == 1)
                {
                    abilityName = parts[0];
                    continue;
                }

                if (lvl == 4 && parts.Count >= 2)
                {
                    foreach (var e in ignoreParam)
                        if (parts[0].IndexOf(e) != -1)
                            goto END4;

                    foreach (var e in increaseParam)
                    {
                        if (parts[0].IndexOf(e) != -1)
                        {
                            output_arr += "\"" + abilityName + "." + parts[0] + "\",\n";
                            goto END4;
                        }
                    }
                END4:
                    continue;
                }
            }

            File.WriteAllText("C:\\d2\\abilities_array.txt", output_arr);

            #endregion
        }
    }
}
