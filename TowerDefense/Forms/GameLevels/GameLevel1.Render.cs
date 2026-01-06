using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging; // Cần thiết cho ColorMatrix
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Configs;
using TowerDefense.Utils;

namespace TowerDefense.Forms.GameLevels
{
    public partial class GameLevel1
    {
        // =========================================================
        // 1. TÀI NGUYÊN TĨNH (STATIC RESOURCES - TỐI ƯU FPS)
        // =========================================================
        private static readonly Font _hudFont = new Font("Segoe UI", 12, FontStyle.Bold);
        private static readonly Font _startFont = new Font("Arial", 8, FontStyle.Bold);
        private static readonly Font _uiTitleFont = new Font("Arial", 7, FontStyle.Regular);

        private static readonly SolidBrush _topBarBrush = new SolidBrush(Color.FromArgb(220, 20, 20, 30));
        private static readonly SolidBrush _bottomUiBrush = new SolidBrush(Color.FromArgb(255, 40, 40, 50));
        private static readonly SolidBrush _rangeFillBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
        private static readonly SolidBrush _redScreenBrush = new SolidBrush(Color.FromArgb(100, 255, 0, 0));

        private static readonly Pen _rangePen = new Pen(Color.White, 2) { DashStyle = DashStyle.Dash };
        private static readonly Pen _validPen = new Pen(Color.LightGreen, 2) { DashStyle = DashStyle.Dash };
        private static readonly Pen _invalidPen = new Pen(Color.Red, 2) { DashStyle = DashStyle.Dash };
        private static readonly Pen _pathPen = new Pen(Color.FromArgb(180, 210, 180, 140), 60) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round }; // Đường rộng 60px
        private static readonly Pen _separatorPen = new Pen(Color.Gold, 4);
        private static readonly Pen _uiBorderPen = new Pen(Color.Gray, 1);

        private Bitmap _cachedBackgroundLayer;

        // =========================================================
        // 2. KHỞI TẠO CACHE NỀN
        // =========================================================
        private void InitBackgroundCache()
        {
            if (_cachedBackgroundLayer != null) _cachedBackgroundLayer.Dispose();

            // Tạo ảnh kích thước lớn bằng ClientSize (để an toàn)
            int w = Math.Max(this.ClientSize.Width, 800);
            int h = Math.Max(this.ClientSize.Height, 760);
            _cachedBackgroundLayer = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(_cachedBackgroundLayer))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // A. Vẽ Nền Map (Chỉ vẽ vùng 0-600)
                string theme = GameManager.Instance.LevelMgr.Theme;
                Image bg = ResourceManager.GetImage(theme);
                if (bg != null) g.DrawImage(bg, 0, 0, 800, 600);
                else
                {
                    Color bgColor = Color.LightGreen;
                    if (theme == "Sand") bgColor = Color.Khaki;
                    if (theme == "Snow") bgColor = Color.WhiteSmoke;
                    if (theme == "Lava") bgColor = Color.FromArgb(50, 20, 20);
                    using (SolidBrush b = new SolidBrush(bgColor)) g.FillRectangle(b, 0, 0, 800, 600);
                }

