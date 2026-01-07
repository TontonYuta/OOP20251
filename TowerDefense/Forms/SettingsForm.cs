using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers; // Để gọi SoundManager
using TowerDefense.Configs;  // Để lưu GameSettings
using TowerDefense.Utils;

namespace TowerDefense.Forms
{
    public class SettingsForm : Form
    {
        // --- CÁC CONTROL TỰ CHẾ (CUSTOM CONTROLS) ---
        private NeonSlider _sliderMusic;
        private NeonSlider _sliderSfx;
        private CyberSwitch _swAutoWave;
        private CyberSwitch _swShowHp;
        private CyberSwitch _swParticles;

        // Kéo thả cửa sổ không viền
        private bool _dragging = false;
        private Point _dragCursorPoint;
        private Point _dragFormPoint;

        public SettingsForm()
        {
            // 1. Cấu hình Form "Ngầu"
            this.Text = "SYSTEM CONFIGURATION";
            this.Size = new Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.None; // Không viền Windows
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(15, 15, 20); // Màu đen tím than
            this.DoubleBuffered = true; // Chống nháy hình

            // Sự kiện kéo cửa sổ
            this.MouseDown += (s, e) => { _dragging = true; _dragCursorPoint = Cursor.Position; _dragFormPoint = this.Location; };
            this.MouseMove += (s, e) => {
                if (_dragging)
                {
                    Point dif = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
                    this.Location = Point.Add(_dragFormPoint, new Size(dif));
                }
            };
            this.MouseUp += (s, e) => { _dragging = false; };

            InitializeFutureUI();
        }

        private void InitializeFutureUI()
        {
            int centerX = this.Width / 2;
            int startY = 80;

            // --- HEADER ---
            Label lblTitle = new Label
            {
                Text = "SYSTEM SETTINGS",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.Cyan,
                AutoSize = true,
                Location = new Point(0, 20),
                BackColor = Color.Transparent
            };
            // Căn giữa tiêu đề
            this.Controls.Add(lblTitle);
            lblTitle.Location = new Point((this.Width - lblTitle.Width) / 2, 25);

            // Nút Đóng (X)
            Label btnClose = new Label
            {
                Text = "X",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(this.Width - 40, 15)
            };
            btnClose.Click += (s, e) => { SoundManager.Play("click"); this.Close(); };
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.Red;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.Gray;
            this.Controls.Add(btnClose);

            // ========================================================
            // 1. AUDIO SECTION (Chức năng thật)
            // ========================================================

            AddSectionTitle("AUDIO CONFIG", 70);

            // Music Slider
            AddLabel("MUSIC VOLUME", 40, 110);
            _sliderMusic = new NeonSlider(0, 100, (int)(GameSettings.MusicVolume * 100));
            _sliderMusic.Location = new Point(40, 135);
            _sliderMusic.ValueChanged += (val) => {
                // [LOGIC THẬT] Chỉnh âm lượng ngay lập tức
                float volume = val / 100f;
                GameSettings.MusicVolume = volume;
                SoundManager.SetMusicVolume(volume);
            };
            this.Controls.Add(_sliderMusic);

            // SFX Slider
            AddLabel("SFX VOLUME", 40, 190);
            _sliderSfx = new NeonSlider(0, 100, (int)(GameSettings.SfxVolume * 100));
            _sliderSfx.Location = new Point(40, 215);
            _sliderSfx.ValueChanged += (val) => {
                // [LOGIC THẬT] Lưu SFX Volume
                GameSettings.SfxVolume = val / 100f;
            };
            this.Controls.Add(_sliderSfx);

            // ========================================================
            // 2. GAMEPLAY (Demo Visual cực đẹp)
            // ========================================================

            AddSectionTitle("GAMEPLAY MODS", 280);

            // Auto Wave Switch
            _swAutoWave = new CyberSwitch(GameSettings.AutoStartWave);
            _swAutoWave.Location = new Point(380, 320);
            _swAutoWave.CheckedChanged += (isOn) => {
                GameSettings.AutoStartWave = isOn; // Lưu setting
                // Demo sound
                SoundManager.Play("click");
            };
            this.Controls.Add(_swAutoWave);
            AddLabel("AUTO START WAVE", 40, 325);

            // Show HP Switch
            _swShowHp = new CyberSwitch(true); // Mặc định bật demo
            _swShowHp.Location = new Point(380, 380);
            _swShowHp.CheckedChanged += (isOn) => SoundManager.Play("click");
            this.Controls.Add(_swShowHp);
            AddLabel("SHOW ENEMY HP", 40, 385);

            // Particles Switch
            _swParticles = new CyberSwitch(true);
            _swParticles.Location = new Point(380, 440);
            _swParticles.CheckedChanged += (isOn) => SoundManager.Play("click");
            this.Controls.Add(_swParticles);
            AddLabel("HIGH QUALITY VFX", 40, 445);

            // ========================================================
            // 3. ACTION BUTTONS
            // ========================================================
            CyberButton btnSave = new CyberButton("APPLY CHANGES");
            btnSave.Location = new Point(125, 520);
            btnSave.Click += (s, e) => {
                SoundManager.Play("click"); // SoundManager.Play("upgrade"); nếu có
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnSave);
        }

