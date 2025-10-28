using System;
using System.Collections.Generic;
using FridayNightFunkin;

namespace FNFBot20
{
    public class MapBot
    {
        
        public FNFSong song { get; set; }
        
        public MapBot(string songDir)
        {
            // Detect and convert Psych Engine JSON to a compatible format if needed
            try
            {
                string pathToLoad = songDir;
                if (PsychConverter.IsPsychChart(songDir))
                {
                    string converted = PsychConverter.ConvertToVanillaFNF(songDir);
                    if (!string.IsNullOrEmpty(converted))
                        pathToLoad = converted;
                }

                song = new FNFSong(pathToLoad);
            }
            catch (Exception e)
            {
                Form1.WriteToConsole("Failed to load map: " + e.Message);
                throw;
            }
        }

        public List<FNFSong.FNFNote> GetHitNotes(FNFSong.FNFSection sect)
        {
            List<FNFSong.FNFNote> notes = new List<FNFSong.FNFNote>();
            foreach (FNFSong.FNFNote n in sect.Notes)
            {
                n.Time = Math.Round(n.Time);
                // Only add player notes (lanes 0-3), ignore opponent notes (lanes 4-7)
                if (n.Type < (FNFSong.NoteType) 4)
                {
                    notes.Add(n);
                }
            }

            return notes;
        }
        
        public void Compile(string path)
        {
            song.SaveSong(path);
        }
    }
}