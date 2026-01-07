using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Entities.Towers;
using TowerDefense.Data;
using TowerDefense.Utils;

namespace TowerDefense.Forms.GameLevels
{
    // 'partial' giúp kết nối với file Input.cs và Render.cs
    public partial class GameLevel1 : Form
    {
        // =========================================================
        // 1. KHAI BÁO BIẾN (SHARED STATE)
        // =========================================================

        private readonly System.Diagnostics.Stopwatch _sw = new System.Diagnostics.Stopwatch();
        private long _lastMs = 0;

        // --- HỆ THỐNG ---
        private Timer _gameTimer;
        private bool _isPaused = false;

        // --- TỶ LỆ SCALE ---
        // (Nếu không dùng scale thì để 1.0f, code render sẽ tự bỏ qua)
        private float _scaleFactor = 1.0f;

        // --- VISUALS (Dùng bên Render.cs) ---
        private int _lastLives;
        private int _hurtTimer = 0;  // Hiệu ứng đỏ màn hình

        // --- GHOST BUILDING (Dùng bên Input.cs & Render.cs) ---
        private Point _ghostPos;       // Vị trí bóng ma (đã snap grid)
        private bool _isGhostValid;    // Vị trí có hợp lệ để xây không?
        private bool _isHoveringBoard; // Chuột có đang ở trong vùng bàn cờ không?

        // --- UI NÂNG CẤP ĐỘNG (Dùng bên Input.cs) ---
        private Button _btnCloseMenu; // Thêm nút đóng
        private Label _lblTowerInfo;
        private Button _btnUpgrade;
        private Button _btnSell;
        private Tower _selectedTower;
        private Rectangle _upgradeMenuRect;

        // =========================================================
        // 2. CONSTRUCTOR & KHỞI TẠO
        // =========================================================
        public GameLevel1(int levelId)
        {
            SoundManager.PlayMusic("battle_theme.wav");
            // Cấu hình Form
            this.Text = $"Tower Defense - Level {levelId}";

            // --- KÍCH THƯỚC CHUẨN MỚI (WIDESCREEN) ---
            // 800 Map + 200 Sidebar = 1000 Width
            // 600 Height (Vừa vặn màn hình Laptop)
            this.ClientSize = new Size(1000, 600);
            // ------------------------------------------

            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // A. Init UI Tĩnh (Từ file Designer.cs)
            InitializeComponent();

            // B. Init UI Động (Các hàm này nằm bên file GameLevel1.Input.cs)
            InitializeDynamicControls(); // Bảng nâng cấp
            InitializeTowerSelector();   // Danh sách tháp cuộn

            // C. Init Game Logic
            SoundManager.LoadSounds();
            GameManager.Instance.StartGame(levelId);
            _lastLives = GameManager.Instance.PlayerLives;

            // --- QUAN TRỌNG: Mặc định KHÔNG chọn súng nào (-1) ---
            SelectTower(-1);

            // D. Init Game Loop
            _gameTimer = new Timer();
            _gameTimer.Interval = 16; // ~60 FPS
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // E. Đăng ký sự kiện Input (Các hàm này nằm bên file Input.cs)
            this.MouseClick += OnBoardClick;
            this.MouseMove += OnMouseMove;
        }

        // Constructor mặc định (để tránh lỗi Designer)
        public GameLevel1() : this(1) { }

