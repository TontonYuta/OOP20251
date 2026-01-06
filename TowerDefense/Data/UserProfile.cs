using System.IO;
using Newtonsoft.Json;

namespace TowerDefense.Data
{
    public class UserProfile
    {
        // --- TÀI SẢN ---
        public int Diamonds { get; set; } = 200; // Tặng 200 kim cương khởi nghiệp

        // --- CẤP ĐỘ NÂNG CẤP (META-PROGRESSION) ---

        // 1. Archer Upgrades
        public int ArcherDamageLevel { get; set; } = 0;
        public int ArcherRangeLevel { get; set; } = 0;
        public int ArcherSpeedLevel { get; set; } = 0;

        // 2. Cannon Upgrades
        public int CannonDamageLevel { get; set; } = 0;
        public int CannonRangeLevel { get; set; } = 0;
        public int CannonBlastRadiusLevel { get; set; } = 0; // Phạm vi nổ

        // 3. Sniper Upgrades
        public int SniperDamageLevel { get; set; } = 0;
        public int SniperRangeLevel { get; set; } = 0;

        // 4. Ice Upgrades
        public int IceSlowLevel { get; set; } = 0; // Làm chậm mạnh hơn

        // --- SAVE/LOAD SYSTEM ---
        private static string _filePath = "userprofile.json";
        private static UserProfile _instance;

        public static UserProfile Instance
        {
            get
            {
                if (_instance == null) Load();
                return _instance;
            }
        }

        public static void Load()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    _instance = JsonConvert.DeserializeObject<UserProfile>(json);
                }
                catch { _instance = new UserProfile(); }
            }
            else
            {
                _instance = new UserProfile();
                Save();
            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}