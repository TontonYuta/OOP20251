using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization; // Dùng XML có sẵn, không cần System.Web
using TowerDefense.Managers;
using TowerDefense.Entities.Towers;

namespace TowerDefense.Data
{
    // Cấu trúc dữ liệu để lưu trữ (Public để XML đọc được)
    public class GameSaveData
    {
        public int LevelId { get; set; }
        public int Wave { get; set; }
        public int Money { get; set; }
        public int Lives { get; set; }
        public List<TowerSaveData> Towers { get; set; } = new List<TowerSaveData>();
    }

    public class TowerSaveData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int TypeId { get; set; }
        public int Level { get; set; }
    }

    public static class SaveLoadSystem
    {
        private static string _savePath = "savegame.xml"; // Đổi đuôi file thành XML

        public static void SaveGame()
        {
            try
            {
                var data = new GameSaveData
                {
                    LevelId = GameManager.Instance.LevelMgr.CurrentLevelId,
                    Wave = GameManager.Instance.WaveMgr.CurrentWave,
                    Money = GameManager.Instance.PlayerMoney,
                    Lives = GameManager.Instance.PlayerLives
                };

                // Lưu danh sách tháp đang hoạt động
                foreach (var t in GameManager.Instance.Towers)
                {
                    if (!t.IsActive) continue;
                    data.Towers.Add(new TowerSaveData
                    {
                        X = t.X,
                        Y = t.Y,
                        TypeId = t.TypeId,
                        Level = t.Level
                    });
                }

                // Ghi ra file XML
                XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
                using (StreamWriter writer = new StreamWriter(_savePath))
                {
                    serializer.Serialize(writer, data);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lỗi lưu game: " + ex.Message);
            }
        }

        public static void LoadGame()
        {
            if (!File.Exists(_savePath))
            {
                System.Windows.Forms.MessageBox.Show("Chưa có file save nào!");
                return;
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
                GameSaveData data;

                using (StreamReader reader = new StreamReader(_savePath))
                {
                    data = (GameSaveData)serializer.Deserialize(reader);
                }

                if (data != null)
                {
                    // 1. Khôi phục thông số Game
                    GameManager.Instance.StartGame(data.LevelId);
                    GameManager.Instance.PlayerMoney = data.Money;
                    GameManager.Instance.PlayerLives = data.Lives;

                    // (Lưu ý: WaveManager cần reset về wave đã lưu nếu muốn chuẩn xác)

                    // 2. Khôi phục Tháp
                    GameManager.Instance.Towers.Clear();

                    foreach (var tData in data.Towers)
                    {
                        GameManager.Instance.SelectedTowerType = tData.TypeId;

                        // Ép kiểu float -> int
                        int buildX = (int)tData.X;
                        int buildY = (int)tData.Y;

                        if (GameManager.Instance.TryBuildTower(buildX, buildY))
                        {
                            var newTower = GameManager.Instance.Towers[GameManager.Instance.Towers.Count - 1];
                            // Nâng cấp lại cấp độ
                            for (int i = 1; i < tData.Level; i++)
                            {
                                newTower.Upgrade();
                            }
                        }
                    }

                    GameManager.Instance.SelectedTowerType = -1; // Reset selection
                    System.Windows.Forms.MessageBox.Show("Đã load game thành công!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("File save lỗi: " + ex.Message);
            }
        }
    }
}