﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WpfDialogs;

namespace Match
{
    internal class TimeRecorder
    {
        private readonly string BestTimesFile = "BestTimes.txt";
        private List<(int time, string name)> BestTimes;
        private string LastPlayer;
        private readonly int BestTimesToRecord = 5;

        internal void RecordBestTimes(int emojisToGuess, int tenthsOfSecondsElapsed, double top, double left)
        {
            ReadBestTimesFromFile(emojisToGuess);

            if (BestTimes.Count < BestTimesToRecord || tenthsOfSecondsElapsed < BestTimes.Max(l => l.time))
            {
                StringGetter name = new($"You have finished in {tenthsOfSecondsElapsed / 10F:0.0s}", "Please enter your name:",
                    !string.IsNullOrWhiteSpace(LastPlayer) ? LastPlayer : "Anonymous", top, left);

                if (name.ShowDialog() == true)
                {
                    LastPlayer = !string.IsNullOrWhiteSpace(name.Answer) ? name.Answer : "Anonymous";
                    BestTimes.Add(new(tenthsOfSecondsElapsed, LastPlayer));
                    SaveBestTimes(emojisToGuess, BestTimes);
                }
            }
        }

        private void SaveBestTimes(int emojisToGuess, List<(int time, string name)> bestTimes)
        {
            string tempFile = Path.GetTempFileName();
            var linesToKeep = File.ReadLines(BestTimesFile).Where(l => !l.StartsWith($"Emojis={emojisToGuess}"));
            File.WriteAllLines(tempFile, linesToKeep);
            File.Delete(BestTimesFile);
            File.Move(tempFile, BestTimesFile);

            using FileStream fs = new(BestTimesFile, FileMode.Append, FileAccess.Write);
            using StreamWriter sw = new(fs);
            foreach ((int bestTime, string byWho) in bestTimes.OrderBy(l => l.time).Take(BestTimesToRecord))
            {
                sw.WriteLine($"Emojis={emojisToGuess}|{bestTime}|{byWho}");
            }
        }

        private List<(int, string)> GetBestTimesFromFile(int emojisToGuess)
        {
            using FileStream fs = new(BestTimesFile, FileMode.OpenOrCreate, FileAccess.Read);
            using StreamReader sr = new(fs);
            List<(int time, string name)> result = new();
            var str = sr.ReadLine();
            while (!string.IsNullOrWhiteSpace(str))
            {
                if (str.StartsWith($"Emojis={emojisToGuess}"))
                {
                    string[] bits = str.Split('|');
                    result.Add(new(Convert.ToInt32(bits[1], CultureInfo.InvariantCulture), bits[2]));
                }
                str = sr.ReadLine();
            }
            return result;
        }

        public string GetBestTimes(int emojisToGuess)
        {
            string result = $"Best times for {emojisToGuess * 2} emojis:";
            if (BestTimes.Count > 0)
            {
                foreach ((int time, string name) in BestTimes.OrderBy(l => l.time).Take(BestTimesToRecord))
                {
                    result += $"\n * {time / 10F:0.0s} by {name}";
                }
            }
            else
            {
                result += "\n * No best time has been recorded";
            }

            return result;
        }

        public void ReadBestTimesFromFile(int emojisToGuess)
        {
            BestTimes = GetBestTimesFromFile(emojisToGuess);
        }

        public string GeMinimumTime(int emojisToGuess)
        {
            return BestTimes.Count > 0
                ? $"Best time for {emojisToGuess * 2} emojis is {BestTimes.Min(l => l.time) / 10F:0.0s}"
                : "No best time has been recorded";
        }
    }
}