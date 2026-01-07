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

        // Thêm tham số levelId để tránh bị hardcode
        public static void SaveLog(bool isVictory, int wave, int levelId)
        {
            List<MatchLog> logs = LoadLogs();

            logs.Add(new MatchLog
            {
                Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                MapName = $"Level {levelId}", // Không còn hardcode "Level 1"
                Result = isVictory ? "VICTORY" : "DEFEAT",
                WaveReached = wave
            });

            // Chỉ giữ 50 trận gần nhất, xóa bớt nếu vượt quá
            if (logs.Count > 50) logs.RemoveAt(0);

            try
            {
                string json = JsonConvert.SerializeObject(logs, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lưu lịch sử: " + ex.Message);
            }
        }

        public static List<MatchLog> LoadLogs()
        {
            if (!File.Exists(_filePath)) return new List<MatchLog>();
            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<MatchLog>>(json) ?? new List<MatchLog>();
            }
            catch
            {
                return new List<MatchLog>();
            }
        }
    }
}