using System;
using System.Collections.Generic;
using System.Drawing;

namespace TowerDefense.Components
{
    public class MovementComponent
    {
        // Danh sách điểm mốc đường đi
        private List<Point> _pathPoints;
        private int _currentPointIndex = 0;

        // Tốc độ
        private float _baseSpeed;    // Tốc độ gốc
        private float _currentSpeed; // Tốc độ hiện tại (có thể bị làm chậm)

        // --- BIẾN ĐỘ LỆCH (LÀM QUÁI ĐI LỘN XỘN) ---
        private PointF _laneOffset;
        private static Random _rnd = new Random(); // Biến Random dùng chung

        // Trạng thái về đích
        public bool HasReachedEnd { get; private set; } = false;

        // Constructor
        public MovementComponent(List<Point> path, float speed)
        {
            _pathPoints = path;
            _baseSpeed = speed;
            _currentSpeed = speed;

            // --- TẠO ĐỘ LỆCH NGẪU NHIÊN KHI SINH RA ---
            // Đường vẽ rộng 60px (Bán kính 30px).
            // Ta cho quái lệch ngẫu nhiên trong khoảng -25 đến +25 pixel
            // để chúng đi thành bầy đàn nhưng không bị lòi ra khỏi mép đường.
            float offsetX = _rnd.Next(-25, 25);
            float offsetY = _rnd.Next(-25, 25);
            _laneOffset = new PointF(offsetX, offsetY);
        }

        // Hàm chỉnh tốc độ (Dùng cho hiệu ứng Slow/Freeze)
        // scale = 1.0 (Bình thường), 0.5 (Chậm 50%), 0.0 (Đứng yên)
        public void SetSpeedScale(float scale)
        {
            _currentSpeed = _baseSpeed * scale;
        }

        // Hàm cập nhật vị trí mỗi khung hình
        public PointF Update(PointF currentPos, float deltaTime)
        {
            if (_pathPoints == null || _pathPoints.Count == 0 || HasReachedEnd)
                return currentPos;

            // 1. Lấy điểm đến gốc từ danh sách Path
            Point rawTarget = _pathPoints[_currentPointIndex];

            // 2. Cộng thêm độ lệch để ra điểm đến thực tế của riêng con quái này
            // (Mục tiêu = Tâm đường + Độ lệch riêng)
            PointF target = new PointF(rawTarget.X + _laneOffset.X, rawTarget.Y + _laneOffset.Y);

            // 3. Tính toán khoảng cách tới mục tiêu
            float dx = target.X - currentPos.X;
            float dy = target.Y - currentPos.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            // 4. Kiểm tra xem đã tới điểm mốc chưa
            if (distance < 5f) // Ngưỡng chấp nhận là 5px
            {
                _currentPointIndex++;

                // Nếu hết điểm để đi -> Đã về đích
                if (_currentPointIndex >= _pathPoints.Count)
                {
                    HasReachedEnd = true;
                }

                // Trả về vị trí hiện tại để tránh bị giật khung hình khi chuyển điểm
                return currentPos;
            }

            // 5. Di chuyển về phía mục tiêu (Vector Normalization)
            float moveX = (dx / distance) * _currentSpeed * deltaTime;
            float moveY = (dy / distance) * _currentSpeed * deltaTime;

            return new PointF(currentPos.X + moveX, currentPos.Y + moveY);
        }
    }
}