                // B. Vẽ Đường đi
                var path = GameManager.Instance.CurrentMapPath;
                if (path != null && path.Count > 1)
                {
                    g.DrawLines(_pathPen, path.ToArray());

                    // Vẽ điểm Đầu (Spawn)
                    Point start = path[0];
                    int sx = Math.Max(25, Math.Min(775, start.X));
                    int sy = Math.Max(25, Math.Min(575, start.Y));
                    g.FillEllipse(Brushes.DarkSlateGray, sx - 25, sy - 25, 50, 50);
                    g.DrawString("SPAWN", _startFont, Brushes.White, sx - 22, sy - 6);

                    // Vẽ điểm Cuối (Base)
                    Point end = path[path.Count - 1];
                    int ex = Math.Max(32, Math.Min(768, end.X));
                    int ey = Math.Max(32, Math.Min(568, end.Y));

                    Image baseImg = ResourceManager.GetImage("Base");
                    if (baseImg != null) g.DrawImage(baseImg, ex - 32, ey - 32, 64, 64);
                    else
                    {
                        g.FillRectangle(Brushes.Purple, ex - 30, ey - 30, 60, 60);
                        g.DrawString("BASE", _startFont, Brushes.White, ex - 18, ey - 6);
                    }
                }
            }
        }

        // =========================================================
        // 3. HÀM VẼ CHÍNH (ON PAINT)
        // =========================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // --- CẤU HÌNH TĂNG TỐC ---
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.SmoothingMode = SmoothingMode.None;

            // Lưu trạng thái gốc (để reset sau khi scale)
            var originalState = g.Save();

            // Áp dụng Scale cho MAP (Nếu có)
            if (_scaleFactor != 1.0f) g.ScaleTransform(_scaleFactor, _scaleFactor);

            // 1. Vẽ Nền từ Cache
            if (_cachedBackgroundLayer == null) InitBackgroundCache();
            g.DrawImageUnscaled(_cachedBackgroundLayer, 0, 0);

            // 2. Vẽ Thực thể
            GameManager.Instance.Render(g);

            // 3. Vẽ Tầm bắn
            if (_selectedTower != null && _selectedTower.IsActive)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                float r = _selectedTower.Range;
                g.FillEllipse(_rangeFillBrush, _selectedTower.X - r, _selectedTower.Y - r, r * 2, r * 2);
                g.DrawEllipse(_rangePen, _selectedTower.X - r, _selectedTower.Y - r, r * 2, r * 2);
                g.SmoothingMode = SmoothingMode.None;
            }

            // 4. Vẽ Bóng ma (Ghost Tower)
            DrawGhostTower(g);

            // 5. Hiệu ứng Màn hình Đỏ
            if (_hurtTimer > 0)
            {
                g.FillRectangle(_redScreenBrush, 0, 0, 800, 600);
            }

            // --- QUAN TRỌNG: RESET SCALE VỀ 1:1 ĐỂ VẼ UI KHỚP VỚI NÚT BẤM ---
            g.Restore(originalState);
            // ---------------------------------------------------------------

            // 6. Vẽ Giao diện (HUD & Control Panel)
            DrawGlassHUD(g);
        }

        // =========================================================
        // 4. CÁC HÀM VẼ PHỤ TRỢ
        // =========================================================

        private void DrawGhostTower(Graphics g)
        {
            int typeId = GameManager.Instance.SelectedTowerType;
            if (typeId < 0 || typeId >= GameConfig.Towers.Length) return;
            if (!_isHoveringBoard || _selectedTower != null) return;

            var stat = GameConfig.Towers[typeId];
            float range = stat.Range;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color stateColor = _isGhostValid ? Color.LightGreen : Color.Red;
            Pen pen = _isGhostValid ? _validPen : _invalidPen;

            // Vẽ tầm bắn
            using (SolidBrush b = new SolidBrush(Color.FromArgb(40, stateColor)))
                g.FillEllipse(b, _ghostPos.X - range, _ghostPos.Y - range, range * 2, range * 2);
            g.DrawEllipse(pen, _ghostPos.X - range, _ghostPos.Y - range, range * 2, range * 2);

            // Vẽ ảnh mờ
            Image sprite = ResourceManager.GetImage(stat.Name);
            if (sprite != null)
            {
                ImageAttributes attr = new ImageAttributes();
                attr.SetColorMatrix(new ColorMatrix { Matrix33 = 0.5f }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(sprite, new Rectangle(_ghostPos.X - 20, _ghostPos.Y - 20, 40, 40), 0, 0, sprite.Width, sprite.Height, GraphicsUnit.Pixel, attr);
            }
            else
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(128, stat.Color)))
                    g.FillRectangle(b, _ghostPos.X - 16, _ghostPos.Y - 16, 32, 32);
            }

            // Vẽ dấu X nếu lỗi
            if (!_isGhostValid)
            {
                using (Pen p = new Pen(Color.Red, 3))
                {
                    g.DrawLine(p, _ghostPos.X - 10, _ghostPos.Y - 10, _ghostPos.X + 10, _ghostPos.Y + 10);
                    g.DrawLine(p, _ghostPos.X + 10, _ghostPos.Y - 10, _ghostPos.X - 10, _ghostPos.Y + 10);
                }
            }
            g.SmoothingMode = SmoothingMode.None;
        }

        private void DrawGlassHUD(Graphics g)
        {
            // A. THANH TRÊN CÙNG
            g.FillRectangle(_topBarBrush, 0, 0, 800, 40);
            TextRenderer.DrawText(g, $"🌊 WAVE: {GameManager.Instance.WaveMgr.CurrentWave}", _hudFont, new Point(20, 8), Color.Cyan);
            TextRenderer.DrawText(g, $"💰 GOLD: {GameManager.Instance.PlayerMoney}", _hudFont, new Point(250, 8), Color.Gold);
            TextRenderer.DrawText(g, $"❤ LIVES: {GameManager.Instance.PlayerLives}", _hudFont, new Point(500, 8), Color.Red);

            // B. KHU VỰC ĐIỀU KHIỂN DƯỚI (Control Panel)
            // Vẽ nền đặc cho khu vực Y=600 đến hết
            Rectangle hudRect = new Rectangle(0, 600, 800, 160);
            g.FillRectangle(_bottomUiBrush, hudRect);
            g.DrawLine(_separatorPen, 0, 600, 800, 600);

            // Vẽ khung phân chia (Đã khớp với Designer)
            g.DrawRectangle(_uiBorderPen, 10, 610, 420, 110);
            g.DrawString("TOWERS", _uiTitleFont, Brushes.Gray, 15, 612);

            g.DrawRectangle(_uiBorderPen, 440, 610, 180, 110);
            g.DrawString("SKILLS", _uiTitleFont, Brushes.Gray, 445, 612);

            UpdateSkillButtonState();
           // if (GameManager.Instance.PlayerLives <= 0) HandleGameOver(g);
        }

        private void UpdateSkillButtonState()
        {
            var sm = GameManager.Instance.SkillMgr;
            if (sm.MeteorCooldown > 0) { _btnSkillMeteor.Enabled = false; _btnSkillMeteor.Text = $"{sm.MeteorCooldown:0.0}"; _btnSkillMeteor.BackColor = Color.Gray; }
            else { _btnSkillMeteor.Enabled = true; _btnSkillMeteor.Text = "METEOR"; _btnSkillMeteor.BackColor = Color.OrangeRed; }

            if (sm.FreezeCooldown > 0) { _btnSkillFreeze.Enabled = false; _btnSkillFreeze.Text = $"{sm.FreezeCooldown:0.0}"; _btnSkillFreeze.BackColor = Color.Gray; }
            else { _btnSkillFreeze.Enabled = true; _btnSkillFreeze.Text = "FREEZE"; _btnSkillFreeze.BackColor = Color.Cyan; }
        }

        private void HandleGameOver(Graphics g)
        {
            using (SolidBrush b = new SolidBrush(Color.FromArgb(220, 0, 0, 0)))
                g.FillRectangle(b, 0, 0, 800, 800);

            using (Font f = new Font("Arial", 50, FontStyle.Bold))
                g.DrawString("GAME OVER", f, Brushes.Red, 200, 250);

            _gameTimer.Stop();

            string name = Prompt.ShowDialog("Nhập tên lưu điểm:", "Game Over");
            if (string.IsNullOrEmpty(name)) name = "Unknown";

            int score = (GameManager.Instance.PlayerMoney / 10) + (GameManager.Instance.WaveMgr.CurrentWave * 100);
            Managers.HighScoreManager.SaveScore(name, score);
            Managers.HistoryManager.SaveLog(false, GameManager.Instance.WaveMgr.CurrentWave);

            this.Close();
        }
    }
}