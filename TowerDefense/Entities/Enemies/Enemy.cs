using System;
using System.Drawing;
using System.Collections.Generic;
using TowerDefense.Components;
using TowerDefense.Managers;
using TowerDefense.Configs;
using TowerDefense.Entities.Towers;
using TowerDefense.Core;

namespace TowerDefense.Entities.Enemies
{
    public class Enemy : BaseEntity
    {
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public int RewardGold { get; set; }
        public int DamageToTower { get; private set; }
        public float AttackRange { get; private set; }
        public Color Color { get; set; }

        private MovementComponent _movement;
        private float _slowTimer = 0;
        private float _rotation = 0f;
        private PointF _lastPos;

        // Cooldown tấn công
        private float _attackCooldownTimer = 0f;
        private const float ATTACK_COOLDOWN = 1.5f;


        public Enemy(List<Point> path, int enemyTypeId)
        {
            if (enemyTypeId < 0 || enemyTypeId >= GameConfig.Enemies.Length) enemyTypeId = 0;
            var stat = GameConfig.Enemies[enemyTypeId];

            this.Name = stat.Name;
            this.MaxHealth = stat.MaxHealth;
            this.Health = stat.MaxHealth;
            this.RewardGold = stat.Reward;
            this.DamageToTower = stat.DamageToTower;
            this.AttackRange = stat.AttackRange;
            this.Color = stat.Color;

            this.Width = 48; this.Height = 48;
            _movement = new MovementComponent(path, stat.Speed);

            if (path != null && path.Count > 0)
            {
                PointF startPos = _movement.Update(new PointF(path[0].X, path[0].Y), 0);
                X = startPos.X; Y = startPos.Y;
                _lastPos = new PointF(X, Y);
            }
        }

        public override void Update(float deltaTime)
        {
            if (_slowTimer > 0)
            {
                _slowTimer -= deltaTime;
                if (_slowTimer <= 0) _movement.SetSpeedScale(1.0f);
            }

            if (_attackCooldownTimer > 0) _attackCooldownTimer -= deltaTime;

            // Nếu đang tấn công thì đứng lại, không di chuyển
            // (Logic này được gọi từ GameManager thông qua TryAttackNearbyTower)

            _lastPos = new PointF(X, Y);
            PointF newPos = _movement.Update(new PointF(X, Y), deltaTime);
            X = newPos.X; Y = newPos.Y;

            if (Math.Abs(X - _lastPos.X) > 0.1f || Math.Abs(Y - _lastPos.Y) > 0.1f)
                _rotation = (float)(Math.Atan2(Y - _lastPos.Y, X - _lastPos.X) * 180 / Math.PI);

            if (_movement.HasReachedEnd) IsActive = false;
        }

        // --- HÀM MỚI BỊ THIẾU ---
        public EnemyProjectile TryAttackNearbyTower(List<Tower> towers, float deltaTime)
        {
            if (DamageToTower <= 0 || AttackRange <= 0) return null;
            if (_attackCooldownTimer > 0) return null;

            Tower target = null;
            float closestDistSq = AttackRange * AttackRange;

            foreach (var tower in towers)
            {
                if (!tower.IsActive) continue;
                float dx = tower.X - X;
                float dy = tower.Y - Y;
                float distSq = dx * dx + dy * dy;

                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    target = tower;
                }
            }

            if (target != null)
            {
                _attackCooldownTimer = ATTACK_COOLDOWN;
                // Trả về viên đạn mới
                return new EnemyProjectile(X, Y, target, DamageToTower);
            }
            return null;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0) IsActive = false;
        }

        public void ApplySlow(float duration, float slowFactor)
        {
            _slowTimer = duration;
            _movement.SetSpeedScale(slowFactor);
        }

        public override void Render(Graphics g)
        {
            Image sprite = ResourceManager.GetImage(this.Name);
            if (sprite == null) sprite = ResourceManager.GetImage("Enemy");

            if (sprite != null)
            {
                var state = g.Save();
                g.TranslateTransform(X, Y);
                g.RotateTransform(_rotation);
                g.DrawImage(sprite, -Width / 2, -Height / 2, Width, Height);
                g.Restore(state);
            }
            else
            {
                using (SolidBrush b = new SolidBrush(this.Color))
                    g.FillEllipse(b, X - Width / 2, Y - Height / 2, Width, Height);
            }

            // Health bar
            float hpPercent = (float)Health / MaxHealth;
            g.FillRectangle(Brushes.Red, X - 16, Y - 24, 32, 4);
            g.FillRectangle(Brushes.Lime, X - 16, Y - 24, 32 * hpPercent, 4);
        }
    }
}