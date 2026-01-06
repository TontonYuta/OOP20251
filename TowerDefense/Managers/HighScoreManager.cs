using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TowerDefense.Managers
{
    public class PlayerScore
    {
        public string Name { get; set; }
        public int Score { get; set; } // Score = (Tiền còn lại + Máu còn lại * 10)
        public string Date { get; set; }
    }

    public static class HighScoreManager
    {
        private static string _filePath = "highscores.json";

        public static void SaveScore(string playerName, int score)
        {
            List<PlayerScore> scores = LoadScores();

            // Thêm điểm mới
            scores.Add(new PlayerScore
            {
                Name = playerName,
                Score = score,
                Date = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            });

            // Sắp xếp giảm dần và lấy Top 10
            scores = scores.OrderByDescending(s => s.Score).Take(10).ToList();

            // Lưu lại file
            string json = JsonConvert.SerializeObject(scores, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static List<PlayerScore> LoadScores()
        {
            if (!File.Exists(_filePath)) return new List<PlayerScore>();

            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<PlayerScore>>(json) ?? new List<PlayerScore>();
        }
    }
}