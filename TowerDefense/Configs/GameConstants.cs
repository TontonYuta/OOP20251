using System.Collections.Generic;
using System.Drawing;

namespace TowerDefense.Configs
{
    // =========================================================
    // 1. CÁC HẰNG SỐ HỆ THỐNG
    // =========================================================
    public static class GameConstants
    {
        public const int TILE_SIZE = 40;       // Kích thước ô lưới chuẩn
        public const int HALF_TILE = 20;       // Tâm ô lưới (40/2)
        public const float DEFAULT_GAME_SPEED = 1.0f;
    }

    // =========================================================
    // 2. CẤU TRÚC DỮ LIỆU
    // =========================================================

    public struct TowerStat
    {
        public string Name;
        public int Price;
        public int Damage;
        public float Range;
        public float ReloadTime;
        public int MaxHealth;
        public string ProjectileType;
        public Color Color;
        public string BulletImage; // Tên ảnh đạn
    }

    public struct EnemyStat
    {
        public string Name;
        public int MaxHealth;
        public float Speed;
        public int Reward;
        public int DamageToBase;
        public int DamageToTower;
        public float AttackRange;
        public bool CanFly;
        public Color Color;
    }

    // =========================================================
    // 3. DỮ LIỆU CẤU HÌNH (ĐÃ CÂN BẰNG)
    // =========================================================
    public static class GameConfig
    {
        // --- DANH SÁCH 10 LOẠI THÁP ---
        public static readonly TowerStat[] Towers = new TowerStat[]
        {
            // 1. ARCHER: Rẻ, ổn định đầu game. DPS ~31.
            new TowerStat { Name="Archer", Price=100, Damage=25, Range=200, ReloadTime=0.8f, MaxHealth=100, ProjectileType="Arrow", Color=Color.Blue, BulletImage="Arrow" },
            
            // 2. CANNON: Sát thương diện rộng (AoE), bắn chậm. DPS ~35 (nhưng trúng nhiều con).
            new TowerStat { Name="Cannon", Price=250, Damage=70, Range=160, ReloadTime=2.0f, MaxHealth=200, ProjectileType="Bomb", Color=Color.Black, BulletImage="Bomb" },
            
            // 3. SNIPER: Chuyên trị Boss/Quái trâu. Tầm cực xa. DPS ~100.
            new TowerStat { Name="Sniper", Price=500, Damage=250, Range=450, ReloadTime=2.5f, MaxHealth=80, ProjectileType="MBullet", Color=Color.ForestGreen, BulletImage="MBullet" },
            
            // 4. MINIGUN: Máy xay thịt. Tầm ngắn nhưng bắn siêu nhanh. DPS ~180.
            // Buff: Tăng damage từ 8 -> 18.
            new TowerStat { Name="Minigun", Price=650, Damage=18, Range=140, ReloadTime=0.1f, MaxHealth=150, ProjectileType="Balista", Color=Color.Gray, BulletImage="Balista" },
            
            // 5. ICE: Sát thương thấp nhưng làm chậm. Quan trọng về late game.
            new TowerStat { Name="Ice", Price=350, Damage=20, Range=180, ReloadTime=1.0f, MaxHealth=120, ProjectileType="IceBullet", Color=Color.Cyan, BulletImage="IBullet" },
            
            // 6. MAGIC: Xuyên giáp/Bắn nhanh vừa phải. DPS ~80.
            new TowerStat { Name="Magic", Price=700, Damage=120, Range=240, ReloadTime=1.5f, MaxHealth=100, ProjectileType="Poison", Color=Color.Purple, BulletImage="Poison" },
            
            // 7. BUNKER: "Cục thịt" để chặn quái, không bắn. (Nếu game bạn có logic chặn đường)
            new TowerStat { Name="Bunker", Price=150, Damage=0, Range=0, ReloadTime=100f, MaxHealth=2000, ProjectileType="Rock", Color=Color.DarkSlateGray, BulletImage="Rock" },
            
            // 8. FIRE: Bắn tầm trung, tốc độ trung bình. DPS ~50.
            new TowerStat { Name="Fire", Price=450, Damage=60, Range=170, ReloadTime=1.2f, MaxHealth=120, ProjectileType="FireBall", Color=Color.OrangeRed, BulletImage="FireBall" },
            
            // 9. ROCKET: Phiên bản nâng cấp của Cannon. Nổ to, bắn xa. DPS ~120 (AoE).
            new TowerStat { Name="Rocket", Price=1200, Damage=300, Range=350, ReloadTime=2.5f, MaxHealth=200, ProjectileType="jet", Color=Color.DarkRed, BulletImage="Jet" },
            
            // 10. GOD: Siêu cấp vũ trụ. DPS ~2000. Đắt xắt ra miếng.
            new TowerStat { Name="God", Price=5000, Damage=1000, Range=600, ReloadTime=0.5f, MaxHealth=1000, ProjectileType="GodBullet", Color=Color.Gold, BulletImage="GodBullet" },
        };

