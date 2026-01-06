using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefense.Entities;
using TowerDefense.Entities.Towers;
using TowerDefense.Entities.Enemies;
using TowerDefense.Configs;

namespace TowerDefense.Managers
{
    public class GameManager
    {
        // =========================================================
        // 1. SINGLETON & STATE
        // =========================================================
        public static GameManager Instance { get; private set; } = new GameManager();

        // Game State
        public int PlayerMoney { get; set; }
        public int PlayerLives { get; set; }
        public float GameSpeed { get; set; } = 1.0f;
        public bool IsVictory { get; private set; }
        public bool IsAutoWave { get; set; } = false;

        // Selection State
        public int SelectedTowerType { get; set; } = -1; // -1: None

        // Managers
        public WaveManager WaveMgr { get; private set; }
        public LevelManager LevelMgr { get; private set; }
        public SkillManager SkillMgr { get; private set; }

        // Entities Lists
        public List<Tower> Towers { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Projectile> Projectiles { get; private set; }
        public List<EnemyProjectile> EnemyProjectiles { get; private set; } // Đạn của quái
        public List<VisualEffect> Effects { get; private set; } // Hiệu ứng hình ảnh
        public List<Point> CurrentMapPath { get; private set; }

        private GameManager()
        {
            WaveMgr = new WaveManager();
            LevelMgr = new LevelManager();
            SkillMgr = new SkillManager();

            Towers = new List<Tower>();
            Enemies = new List<Enemy>();
            Projectiles = new List<Projectile>();
            EnemyProjectiles = new List<EnemyProjectile>();
            Effects = new List<VisualEffect>();
        }

        // =========================================================
        // 2. GAME FLOW
        // =========================================================
        public void StartGame(int levelId)
        {
            // Reset State
            PlayerMoney = 6500;
            PlayerLives = 20;
            GameSpeed = 1.0f;
            IsVictory = false;
            IsAutoWave = false;
            SelectedTowerType = -1;

            // Clear Lists
            Towers.Clear();
            Enemies.Clear();
            Projectiles.Clear();
            EnemyProjectiles.Clear();
            Effects.Clear();

            // Load Level
            CurrentMapPath = LevelMgr.LoadLevelPath(levelId);
            WaveMgr = new WaveManager();
            SkillMgr = new SkillManager();
        }

        public void Update(float deltaTime)
        {
            float scaledDeltaTime = deltaTime * GameSpeed;

            // 1. Update Managers
            SkillMgr.Update(scaledDeltaTime);

            // 2. Update Wave (Spawn Enemies)
            if (IsAutoWave && !WaveMgr.IsWaveRunning && Enemies.Count == 0 && !IsVictory)
            {
                WaveMgr.StartNextWave();
            }

            int spawnEnemyId = WaveMgr.Update(scaledDeltaTime);
            if (spawnEnemyId != -1)
            {
                SpawnEnemy(spawnEnemyId);
            }

            // Check Victory
            if (!WaveMgr.IsWaveRunning && WaveMgr.CurrentWave >= LevelMgr.MaxWaves && Enemies.Count == 0)
            {
                IsVictory = true;
            }

            // 3. UPDATE ENTITIES
            UpdateTowers(scaledDeltaTime);
            UpdateEnemies(scaledDeltaTime);
            UpdateProjectiles(scaledDeltaTime);
            UpdateEnemyProjectiles(scaledDeltaTime);
            UpdateEffects(scaledDeltaTime);
        }

        // =========================================================
        // 3. UPDATE LOGIC CHI TIẾT
        // =========================================================

        private void UpdateTowers(float dt)
        {
            foreach (var tower in Towers)
            {
                tower.Update(dt);
            }
        }

        private void UpdateEnemies(float dt)
        {
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                enemy.Update(dt);

                // --- QUÁI TẤN CÔNG TRỤ ---
                // Gọi hàm TryAttackNearbyTower (đã có trong Enemy.cs)
                var projectile = enemy.TryAttackNearbyTower(Towers, dt);
                if (projectile != null)
                {
                    EnemyProjectiles.Add(projectile);
                }

                // Kiểm tra chết
                if (enemy.Health <= 0)
                {
                    PlayerMoney += enemy.RewardGold;
                    ShowFloatingText($"+{enemy.RewardGold}", enemy.X, enemy.Y, Color.Gold);
                    Enemies.RemoveAt(i);
                    continue;
                }

                // Kiểm tra về đích
                if (!enemy.IsActive)
                {
                    PlayerLives--;
                    Enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateProjectiles(float dt)
        {
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                var p = Projectiles[i];
                p.Update(dt);
                if (!p.IsActive) Projectiles.RemoveAt(i);
            }
        }

        private void UpdateEnemyProjectiles(float dt)
        {
            for (int i = EnemyProjectiles.Count - 1; i >= 0; i--)
            {
                var ep = EnemyProjectiles[i];
                ep.Update(dt);
                if (!ep.IsActive) EnemyProjectiles.RemoveAt(i);
            }
        }

        private void UpdateEffects(float dt)
        {
            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                Effects[i].Update(dt);
                if (Effects[i].IsExpired) Effects.RemoveAt(i);
            }
        }

        // =========================================================
        // 4. RENDER (VẼ)
        // =========================================================
        public void Render(Graphics g)
        {
            // Vẽ theo lớp (Layer)
            foreach (var tower in Towers) tower.Render(g);
            foreach (var enemy in Enemies) enemy.Render(g);
            foreach (var p in Projectiles) p.Render(g);
            foreach (var ep in EnemyProjectiles) ep.Render(g); // Vẽ đạn quái
            foreach (var eff in Effects) eff.Render(g);        // Vẽ hiệu ứng
        }

        // =========================================================
        // 5. HELPERS (BUILDING & SPAWNING)
        // =========================================================

        private void SpawnEnemy(int typeId)
        {
            if (CurrentMapPath == null) return;
            Enemies.Add(new Enemy(CurrentMapPath, typeId));
        }

        public bool TryBuildTower(int x, int y)
        {
            if (SelectedTowerType == -1) return false;

            var stat = GameConfig.Towers[SelectedTowerType];
            if (PlayerMoney >= stat.Price)
            {
                PlayerMoney -= stat.Price;
                Tower newTower = new Tower(x, y, SelectedTowerType);
                Towers.Add(newTower);
                return true;
            }
            return false;
        }

        public bool CanPlaceTower(int x, int y)
        {
            // 1. Kiểm tra đè lên tháp khác
            foreach (var t in Towers)
            {
                if (Math.Abs(t.X - x) < 20 && Math.Abs(t.Y - y) < 20) return false;
            }

            // 2. Kiểm tra đè lên đường đi
            if (IsOnPath(x, y)) return false;

            // 3. Kiểm tra ngoài map (Với Map 800x600)
            if (x < 20 || x > 780 || y < 20 || y > 580) return false;

            return true;
        }

        private bool IsOnPath(float x, float y)
        {
            if (CurrentMapPath == null || CurrentMapPath.Count < 2) return false;

            float coreSize = 15f;
            RectangleF towerRect = new RectangleF(x - coreSize / 2, y - coreSize / 2, coreSize, coreSize);
            float pathRadius = 28f; // Đường rộng 60px

            for (int i = 0; i < CurrentMapPath.Count - 1; i++)
            {
                Point p1 = CurrentMapPath[i];
                Point p2 = CurrentMapPath[i + 1];

                float left = Math.Min(p1.X, p2.X) - pathRadius;
                float right = Math.Max(p1.X, p2.X) + pathRadius;
                float top = Math.Min(p1.Y, p2.Y) - pathRadius;
                float bottom = Math.Max(p1.Y, p2.Y) + pathRadius;

                RectangleF pathRect = new RectangleF(left, top, right - left, bottom - top);
                if (towerRect.IntersectsWith(pathRect)) return true;
            }
            return false;
        }

        // =========================================================
        // 6. EFFECT HELPERS (FIX CÁC LỖI THIẾU HÀM)
        // =========================================================

        // Tạo hiệu ứng nổ (dùng cho Missile/Cannon)
        // --- SỬA HÀM NÀY: THÊM THAM SỐ RADIUS ---
        public void CreateExplosion(float x, float y, float radius)
        {
            // Truyền radius vào hiệu ứng
            Effects.Add(new ExplosionEffect(x, y, radius));
        }

        // Tạo hiệu ứng băng (dùng cho Ice Tower)
        public void CreateIceEffect(float x, float y)
        {
            Effects.Add(new IceEffect(x, y));
        }

        // Tạo hiệu ứng trúng đạn nhỏ (dùng cho Sniper/Gun)
        public void CreateHitEffect(float x, float y)
        {
            Effects.Add(new HitEffect(x, y));
        }

        // Tạo chữ bay (Sửa nhận float x, float y)
        public void ShowFloatingText(string text, float x, float y, Color color)
        {
            Effects.Add(new FloatingText(text, x, y, color));
        }
    }
}