        // =========================================================
        // 3. GAME LOOP (TRÁI TIM CỦA GAME)
        // =========================================================
        // =========================================================
        // 3. GAME LOOP (TRÁI TIM CỦA GAME)
        // =========================================================
        private void GameLoop(object sender, EventArgs e)
        {
            if (_isPaused) return;

            // --- A. QUẢN LÝ THỜI GIAN (DeltaTime) ---
            if (!_sw.IsRunning)
            {
                _sw.Start();
                _lastMs = _sw.ElapsedMilliseconds;
            }

            long now = _sw.ElapsedMilliseconds;
            float dt = (now - _lastMs) / 1000f;
            _lastMs = now;

            if (dt > 0.05f) dt = 0.05f; // Chống lag giật

            // --- B. CẬP NHẬT LOGIC (GameManager) ---
            GameManager.Instance.Update(dt);

            // --- C. KIỂM TRA MẤT MÁU & HIỆU ỨNG HÀNH ĐỘNG ---
            if (GameManager.Instance.PlayerLives < _lastLives)
            {
                _hurtTimer = 10;
                _lastLives = GameManager.Instance.PlayerLives;
                SoundManager.Play("lose"); // Tiếng mất mạng
            }
            if (_hurtTimer > 0) _hurtTimer--;

            // --- D. KIỂM TRA ĐIỀU KIỆN KẾT THÚC (QUAN TRỌNG) ---

            // 1. Kiểm tra THUA TRẬN (Mạng về 0)
            if (GameManager.Instance.PlayerLives <= 0)
            {
                _gameTimer.Stop(); // Dừng game ngay lập tức
                _sw.Stop();
                ShowResultForm(false); // Hiện bảng Defeat
                return;
            }

            // 2. Kiểm tra CHIẾN THẮNG
            if (GameManager.Instance.IsVictory)
            {
                _gameTimer.Stop();
                _sw.Stop();
                ShowResultForm(true); // Hiện bảng Victory
                return;
            }

            // --- E. UI & RENDER ---
            UpdateWaveButtonState();
            this.Invalidate(); // Vẽ lại màn hình
        }
        // =========================================================
        // 4. XỬ LÝ KẾT QUẢ (THẮNG / THUA)
        // =========================================================
        private void ShowResultForm(bool isVictory)
        {
            // 1. Dừng Timer ngay lập tức để chặn quái đi tiếp (fix lỗi mạng âm)
            _gameTimer.Stop();
            _sw.Stop();

            // 2. Âm thanh thông báo
            if (isVictory) SoundManager.Play("win");
            else SoundManager.Play("lose");

            // 3. Hiện Form Kết Quả (ResultForm)
            using (var resultForm = new ResultForm(isVictory,
                GameManager.Instance.WaveMgr.CurrentWave,
                GameManager.Instance.PlayerMoney))
            {
                // Hiển thị dạng Dialog để chặn mọi thao tác phía sau
                if (resultForm.ShowDialog() == DialogResult.OK)
                {
                    // --- LƯU TRỮ DỮ LIỆU (CHỈ LƯU TẠI ĐÂY) ---
                    try
                    {
                        // Lấy thông tin từ GameManager
                        int currentLevel = GameManager.Instance.LevelMgr.CurrentLevelId;
                        int currentWave = GameManager.Instance.WaveMgr.CurrentWave;
                        string pName = string.IsNullOrWhiteSpace(resultForm.PlayerName) ? "Unknown" : resultForm.PlayerName;

                        // Tính điểm tổng hợp
                        int finalScore = (GameManager.Instance.PlayerMoney / 10) + (currentWave * 100);

                        // Lưu vào Ranking
                        HighScoreManager.SaveScore(pName, finalScore);

                        // FIX LỖI MAP AUTO 1: Truyền currentLevel vào đây
                        HistoryManager.SaveLog(isVictory, currentWave, currentLevel);
                    }
                    catch { /* Bỏ qua nếu lỗi file lưu trữ */ }

                    // --- XỬ LÝ SAU KẾT QUẢ ---
                    if (resultForm.IsRetry)
                    {
                        // A. Reset Logic trong GameManager
                        GameManager.Instance.StartGame(GameManager.Instance.LevelMgr.CurrentLevelId);

                        // B. FIX LỖI AUTO WAVE: Ép trạng thái về OFF và cập nhật UI
                        GameManager.Instance.IsAutoWave = false;
                        UpdateWaveButtonState(); // Hàm này sẽ đổi màu nút về Next Wave thủ công

                        // C. Reset trạng thái UI Level
                        _lastLives = GameManager.Instance.PlayerLives;
                        _hurtTimer = 0;
                        SelectTower(-1); // Bỏ chọn tháp đang cầm

                        // D. Khởi động lại đồng hồ và vòng lặp
                        _sw.Reset();
                        _lastMs = 0;
                        _gameTimer.Start();
                    }
                    else
                    {
                        // Đóng Form game để về Menu chính
                        this.Close();
                    }
                }
            }
        }

        // Logic điều khiển nút Start Wave (Tắt/Bật/Auto)
        private void UpdateWaveButtonState()
        {
            var waveMgr = GameManager.Instance.WaveMgr;

            // Điều kiện: Wave đã ngừng sinh quái VÀ Đã diệt sạch quái
            bool isWaveClear = !waveMgr.IsWaveRunning && GameManager.Instance.Enemies.Count == 0;

            if (isWaveClear)
            {
                if (GameManager.Instance.IsAutoWave)
                {
                    // Chế độ Auto: Disable nút, hiện text chờ
                    if (_btnStartWave.Enabled)
                    {
                        _btnStartWave.Enabled = false;
                        _btnStartWave.Text = "AUTO...";
                        _btnStartWave.BackColor = Color.Gray;
                    }
                }
                else
                {
                    // Chế độ Thủ công: Bật nút để người chơi bấm
                    if (!_btnStartWave.Enabled)
                    {
                        _btnStartWave.Enabled = true;
                        _btnStartWave.Text = "NEXT WAVE >>";
                        _btnStartWave.BackColor = Color.LightGreen;
                    }
                }
            }
            else
            {
                // Wave đang chạy
                if (_btnStartWave.Enabled)
                {
                    _btnStartWave.Enabled = false;
                    _btnStartWave.Text = "RUNNING...";
                    _btnStartWave.BackColor = Color.Orange;
                }
            }
        }
    }
}