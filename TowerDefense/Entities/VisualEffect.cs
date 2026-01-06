using System.Drawing;

namespace TowerDefense.Entities
{
    // 1. Lớp cơ sở (Abstract Base Class)
    public abstract class VisualEffect
    {
        public bool IsExpired { get; protected set; } = false;
        public abstract void Update(float deltaTime);
        public abstract void Render(Graphics g);
    }

    // 2. Hiệu ứng Chữ bay (Floating Text)
    public class FloatingText : VisualEffect
    {
        private string _text;
        private float _x, _y;
        private Color _color;
        private float _lifeTime = 1.0f;
        // Sử dụng Font tĩnh để tối ưu bộ nhớ
        private static readonly Font _font = new Font("Arial", 10, FontStyle.Bold);

        // Constructor nhận float x, float y để khớp với GameManager
        public FloatingText(string text, float x, float y, Color color)
        {
            _text = text;
            _x = x;
            _y = y;
            _color = color;
        }

        public override void Update(float deltaTime)
        {
            _lifeTime -= deltaTime;
            if (_lifeTime <= 0) IsExpired = true;
            _y -= 30 * deltaTime; // Bay lên
        }

        public override void Render(Graphics g)
        {
            if (_lifeTime <= 0) return;

            // Tính độ trong suốt
            int alpha = (int)(255 * (_lifeTime / 1.0f));
            if (alpha < 0) alpha = 0; if (alpha > 255) alpha = 255;

            using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, _color)))
            {
                // Vẽ bóng đen
                using (SolidBrush shadow = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                {
                    g.DrawString(_text, _font, shadow, _x + 1, _y + 1);
                }
                g.DrawString(_text, _font, b, _x, _y);
            }
        }
    }

    // 3. Hiệu ứng Nổ (Explosion)
    // 3. Hiệu ứng Nổ (Explosion)
    public class ExplosionEffect : VisualEffect
    {
        private float _x, _y, _radius;
        private float _maxRadius; // Bán kính tối đa
        private float _timer = 0.3f; // Nổ nhanh trong 0.3s

        // --- SỬA CONSTRUCTOR ĐỂ NHẬN THÊM THAM SỐ RADIUS ---
        public ExplosionEffect(float x, float y, float radius)
        {
            _x = x;
            _y = y;
            _maxRadius = radius; // Lưu bán kính nổ truyền vào
            _radius = 5;         // Bắt đầu nhỏ
        }

        public override void Update(float deltaTime)
        {
            _timer -= deltaTime;

            // Hiệu ứng nở to dần ra đến MaxRadius
            if (_radius < _maxRadius)
                _radius += (_maxRadius * 5) * deltaTime;

            if (_timer <= 0) IsExpired = true;
        }

        public override void Render(Graphics g)
        {
            int alpha = (int)(255 * (_timer / 0.3f));
            if (alpha < 0) alpha = 0;

            // Vẽ vòng tròn nổ
            using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, Color.OrangeRed)))
                g.FillEllipse(b, _x - _radius, _y - _radius, _radius * 2, _radius * 2);

            // Vẽ viền vàng cho đẹp
            using (Pen p = new Pen(Color.FromArgb(alpha, Color.Yellow), 2))
                g.DrawEllipse(p, _x - _radius, _y - _radius, _radius * 2, _radius * 2);
        }
    }

    // 4. Hiệu ứng Băng (Ice)
    public class IceEffect : VisualEffect
    {
        private float _x, _y, _radius;
        private float _timer = 0.5f;

        public IceEffect(float x, float y) { _x = x; _y = y; _radius = 5; }

        public override void Update(float deltaTime)
        {
            _timer -= deltaTime;
            _radius += 50 * deltaTime;
            if (_timer <= 0) IsExpired = true;
        }

        public override void Render(Graphics g)
        {
            using (Pen p = new Pen(Color.Cyan, 2))
                g.DrawEllipse(p, _x - _radius, _y - _radius, _radius * 2, _radius * 2);
        }
    }

    // 5. Hiệu ứng Trúng đạn (Hit)
    public class HitEffect : VisualEffect
    {
        private float _x, _y;
        private float _timer = 0.2f;

        public HitEffect(float x, float y) { _x = x; _y = y; }

        public override void Update(float deltaTime)
        {
            _timer -= deltaTime;
            if (_timer <= 0) IsExpired = true;
        }

        public override void Render(Graphics g)
        {
            g.FillEllipse(Brushes.Yellow, _x - 3, _y - 3, 6, 6);
        }
    }
}