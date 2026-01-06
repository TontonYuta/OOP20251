using System.Drawing;

namespace TowerDefense.Core
{
    public abstract class BaseEntity : IGameObject
    {
        // Tọa độ thực (float để di chuyển mượt)
        public float X { get; set; }
        public float Y { get; set; }

        // Kích thước
        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;

        // Trạng thái tồn tại
        public bool IsActive { get; set; } = true;

        // Hình chữ nhật bao quanh (Dùng để xử lý va chạm)
        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        // Các hàm trừu tượng (Con phải tự viết code cho hàm này)
        public abstract void Update(float deltaTime);
        public abstract void Render(Graphics g);

        // Hàm tiện ích lấy tâm vật thể
        public PointF Center => new PointF(X + Width / 2, Y + Height / 2);
    }
}