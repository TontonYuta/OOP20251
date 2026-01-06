using System.Collections.Generic;
using System.Drawing;
using TowerDefense.Entities.Enemies;

namespace TowerDefense.Components
{
    public class CombatComponent
    {
        private float _range;
        private float _reloadTime;
        private float _currentCooldown = 0;

        // Cho phép đọc Range từ bên ngoài (để vẽ vòng tròn tầm bắn)
        public float Range => _range;

        public CombatComponent(float range, float reloadTime)
        {
            _range = range;
            _reloadTime = reloadTime;
        }

        public void Update(float deltaTime)
        {
            if (_currentCooldown > 0) _currentCooldown -= deltaTime;
        }

        public bool CanShoot() => _currentCooldown <= 0;

        public void ResetCooldown() => _currentCooldown = _reloadTime;

        // --- HÀM TÌM MỤC TIÊU (Trả về Enemy object) ---
        public Enemy FindTarget(List<Enemy> enemies, PointF shooterPos)
        {
            Enemy bestTarget = null;
            float minDistanceSqr = float.MaxValue; // Tìm con gần nhất (hoặc thay đổi logic tùy thích)

            foreach (var enemy in enemies)
            {
                if (!enemy.IsActive) continue;

                float dx = enemy.X - shooterPos.X;
                float dy = enemy.Y - shooterPos.Y;
                float distSqr = dx * dx + dy * dy;

                // Kiểm tra trong tầm bắn
                if (distSqr <= _range * _range)
                {
                    // Logic ưu tiên: Bắn con gần tháp nhất
                    if (distSqr < minDistanceSqr)
                    {
                        minDistanceSqr = distSqr;
                        bestTarget = enemy;
                    }

                    // Logic thay thế (Bắn con đi xa nhất trên map):
                    // return enemy; // Trả về con đầu tiên trong list (thường là con ra trước)
                }
            }
            return bestTarget;
        }
    }
}