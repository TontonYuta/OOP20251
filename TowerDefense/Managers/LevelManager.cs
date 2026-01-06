using System.Collections.Generic;
using System.Drawing;
using TowerDefense.Configs;

namespace TowerDefense.Managers
{
    public class LevelManager
    {
        public int CurrentLevelId { get; private set; } = 1;
        public int MaxWaves { get; private set; } = 10;
        public string Theme { get; private set; } = "Grass";

        // Hàm G(i): Trả về tọa độ tâm của ô lưới thứ i
        // Lưới 40x40. Tâm ô = (i * 40) + 20
        private int G(int index)
        {
            return (index * GameConstants.TILE_SIZE) + GameConstants.HALF_TILE;
        }

        public List<Point> LoadLevelPath(int levelId)
        {
            CurrentLevelId = levelId;
            var path = new List<Point>();

            // Cấu hình số Wave tăng dần: Level 1 (5 wave) -> Level 10 (14 wave)
            MaxWaves = 4 + levelId;

            // Vùng chơi Game: X(0-800), Y(0-600)
            // Tọa độ âm (-40) hoặc quá khổ (840) để quái đi từ ngoài màn hình vào

            switch (levelId)
            {
                // =========================================================
                // THEME: GRASS (ĐỒNG CỎ) - DỄ
                // =========================================================
                case 1: // MAP 1: Đường thẳng (The Line)
                    Theme = "Grass";
                    path.Add(new Point(-40, G(7)));   // Vào từ trái (Hàng 7)
                    path.Add(new Point(840, G(7)));   // Ra bên phải
                    break;

                case 2: // MAP 2: Góc Vuông (The Corner)
                    Theme = "Grass";
                    path.Add(new Point(G(4), -40));   // Vào từ trên (Cột 4)
                    path.Add(new Point(G(4), G(10))); // Xuống hàng 10
                    path.Add(new Point(840, G(10)));  // Rẽ phải ra ngoài
                    break;

                case 3: // MAP 3: Chữ S (The Snake)
                    Theme = "Grass";
                    path.Add(new Point(-40, G(3)));   // Vào trái hàng 3
                    path.Add(new Point(G(15), G(3))); // Đi ngang
                    path.Add(new Point(G(15), G(10)));// Xuống hàng 10
                    path.Add(new Point(-40, G(10)));  // Quay lại trái ra ngoài
                    break;

                // =========================================================
                // THEME: SAND (SA MẠC) - TRUNG BÌNH
                // =========================================================
                case 4: // MAP 4: Chữ U Lớn (The Horseshoe)
                    Theme = "Sand";
                    path.Add(new Point(G(2), -40));    // Vào trên cột 2
                    path.Add(new Point(G(2), G(12)));  // Xuống sâu hàng 12
                    path.Add(new Point(G(17), G(12))); // Sang phải cột 17
                    path.Add(new Point(G(17), -40));   // Lên trên ra ngoài
                    break;

                case 5: // MAP 5: Ziczac Ngang (The Zipper)
                    Theme = "Sand";
                    path.Add(new Point(-40, G(2)));    // Vào trái hàng 2
                    path.Add(new Point(G(4), G(2)));
                    path.Add(new Point(G(4), G(11)));  // Xuống hàng 11
                    path.Add(new Point(G(9), G(11)));
                    path.Add(new Point(G(9), G(2)));   // Lên hàng 2
                    path.Add(new Point(G(14), G(2)));
                    path.Add(new Point(G(14), G(11))); // Xuống hàng 11
                    path.Add(new Point(G(19), G(11))); // Sang phải
                    path.Add(new Point(G(19), 640));   // Ra dưới đáy
                    break;

                case 6: // MAP 6: Xoắn Ốc (The Spiral)
                    Theme = "Sand";
                    path.Add(new Point(-40, G(1)));    // Vào trái hàng 1
                    path.Add(new Point(G(18), G(1)));  // Sang phải cùng
                    path.Add(new Point(G(18), G(12))); // Xuống dưới cùng
                    path.Add(new Point(G(2), G(12)));  // Sang trái cùng
                    path.Add(new Point(G(2), G(4)));   // Lên giữa
                    path.Add(new Point(G(14), G(4)));  // Sang phải giữa
                    path.Add(new Point(G(14), G(8)));  // Kết thúc ở tâm xoắn ốc
                    break;

                // =========================================================
                // THEME: SNOW (TUYẾT) - KHÓ
                // =========================================================
                case 7: // MAP 7: Ziczac Dọc (Vertical Zigzag)
                    Theme = "Snow";
                    path.Add(new Point(G(2), -40));    // Vào trên cột 2
                    path.Add(new Point(G(2), G(12)));  // Xuống
                    path.Add(new Point(G(6), G(12)));  // Sang phải
                    path.Add(new Point(G(6), G(2)));   // Lên
                    path.Add(new Point(G(10), G(2)));  // Sang phải
                    path.Add(new Point(G(10), G(12))); // Xuống
                    path.Add(new Point(G(14), G(12))); // Sang phải
                    path.Add(new Point(G(14), -40));   // Ra trên
                    break;

                case 8: // MAP 8: Hai Cổng (The Cross)
                    Theme = "Snow";
                    // Đi chéo màn hình bằng các bậc thang
                    path.Add(new Point(G(0), -40));
                    path.Add(new Point(G(0), G(2)));
                    path.Add(new Point(G(4), G(2)));
                    path.Add(new Point(G(4), G(6)));
                    path.Add(new Point(G(9), G(6)));
                    path.Add(new Point(G(9), G(10)));
                    path.Add(new Point(G(14), G(10)));
                    path.Add(new Point(G(14), G(14)));
                    path.Add(new Point(840, G(14)));
                    break;

                // =========================================================
                // THEME: LAVA (NHAM THẠCH) - ĐỊA NGỤC
                // =========================================================
                case 9: // MAP 9: Đồng Hồ Cát (Hourglass)
                    Theme = "Stone"; // Hoặc Lava
                    path.Add(new Point(-40, G(1)));    // Vào trái trên
                    path.Add(new Point(G(18), G(1)));  // Sang phải trên
                    path.Add(new Point(G(18), G(5)));  // Xuống giữa
                    path.Add(new Point(G(2), G(5)));   // Sang trái giữa
                    path.Add(new Point(G(2), G(12)));  // Xuống dưới
                    path.Add(new Point(840, G(12)));   // Ra phải dưới
                    break;

                case 10: // MAP 10: ĐƯỜNG DÀI NHẤT (The Maze)
                    Theme = "Lava";
                    MaxWaves = 20; // Boss cuối

                    // Đi kín màn hình
                    path.Add(new Point(G(1), -40));    // Vào trên cột 1
                    path.Add(new Point(G(1), G(13)));  // Xuống đáy
                    path.Add(new Point(G(5), G(13)));  // Sang phải
                    path.Add(new Point(G(5), G(1)));   // Lên đỉnh
                    path.Add(new Point(G(9), G(1)));   // Sang phải
                    path.Add(new Point(G(9), G(13)));  // Xuống đáy
                    path.Add(new Point(G(13), G(13))); // Sang phải
                    path.Add(new Point(G(13), G(1)));  // Lên đỉnh
                    path.Add(new Point(G(17), G(1)));  // Sang phải
                    path.Add(new Point(G(17), 640));   // Ra dưới cùng
                    break;

                default: // Fallback map
                    path.Add(new Point(-40, G(7)));
                    path.Add(new Point(840, G(7)));
                    break;
            }

            return path;
        }
    }
}