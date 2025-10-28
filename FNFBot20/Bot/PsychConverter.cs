using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FNFBot20
{
    public static class PsychConverter
    {
        public static bool IsPsychChart(string path)
        {
            if (!File.Exists(path)) return false;
            try
            {
                string json = File.ReadAllText(path);
                JObject obj = JObject.Parse(json);
                return obj["generatedBy"]?.ToString().Contains("Psych Engine") ?? false;
            }
            catch
            {
                return false;
            }
        }

        public static string ConvertToVanillaFNF(string path)
        {
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            JObject psychObj = JObject.Parse(json);

            string songName = Path.GetFileNameWithoutExtension(path);
            double speed = psychObj["speed"]?.Value<double>() ?? 2.5;
            string player1 = psychObj["player1"]?.ToString() ?? "bf";
            string player2 = psychObj["player2"]?.ToString() ?? "dad";
            string stage = psychObj["stage"]?.ToString() ?? "stage";

            double bpm = 150;

            JArray psychNotes = (JArray)psychObj["notes"];
            JArray vanillaNotes = new JArray();
            foreach (JObject sect in psychNotes)
            {
                JArray sectionNotes = (JArray)sect["sectionNotes"];
                bool mustHitSection = sect["mustHitSection"]?.Value<bool>() ?? true;
                int sectionBeats = sect["sectionBeats"]?.Value<int>() ?? 4;
                int lengthInSteps = sectionBeats * 4;

                JArray adjustedNotes = new JArray();
                foreach (JArray note in sectionNotes)
                {
                    if (note.Count >= 2)
                    {
                        while (note.Count > 3)
                        {
                            note.RemoveAt(3);
                        }
                        if (note.Count >= 3 && note[2].Type == JTokenType.String)
                        {
                            note[2] = 0;
                        }
                        adjustedNotes.Add(note);
                    }
                }

                JObject vanillaSect = new JObject
                {
                    ["lengthInSteps"] = lengthInSteps,
                    ["sectionNotes"] = adjustedNotes,
                    ["mustHitSection"] = mustHitSection,
                    ["typeOfSection"] = 0,
                    ["changeBPM"] = false,
                    ["bpm"] = bpm
                };
                vanillaNotes.Add(vanillaSect);
            }

            JObject vanillaSong = new JObject
            {
                ["song"] = songName,
                ["notes"] = vanillaNotes,
                ["bpm"] = bpm,
                ["sections"] = 0,
                ["sectionLengths"] = new JArray(),
                ["player1"] = player1,
                ["player2"] = player2,
                ["player3"] = JValue.CreateNull(),
                ["gfVersion"] = "gf",
                ["stage"] = stage,
                ["validScore"] = false,
                ["speed"] = 1.0,
                ["needsVoices"] = false,
                ["noteStyle"] = "normal",
                ["noteSkin"] = "normal"
            };

            JObject vanillaRoot = new JObject
            {
                ["song"] = vanillaSong
            };

            string dir = Path.GetDirectoryName(path);
            string newPath = Path.Combine(dir, songName + "_vanilla.json");
            File.WriteAllText(newPath, vanillaRoot.ToString(Formatting.Indented));
            return newPath;
        }
    }
}
