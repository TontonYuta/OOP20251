using System;
using TowerDefense.Configs;

namespace TowerDefense.Managers
{
    public class WaveManager
    {
        public int CurrentWave { get; private set; } = 0;
        public bool IsWaveRunning { get; private set; } = false;

        private float _spawnTimer = 0;
        private float _spawnInterval = 1.0f;
        private int _enemiesLeftToSpawn = 0;

        private Random _random = new Random();

        public void StartNextWave()
        {
            CurrentWave++;
            IsWaveRunning = true;

            // --- TĂNG SỐ LƯỢNG QUÁI (ĐÔNG HƠN) ---
            // Wave 1: 13 con, Wave 10: 40 con
            _enemiesLeftToSpawn = 10 + (CurrentWave * 3);

            // Tốc độ ra quái nhanh dần (Max tốc độ: 0.15s/con)
            _spawnInterval = Math.Max(0.15f, 1.2f - (CurrentWave * 0.05f));

            // Nếu là Wave Boss (Chia hết cho 5), ra ít quái hơn nhưng chất lượng hơn
            if (CurrentWave % 5 == 0)
            {
                _enemiesLeftToSpawn = 5 + (CurrentWave / 2); // Ít nhưng trâu
                _spawnInterval = 2.0f; // Ra chậm để người chơi kịp thở
            }

            _spawnTimer = 0;
        }

        public int Update(float deltaTime)
        {
            if (!IsWaveRunning) return -1;

            if (_enemiesLeftToSpawn <= 0)
            {
                IsWaveRunning = false;
                return -1;
            }

            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0)
            {
                _spawnTimer = _spawnInterval;
                _enemiesLeftToSpawn--;
                return GetEnemyTypeForCurrentWave();
            }
            return -1;
        }

        // --- THUẬT TOÁN CHỌN QUÁI KHÓ HƠN ---
        private int GetEnemyTypeForCurrentWave()
        {
            // ID của các loại quái trong mảng (0-19)
            int minType = 0;
            int maxType = 0;

            // 1. FINAL BOSS (Wave 20)
            if (CurrentWave == 20) return 19; // Demon King

            // 2. BOSS WAVE (Mỗi 5 wave: 5, 10, 15)
            if (CurrentWave % 5 == 0)
            {
                // Wave 5: Boss Tier 2 (Witch/Ghost)
                if (CurrentWave == 5) return _random.Next(8, 10);

                // Wave 10: Boss Tier 3 (Golem/Assassin)
                if (CurrentWave == 10) return _random.Next(13, 15);

                // Wave 15: Boss Tier 4 (Cyclops/Hydra...)
                if (CurrentWave == 15) return _random.Next(15, 19);
            }

            // 3. HARD WAVE (Mỗi 3 wave: 3, 6, 9, 12...)
            // Xuất hiện quái mạnh hơn bình thường 1 bậc
            if (CurrentWave % 3 == 0)
            {
                minType = Math.Min(10, CurrentWave);
                maxType = Math.Min(14, CurrentWave + 2);
            }
            // 4. NORMAL WAVE
            else
            {
                // Cửa sổ trượt: Wave 1 (0-3), Wave 2 (1-4)...
                minType = Math.Max(0, CurrentWave - 2);
                maxType = Math.Min(14, CurrentWave + 2);
            }

            // Random trong khoảng tính toán
            if (maxType >= GameConfig.Enemies.Length) maxType = GameConfig.Enemies.Length - 1;
            return _random.Next(minType, maxType + 1);
        }
    }
}