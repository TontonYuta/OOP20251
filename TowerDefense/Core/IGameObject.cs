using System.Drawing;

namespace TowerDefense.Core
{
    // Interface bắt buộc mọi vật thể phải có hàm Update và Render
    public interface IGameObject
    {
        void Update(float deltaTime);
        void Render(Graphics g);
        bool IsActive { get; set; } // Nếu false => Xóa khỏi game
    }
}