        // --- Helpers ---
        private void AddSectionTitle(string text, int y)
        {
            Label l = new Label { Text = text, ForeColor = Color.Cyan, Font = new Font("Consolas", 10, FontStyle.Bold), AutoSize = true, Location = new Point(35, y) };
            this.Controls.Add(l);
            // Kẻ đường line
            Panel line = new Panel { BackColor = Color.FromArgb(50, 50, 60), Size = new Size(430, 2), Location = new Point(35, y + 25) };
            this.Controls.Add(line);
        }
        private void AddLabel(string text, int x, int y)
        {
            Label l = new Label { Text = text, ForeColor = Color.Silver, Font = new Font("Segoe UI", 10), AutoSize = true, Location = new Point(x, y) };
            this.Controls.Add(l);
        }

        // Vẽ Background Grid (Lưới)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            // Vẽ viền Neon
            using (Pen p = new Pen(Color.Cyan, 2)) g.DrawRectangle(p, 0, 0, Width, Height);

            // Vẽ lưới mờ (Grid effect)
            using (Pen gridPen = new Pen(Color.FromArgb(30, 255, 255, 255), 1))
            {
                for (int i = 0; i < Width; i += 40) g.DrawLine(gridPen, i, 0, i, Height);
                for (int i = 0; i < Height; i += 40) g.DrawLine(gridPen, 0, i, Width, i);
            }
        }
    }

    // =================================================================================
    // CÁC CLASS UI TỰ CHẾ (NESTED UI CLASSES - ĐỂ CHUNG FILE CHO GỌN)
    // =================================================================================

    // 1. THANH TRƯỢT NEON (Thay thế TrackBar)
    public class NeonSlider : Control
    {
        public int Value { get; private set; }
        public event Action<int> ValueChanged;
        private bool _isDragging = false;
        private int _min, _max;

        public NeonSlider(int min, int max, int current)
        {
            this.Size = new Size(400, 30);
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            _min = min; _max = max; Value = current;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isDragging = true;
            UpdateValue(e.X);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging) UpdateValue(e.X);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void UpdateValue(int mouseX)
        {
            float percent = (float)mouseX / Width;
            if (percent < 0) percent = 0; if (percent > 1) percent = 1;
            Value = (int)(percent * (_max - _min)) + _min;
            ValueChanged?.Invoke(Value);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Vẽ rãnh (Track)
            int trackY = Height / 2;
            using (Pen p = new Pen(Color.FromArgb(40, 40, 40), 6))
            {
                p.StartCap = LineCap.Round; p.EndCap = LineCap.Round;
                g.DrawLine(p, 10, trackY, Width - 10, trackY);
            }

            // Vẽ phần đã chạy (Fill) - Màu Cyan pha Magenta
            float percent = (float)(Value - _min) / (_max - _min);
            int fillWidth = (int)((Width - 20) * percent);

            if (fillWidth > 0)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), Color.Cyan, Color.Magenta))
                using (Pen p = new Pen(brush, 6))
                {
                    p.StartCap = LineCap.Round; p.EndCap = LineCap.Round;
                    g.DrawLine(p, 10, trackY, 10 + fillWidth, trackY);
                }
            }

            // Vẽ cục tròn (Thumb)
            int thumbX = 10 + fillWidth;
            g.FillEllipse(Brushes.White, thumbX - 8, trackY - 8, 16, 16);
            // Glow effect
            using (SolidBrush b = new SolidBrush(Color.FromArgb(100, 0, 255, 255)))
                g.FillEllipse(b, thumbX - 12, trackY - 12, 24, 24);

            // Vẽ số %
            string txt = $"{Value}%";
            g.DrawString(txt, new Font("Arial", 8), Brushes.Gray, Width - 35, 0);
        }
    }

    // 2. CÔNG TẮC CYBER (Thay thế CheckBox)
    public class CyberSwitch : Control
    {
        public bool IsOn { get; private set; }
        public event Action<bool> CheckedChanged;

        public CyberSwitch(bool initialState)
        {
            this.Size = new Size(60, 30);
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            IsOn = initialState;
        }

        protected override void OnClick(EventArgs e)
        {
            IsOn = !IsOn;
            CheckedChanged?.Invoke(IsOn);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Màu nền: Xanh nếu On, Xám nếu Off
            Color backColor = IsOn ? Color.FromArgb(0, 200, 255) : Color.FromArgb(40, 40, 40);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = new GraphicsPath())
            {
                int r = Height;
                path.AddArc(rect.X, rect.Y, r, r, 90, 180);
                path.AddArc(rect.Right - r, rect.Y, r, r, 270, 180);
                path.CloseFigure();

                using (SolidBrush b = new SolidBrush(backColor)) g.FillPath(b, path);
                using (Pen p = new Pen(Color.Gray, 1)) g.DrawPath(p, path);
            }

            // Vẽ nút tròn
            int toggleX = IsOn ? Width - Height + 2 : 2;
            g.FillEllipse(Brushes.White, toggleX, 2, Height - 5, Height - 5);
        }
    }

    // 3. NÚT BẤM CYBER (Custom Button)
}