        // --- DANH SÁCH QUÁI (ĐÃ GIẢM SỨC MẠNH BOSS CUỐI ĐỂ KHẢ THI HƠN) ---
        public static readonly EnemyStat[] Enemies = new EnemyStat[]
        {
            // --- TIER 1: QUÁI YẾU (Wave 1-4) ---
            // Bee: Máu giấy, bay nhanh. Test độ chính xác của tháp.
            new EnemyStat { Name="Bee", MaxHealth=40, Speed=100, Reward=5, Color=Color.Green },
            new EnemyStat { Name="Bat", MaxHealth=50, Speed=120, Reward=6, Color=Color.Gray },
            new EnemyStat { Name="Cobra", MaxHealth=60, Speed=140, Reward=8, CanFly=true, Color=Color.Black },
            // Wolf: Trâu hơn chút, đi bộ.
            new EnemyStat { Name="Wolf", MaxHealth=120, Speed=90, Reward=12, DamageToTower=10, AttackRange=30, Color=Color.DarkGreen },
            new EnemyStat { Name="Goblin", MaxHealth=180, Speed=75, Reward=15, DamageToTower=15, AttackRange=30, Color=Color.White },

            // --- TIER 2: QUÁI TRUNG BÌNH (Wave 5-9) ---
            // Witch: Boss đầu game.
            new EnemyStat { Name="Witch", MaxHealth=500, Speed=60, Reward=40, DamageToTower=30, AttackRange=40, Color=Color.DarkOliveGreen },
            new EnemyStat { Name="Skeleton", MaxHealth=250, Speed=110, Reward=20, DamageToTower=20, AttackRange=30, Color=Color.Gray },
            new EnemyStat { Name="Zombie", MaxHealth=350, Speed=80, Reward=22, DamageToTower=25, AttackRange=30, Color=Color.Brown },
            new EnemyStat { Name="Magma", MaxHealth=400, Speed=50, Reward=25, Color=Color.LightBlue },
            // Orc: Damage to tower cao, cần diệt sớm.
            new EnemyStat { Name="Orc", MaxHealth=300, Speed=85, Reward=30, DamageToTower=60, AttackRange=100, Color=Color.Purple },

            // --- TIER 3: QUÁI MẠNH (Wave 10-14) ---
            // Golem: Tanker siêu cứng. Cần Sniper hoặc Magic.
            new EnemyStat { Name="Golem", MaxHealth=2000, Speed=40, Reward=80, DamageToTower=80, AttackRange=50, Color=Color.DarkCyan },
            new EnemyStat { Name="Gargoyle", MaxHealth=900, Speed=130, Reward=60, DamageToTower=50, AttackRange=40, CanFly=true, Color=Color.DarkSlateBlue },
            new EnemyStat { Name="Vampire", MaxHealth=1200, Speed=140, Reward=70, DamageToTower=50, AttackRange=40, Color=Color.Red },
            // Dragon: Mini-boss bay.
            new EnemyStat { Name="Dragon", MaxHealth=4000, Speed=45, Reward=150, DamageToTower=150, AttackRange=60, Color=Color.SandyBrown }, 
            // Spiderling: Chạy siêu nhanh (Zerg rush). Cần Minigun hoặc Ice.
            new EnemyStat { Name="Spiderling", MaxHealth=600, Speed=200, Reward=50, DamageToTower=40, AttackRange=30, Color=Color.Black }, 

            // --- TIER 4: BOSS & SIÊU QUÁI (Wave 15+) ---
            // Cyclops
            new EnemyStat { Name="Cyclops", MaxHealth=8000, Speed=50, Reward=300, DamageToTower=200, AttackRange=80, Color=Color.DarkOrange }, 
            // Treant: Máu rất trâu.
            new EnemyStat { Name="Treant", MaxHealth=15000, Speed=35, Reward=500, DamageToTower=150, AttackRange=100, Color=Color.DarkGreen }, 
            // Phoenix: Bay nhanh + Máu trâu. Cực nguy hiểm.
            new EnemyStat { Name="Phoenix", MaxHealth=7000, Speed=180, Reward=600, DamageToTower=120, AttackRange=150, CanFly=true, Color=Color.OrangeRed }, 
            // Titan: One-hit trụ.
            new EnemyStat { Name="Titan", MaxHealth=25000, Speed=30, Reward=1000, DamageToTower=800, AttackRange=80, Color=Color.DarkBlue }, 
            // FINAL BOSS: Dragon King. (Giảm từ 50k xuống 40k để khả thi hơn)
            new EnemyStat { Name="Dragon King", MaxHealth=40000, Speed=60, Reward=5000, DamageToTower=400, AttackRange=200, Color=Color.Crimson },
        };
    }
}