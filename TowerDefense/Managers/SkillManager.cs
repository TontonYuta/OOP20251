using System.Collections.Generic;
using TowerDefense.Entities.Enemies;

namespace TowerDefense.Managers
{
    public class SkillManager
    {
        // Thời gian hồi chiêu (Cooldown)
        public float MeteorCooldown { get; private set; } = 0;
        public float FreezeCooldown { get; private set; } = 0;

        // Cấu hình
        private const float METEOR_MAX_CD = 5.0f; // 5 giây hồi
        private const float FREEZE_MAX_CD = 10.0f; // 10 giây hồi

        public void Update(float deltaTime)
        {
            if (MeteorCooldown > 0) MeteorCooldown -= deltaTime;
            if (FreezeCooldown > 0) FreezeCooldown -= deltaTime;
        }

        // Kỹ năng 1: Thiên thạch (Gây 50 damage lên tất cả quái)
        public bool CastMeteor()
        {
            if (MeteorCooldown > 0) return false; // Chưa hồi xong

            var enemies = GameManager.Instance.Enemies;
            // Duyệt ngược để xóa an toàn nếu quái chết
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].TakeDamage(50);
            }

            MeteorCooldown = METEOR_MAX_CD; // Kích hoạt hồi chiêu
            return true;
        }

        // Kỹ năng 2: Đóng băng (Làm chậm 50% trong 3 giây)
        public bool CastFreeze()
        {
            if (FreezeCooldown > 0) return false;

            var enemies = GameManager.Instance.Enemies;
            foreach (var enemy in enemies)
            {
                enemy.ApplySlow(3.0f, 0.5f); // Chậm 3 giây, tốc độ còn 50%
            }

            FreezeCooldown = FREEZE_MAX_CD;
            return true;
        }
    }
}