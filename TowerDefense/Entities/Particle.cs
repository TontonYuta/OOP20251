using System;
using System.Drawing;
using TowerDefense.Core;

namespace TowerDefense.Entities
{
    public class Particle : IGameObject
    {
        public float X, Y;
        public float Size;
        public Color Color;
        public float SpeedX, SpeedY;
        public float LifeTime; // Thời gian tồn tại (giây)
        public bool IsActive { get; set; } = true;

        private float _maxLifeTime;

        public Particle(float x, float y, Color color, float size, float speed, float angle, float lifeTime)
        {
            X = x; Y = y;
            Color = color;
            Size = size;
            LifeTime = lifeTime;
            _maxLifeTime = lifeTime;

            // Tính vector bay dựa trên góc (Angle)
            SpeedX = (float)(Math.Cos(angle) * speed);
            SpeedY = (float)(Math.Sin(angle) * speed);
        }

        public void Update(float deltaTime)
        {
            LifeTime -= deltaTime;
            if (LifeTime <= 0)
            {
                IsActive = false;
                return;
            }

            // Di chuyển
            X += SpeedX * deltaTime;
            Y += SpeedY * deltaTime;

            // Hiệu ứng vật lý giả (Ma sát & Trọng lực)
            SpeedX *= 0.95f; // Chậm dần
            SpeedY *= 0.95f;

            // Size nhỏ dần
            Size *= 0.95f;
        }

        public void Render(Graphics g)
        {
            if (Size < 1) return;

            // Vẽ hạt (Mờ dần theo thời gian nếu muốn, nhưng SolidBrush cho nhanh)
            int alpha = (int)((LifeTime / _maxLifeTime) * 255);
            if (alpha > 255) alpha = 255;
            if (alpha < 0) alpha = 0;

            using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, Color)))
            {
                g.FillEllipse(b, X - Size / 2, Y - Size / 2, Size, Size);
            }
        }
    }
}