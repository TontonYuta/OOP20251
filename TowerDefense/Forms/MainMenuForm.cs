using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;
using TowerDefense.Forms.Reports;
using TowerDefense.Managers;
// =========================================================
using TowerDefense.Utils; // <--- QUAN TRỌNG NHẤT: Để nhận diện CyberButton
// =========================================================

namespace TowerDefense.Forms
{
    public partial class MainMenuForm : Form
    {
        private PictureBox _bgContainer;
        private Timer _animTimer;
        private int _gridOffset = 0;

        public MainMenuForm()
        {
            InitializeComponent();
            SetupMenuUI();

            SoundManager.PlayMusic("menu_theme.wav");

            // Timer hiệu ứng lưới
            _animTimer = new Timer();
            _animTimer.Interval = 50;
            _animTimer.Tick += (s, e) => {
                _gridOffset = (_gridOffset + 1) % 40;
                _bgContainer.Invalidate();
            };
            _animTimer.Start();
        }

        private void SetupMenuUI()
        {
            this.Text = "Tower Defense - Chaos Edition";
            this.Size = new Size(600, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            // 1. NỀN GIF
            _bgContainer = new PictureBox();
            _bgContainer.Dock = DockStyle.Fill;
            _bgContainer.SizeMode = PictureBoxSizeMode.StretchImage;

            string gifPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\Images\menu_bg.gif");
            if (!File.Exists(gifPath)) gifPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Images\menu_bg.gif");

            if (File.Exists(gifPath)) _bgContainer.Image = Image.FromFile(gifPath);
            else _bgContainer.BackColor = Color.FromArgb(10, 10, 15);

            _bgContainer.Paint += DrawCyberOverlay;
            this.Controls.Add(_bgContainer);

            // 2. MENU BUTTONS
            int startY = 180;
            int gap = 65;
            int btnX = (this.Width - 260) / 2;

            // PLAY GAME
            var btnPlay = CreateCyberButton("▶ INITIATE GAME", btnX, startY, true);
            btnPlay.Click += (s, e) => {
                SoundManager.Play("click");
                this.Hide();
                new LevelSelectForm().ShowDialog();
                this.Show();
                SoundManager.PlayMusic("menu_theme.wav");
            };

            // SETTINGS
            var btnSettings = CreateCyberButton("⚙ SYSTEM CONFIG", btnX, startY + gap);
            btnSettings.Click += (s, e) => {
                SoundManager.Play("click");
                new SettingsForm().ShowDialog();
            };

            // SHOP
            var btnShop = CreateCyberButton("🛒 ARMORY & SHOP", btnX, startY + gap * 2);
            btnShop.Click += (s, e) => {
                SoundManager.Play("click");
                this.Hide();
                new ShopForm().ShowDialog();
                this.Show();
            };

            // HIGH SCORES
            var btnScore = CreateCyberButton("🏆 HALL OF FAME", btnX, startY + gap * 3);
            btnScore.Click += (s, e) => { SoundManager.Play("click"); new HighScoreForm().ShowDialog(); };

            // BESTIARY
            var btnBestiary = CreateCyberButton("👹 ENEMY DATABASE", btnX, startY + gap * 4);
            btnBestiary.Click += (s, e) => { SoundManager.Play("click"); new BestiaryForm().ShowDialog(); };

            // HISTORY
            var btnHistory = CreateCyberButton("📜 BATTLE LOGS", btnX, startY + gap * 5);
            btnHistory.Click += (s, e) => { SoundManager.Play("click"); new HistoryForm().ShowDialog(); };

            // ABOUT
            var btnAbout = CreateCyberButton("ℹ ABOUT SYSTEM", btnX, startY + gap * 6);
            btnAbout.Click += (s, e) => { SoundManager.Play("click"); new AboutForm().ShowDialog(); };

            // EXIT
            var btnExit = CreateCyberButton("🚪 TERMINATE", btnX, startY + gap * 7);
            btnExit.ForeColor = Color.Red;
            btnExit.Click += (s, e) => Application.Exit();

            // Nút kéo cửa sổ
            Label dragHandle = new Label { Dock = DockStyle.Top, Height = 30, BackColor = Color.Transparent, Cursor = Cursors.SizeAll };
            dragHandle.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); }
            };
            _bgContainer.Controls.Add(dragHandle);
        }

        private void DrawCyberOverlay(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int w = _bgContainer.Width;
            int h = _bgContainer.Height;

            // Vẽ Lưới
            using (Pen gridPen = new Pen(Color.FromArgb(30, 0, 255, 255), 1))
            {
                for (int x = 0; x < w; x += 40) g.DrawLine(gridPen, x, 0, x, h);
                for (int y = _gridOffset - 40; y < h; y += 40) g.DrawLine(gridPen, 0, y, w, y);
            }

            // Vẽ Tiêu đề
            string title = "DEFENSE PROTOCOL";
            string sub = "CHAOS EDITION v1.2";
            Font fTitle = new Font("Segoe UI", 32, FontStyle.Bold);
            Font fSub = new Font("Consolas", 12, FontStyle.Bold);

            SizeF sTitle = g.MeasureString(title, fTitle);
            float tx = (w - sTitle.Width) / 2;
            float ty = 50;

            g.DrawString(title, fTitle, new SolidBrush(Color.FromArgb(100, 255, 0, 0)), tx - 3, ty);
            g.DrawString(title, fTitle, new SolidBrush(Color.FromArgb(100, 0, 255, 255)), tx + 3, ty);
            g.DrawString(title, fTitle, Brushes.White, tx, ty);

            SizeF sSub = g.MeasureString(sub, fSub);
            g.DrawString(sub, fSub, Brushes.Gold, (w - sSub.Width) / 2, ty + 60);

            // Vẽ Viền
            using (Pen p = new Pen(Color.Cyan, 3)) g.DrawRectangle(p, 0, 0, w, h);
        }

        private CyberButton CreateCyberButton(string text, int x, int y, bool isPrimary = false)
        {
            CyberButton btn = new CyberButton();
            btn.Text = text;
            btn.Location = new Point(x, y);
            if (isPrimary)
            {
                btn.DefaultColor = Color.FromArgb(0, 100, 180);
                btn.HoverColor = Color.FromArgb(0, 150, 255);
            }
            _bgContainer.Controls.Add(btn);
            return btn;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}