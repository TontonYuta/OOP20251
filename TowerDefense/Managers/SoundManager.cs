using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace TowerDefense.Managers
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundPlayer> _sounds = new Dictionary<string, SoundPlayer>();
        private static SoundPlayer _bgmPlayer;
        private static string _currentMusicName; // Lưu tên bài hát đang phát
        private static bool _isMusicPlaying = false;

        // Đường dẫn tới thư mục Sounds
        private static string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds");

        public static void LoadSounds()
        {
            // Fix lỗi đường dẫn nếu chạy Debug
            if (!Directory.Exists(_basePath))
            {
                string altPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Sounds");
                if (Directory.Exists(altPath)) _basePath = altPath;
            }

            Load("shoot", "shoot.wav");
            Load("build", "build.wav");
            Load("upgrade", "upgrade.wav");
            Load("win", "win.wav");
            Load("lose", "lose.wav");
            Load("click", "click.wav"); // [QUAN TRỌNG] Thêm tiếng click cho Menu
        }

        private static void Load(string key, string fileName)
        {
            try
            {
                string fullPath = Path.Combine(_basePath, fileName);
                if (File.Exists(fullPath))
                {
                    SoundPlayer player = new SoundPlayer(fullPath);
                    player.Load();
                    _sounds[key] = player;
                }
            }
            catch { }
        }

        public static void Play(string key)
        {
            if (_sounds.ContainsKey(key))
            {
                try { _sounds[key].Play(); } catch { }
            }
        }

        public static void PlayMusic(string fileName)
        {
            _currentMusicName = fileName;
            try
            {
                string path = Path.Combine(_basePath, fileName);
                if (File.Exists(path))
                {
                    if (_bgmPlayer != null) _bgmPlayer.Stop();
                    _bgmPlayer = new SoundPlayer(path);
                    _bgmPlayer.PlayLooping();
                    _isMusicPlaying = true;
                }
            }
            catch { }
        }

        public static void StopMusic()
        {
            if (_bgmPlayer != null)
            {
                _bgmPlayer.Stop();
                _isMusicPlaying = false;
            }
        }

        // --- [HÀM MỚI] THÊM VÀO ĐỂ SỬA LỖI ---
        public static void SetMusicVolume(float volume)
        {
            // Logic: Nếu volume <= 0 thì Tắt, ngược lại thì Bật
            if (volume <= 0)
            {
                StopMusic();
            }
            else
            {
                // Nếu đang tắt mà chỉnh volume lên -> Bật lại bài cũ
                if (!_isMusicPlaying && !string.IsNullOrEmpty(_currentMusicName))
                {
                    PlayMusic(_currentMusicName);
                }
            }
        }
    }
}