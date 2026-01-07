using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TowerDefense.Managers
{
    // Class đại diện cho 1 người chơi trong bảng xếp hạng
    public class PlayerScore
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public static class HighScoreManager
    {
        // Tên file lưu điểm (nằm cùng thư mục với file .exe của game)
        private static string _filePath = "highscores.txt";

        // Hàm lưu điểm mới
        public static void SaveScore(string name, int score)
        {
            // 1. Tải danh sách cũ lên
            List<PlayerScore> scores = LoadScores();

            // 2. Thêm điểm mới
            scores.Add(new PlayerScore { Name = name, Score = score });

            // 3. Sắp xếp giảm dần (Điểm cao lên đầu)
            scores = scores.OrderByDescending(s => s.Score).ToList();

            // 4. Giới hạn chỉ lưu Top 10 người cao nhất
            if (scores.Count > 10) scores = scores.Take(10).ToList();

            // 5. Ghi đè lại vào file text
            SaveToFile(scores);
        }

        // Hàm đọc điểm từ file
        public static List<PlayerScore> LoadScores()
        {
            var list = new List<PlayerScore>();

            // Kiểm tra xem file có tồn tại không
            if (File.Exists(_filePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(_filePath);
                    foreach (string line in lines)
                    {
                        // Định dạng file: Tên|Điểm
                        string[] parts = line.Split('|');
                        if (parts.Length == 2)
                        {
                            if (int.TryParse(parts[1], out int s))
                            {
                                list.Add(new PlayerScore { Name = parts[0], Score = s });
                            }
                        }
                    }
                }
                catch { }
            }

            // Sắp xếp lại lần nữa cho chắc chắn
            return list.OrderByDescending(s => s.Score).ToList();
        }

        // Helper ghi file
        private static void SaveToFile(List<PlayerScore> scores)
        {
            try
            {
                var lines = new List<string>();
                foreach (var s in scores)
                {
                    // Lưu dạng: Tên|Điểm
                    lines.Add($"{s.Name}|{s.Score}");
                }
                File.WriteAllLines(_filePath, lines);
            }
            catch { }
        }
    }
}