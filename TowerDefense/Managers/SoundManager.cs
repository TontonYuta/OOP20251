using System;
using System.Collections.Generic;
using System.IO;
using System.Media; // Cần thiết để phát âm thanh .wav

namespace TowerDefense.Managers
{
    public static class SoundManager
    {
        // Dictionary lưu các file âm thanh đã load
        private static Dictionary<string, SoundPlayer> _sounds = new Dictionary<string, SoundPlayer>();

        // Đường dẫn tới thư mục Sounds
        private static string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Sounds");

        public static void LoadSounds()
        {
            // Hãy tạo các file .wav trống hoặc copy file thật vào thư mục Assets/Sounds
            // Tên file phải khớp chính xác
            Load("shoot", "shoot.wav");      // Tiếng bắn
            Load("build", "build.wav");      // Tiếng xây tháp
            Load("upgrade", "upgrade.wav");  // Tiếng nâng cấp
            Load("win", "win.wav");          // Tiếng thắng wave
            Load("lose", "lose.wav");        // Tiếng thua
        }

        private static void Load(string key, string fileName)
        {
            try
            {
                string fullPath = Path.Combine(_basePath, fileName);
                if (File.Exists(fullPath))
                {
                    SoundPlayer player = new SoundPlayer(fullPath);
                    player.Load(); // Load trước vào RAM
                    _sounds[key] = player;
                }
            }
            catch { /* Bỏ qua lỗi nếu không tìm thấy file */ }
        }

        public static void Play(string key)
        {
            if (_sounds.ContainsKey(key))
            {
                // Play(): Chạy bất đồng bộ (không làm đơ game)
                // PlaySync(): Chặn game lại để phát (không dùng cái này trong game loop)
                try
                {
                    _sounds[key].Play();
                }
                catch { }
            }
        }

        // Thêm biến riêng cho nhạc nền
        private static SoundPlayer _bgmPlayer;

        public static void PlayMusic(string fileName)
        {
            try
            {
                string path = Path.Combine(_basePath, fileName);
                if (File.Exists(path))
                {
                    if (_bgmPlayer != null) _bgmPlayer.Stop(); // Dừng bài cũ

                    _bgmPlayer = new SoundPlayer(path);
                    _bgmPlayer.PlayLooping(); // Phát lặp lại liên tục
                }
            }
            catch { }
        }

        public static void StopMusic()
        {
            if (_bgmPlayer != null) _bgmPlayer.Stop();
        }
    }
}