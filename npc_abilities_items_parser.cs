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
            "damage_incoming",
            "damage_outgoing",
	        "images_take",
	        "images_do",
	        "fow",
	        "vision",
            "tooltip"
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

        static string[] decreaseParam =
        {
            "fade_delay",
            "fade_time",
            "transparency_fade",
            "focusfire_damage_reduction",
            "backlash_damage",
            "health_cost_percent"
        };

        static string[] chanceFlags =
        {
	        "chance"
        };

        static string[] mrFlags =
        {
	        "spell_shield_resistance",
            "magic_resistance",
            "magic_resist",
            "resistance_per_stack",
            "spell_resistance",
            "spell_resist"
        };

        static string[] stunFlags =
        {
	        "stun"
        };

        static string[] damageFlags =
        {
	        "damage"
        };

        static string[] durationFlags =
        {
	        "duration"
        };

        static string[] distFlags =
        {
            "radius",
	        "distance",
            "range",
            "aoe",
            "area"
        };

        static string AddFlags(string p)
        {
            foreach (var e in chanceFlags)
                if (p.IndexOf(e) != -1)
                    return ", chance";

            foreach (var e in mrFlags)
                if (p.IndexOf(e) != -1)
                    return ", mr";

            foreach (var e in stunFlags)
                if (p.IndexOf(e) != -1)
                    return ", stun, dur";

            foreach (var e in durationFlags)
                if (p.IndexOf(e) != -1)
                    return ", dur";

            foreach (var e in distFlags)
                if (p.IndexOf(e) != -1)
                    return ", dst";

            foreach (var e in damageFlags)
                if (p.IndexOf(e) != -1)
                    return ", dmg";

            return null;
        }

        static void Main(string[] args)
        {
            string path;
            StreamReader reader;

            string output_arr = "", abilityCooldown = "", abilityHookCalls = "";

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
                        if (parts[i] != "0" && parts[i] != "0.0")
                            w = true;

                    if (parts[0] == "AbilityCooldown" && w)
                    {
                        abilityCooldown += "[\"" + abilityName + "\"";

                        for (int i = 1; i < parts.Count; i++)
                            abilityCooldown += "," + parts[i];

                        abilityCooldown += "],\n";
                    }
                }

                if (lvl == 4 && parts.Count >= 2)
                {
                    foreach (var e in ignoreParam)
                        if (parts[0].IndexOf(e) != -1)
                            goto END4;

                    foreach (var ie in increaseParam)
                    {
                        if (parts[0].IndexOf(ie) != -1)
                        {
                            output_arr += "[\"" + abilityName + "." + parts[0] + "\", " + "\"" + "inc";
                            output_arr += AddFlags(parts[0]);
                            output_arr += "\"],\n";
                            goto END4;
                        }
                    }

                    foreach (var e in decreaseParam)
                    {
                        if (parts[0].IndexOf(e) != -1)
                        {
                            output_arr += "[\"" + abilityName + "." + parts[0] + "\", " + "\"" + "dec";
                            output_arr += AddFlags(parts[0]);
                            output_arr += "\"],\n";
                            goto END4;
                        }
                    }

                END4:
                    continue;
                }
            }

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

                if (lvl == 2 && parts.Count >= 2)
                {
                    var w = false;
                    for (int i = 1; i < parts.Count; i++)
                        if (parts[i] != "0" && parts[i] != "0.0")
                            w = true;

                    if (parts[0] == "AbilityCooldown" && w)
                    {
                        abilityCooldown += "[\"" + abilityName + "\"";

                        for (int i = 1; i < parts.Count; i++)
                            abilityCooldown += "," + parts[i];

                        abilityCooldown += "],\n";
                    }
                }

                if (lvl == 4 && parts.Count >= 2)
                {
                    foreach (var e in ignoreParam)
                        if (parts[0].IndexOf(e) != -1)
                            goto END4;

                    foreach (var ie in increaseParam)
                    {
                        if (parts[0].IndexOf(ie) != -1)
                        {
                            output_arr += "[\"" + abilityName + "." + parts[0] + "\", " + "\"" + "inc";
                            output_arr += AddFlags(parts[0]);
                            output_arr += "\"],\n";
                            goto END4;
                        }
                    }

                    foreach (var e in decreaseParam)
                    {
                        if (parts[0].IndexOf(e) != -1)
                        {
                            output_arr += "[\"" + abilityName + "." + parts[0] + "\", " + "\"" + "dec";
                            output_arr += AddFlags(parts[0]);
                            output_arr += "\"],\n";
                            goto END4;
                        }
                    }

                END4:
                    continue;
                }
            }

            File.WriteAllText("C:\\d2\\abilities_hook.txt", abilityHookCalls + abilityCooldown);
            File.WriteAllText("C:\\d2\\abilities_array.txt", output_arr);

            #endregion
        }
    }
}
