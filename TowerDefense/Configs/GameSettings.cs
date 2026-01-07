namespace TowerDefense.Configs
{
    public static class GameSettings
    {
        // ==========================================
        // DỮ LIỆU CÀI ĐẶT TOÀN CỤC
        // ==========================================
        
        // Âm thanh
        public static float MusicVolume { get; set; } = 0.5f; 
        public static float SfxVolume { get; set; } = 1.0f;

        // Gameplay
        public static bool AutoStartWave { get; set; } = false;
        public static bool ShowHealthBars { get; set; } = true;
        
        // Đồ họa
        public static bool IsHighQuality { get; set; } = true;
    }
}