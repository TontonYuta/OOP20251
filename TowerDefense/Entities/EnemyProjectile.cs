using System;
using System.Drawing;
using TowerDefense.Entities.Towers;
using TowerDefense.Managers;

namespace TowerDefense.Entities
{
    public class EnemyProjectile
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public Tower TargetTower { get; private set; } // Mục tiêu là Trụ
        public bool IsActive { get; private set; } = true;

        private float _speed = 250f; // Tốc độ bay chậm hơn đạn trụ một chút cho dễ nhìn
        private int _damage;

        public EnemyProjectile(float startX, float startY, Tower target, int damage)
        {
            this.X = startX;
            this.Y = startY;
            this.TargetTower = target;
            this._damage = damage;
        }

        public void Update(float deltaTime)
        {
            // Nếu trụ mục tiêu đã bị bán hoặc biến mất thì đạn cũng mất
            if (TargetTower == null || !TargetTower.IsActive)
            {
                IsActive = false;
                return;
            }

            // Tính hướng bay đến trụ
            float dx = TargetTower.X - this.X;
            float dy = TargetTower.Y - this.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            // Nếu đã chạm mục tiêu (khoảng cách < 5 pixel)
            if (distance < 5f)
            {
                HitTarget();
                return;
            }

            // Di chuyển đạn
            float moveStep = _speed * deltaTime;
            this.X += (dx / distance) * moveStep;
            this.Y += (dy / distance) * moveStep;
        }

        private void HitTarget()
        {
            IsActive = false;
            // Tạm thời chỉ tạo hiệu ứng hình ảnh.
            // Sau này nếu Trụ có máu (HP), ta sẽ trừ máu trụ ở đây: TargetTower.TakeDamage(_damage);

            // Hiệu ứng nổ nhỏ tại vị trí trụ (Dùng tạm floating text màu đỏ)
            GameManager.Instance.ShowFloatingText("BOOM!", (int)TargetTower.X, (int)TargetTower.Y, Color.DarkRed);
        }

        public void Render(Graphics g)
        {
            // Vẽ viên đạn là một quả cầu lửa màu đỏ cam
            using (SolidBrush b = new SolidBrush(Color.OrangeRed))
            {
                g.FillEllipse(b, X - 6, Y - 6, 12, 12);
            }
            using (Pen p = new Pen(Color.Yellow, 2))
            {
                g.DrawEllipse(p, X - 6, Y - 6, 12, 12);
            }
        }
    }
}