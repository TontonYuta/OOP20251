using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TowerDefense.Managers
{
    public class MatchLog
    {
        public string Date { get; set; }
        public string MapName { get; set; }
        public string Result { get; set; } // "VICTORY" or "DEFEAT"
        public int WaveReached { get; set; }
    }

    public static class HistoryManager
    {
        private static string _filePath = "history.json";

        public static void SaveLog(bool isVictory, int wave)
        {
            List<MatchLog> logs = LoadLogs();

            logs.Add(new MatchLog
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                MapName = "Level 1", // Tạm hardcode, sau này lấy từ GameManager
                Result = isVictory ? "VICTORY" : "DEFEAT",
                WaveReached = wave
            });

            // Chỉ lưu 50 trận gần nhất
            if (logs.Count > 50) logs.RemoveAt(0);

            string json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static List<MatchLog> LoadLogs()
        {
            if (!File.Exists(_filePath)) return new List<MatchLog>();
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<MatchLog>>(json) ?? new List<MatchLog>();
        }
    }
}