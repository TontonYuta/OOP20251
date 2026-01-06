using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TowerDefense.Managers
{
    public static class ResourceManager
    {
        public static Dictionary<string, Image> Images = new Dictionary<string, Image>();

        // Đường dẫn tương đối đến thư mục Assets
        // Lưu ý: Đảm bảo thư mục Assets nằm đúng vị trí so với file .exe khi chạy
        private static string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Images");

        public static void LoadResources()
        {
            // 1. Load thủ công một số ảnh cơ bản nếu cần (ví dụ ảnh nền)
            LoadImage("Grass", "grass.png");

            // 2. TỰ ĐỘNG LOAD TẤT CẢ ẢNH .PNG TRONG THƯ MỤC
            if (Directory.Exists(_basePath))
            {
                string[] files = Directory.GetFiles(_basePath, "*.png");
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    // Key sẽ là tên file không đuôi (VD: "IBullet.png" -> Key: "IBullet")
                    string key = Path.GetFileNameWithoutExtension(file);

                    // Chỉ load nếu chưa có (tránh duplicate)
                    if (!Images.ContainsKey(key))
                    {
                        LoadImage(key, fileName);
                    }
                }
            }
        }

        private static void LoadImage(string key, string fileName)
        {
            try
            {
                string fullPath = Path.Combine(_basePath, fileName);
                if (File.Exists(fullPath))
                {
                    Images[key] = Image.FromFile(fullPath);
                }
            }
            catch { }
        }

        public static Image GetImage(string key)
        {
            if (Images.ContainsKey(key)) return Images[key];
            return null;
        }
    }
}