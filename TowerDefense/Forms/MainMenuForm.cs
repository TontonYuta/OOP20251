using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;
using TowerDefense.Forms.Reports;
using TowerDefense.Managers;
using TowerDefense.Utils;

namespace TowerDefense.Forms
{
    public partial class MainMenuForm : Form
    {
        // Biến này chứa khung ảnh nền GIF
        private PictureBox _bgContainer;

        public MainMenuForm()
        {
            InitializeComponent();
            SetupMenuUI();
            SoundManager.PlayMusic("menu_theme.wav");
        }

        private void SetupMenuUI()
        {
            this.Text = "Tower Defense Game";
            this.Size = new Size(600, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // --- 1. CÀI ĐẶT NỀN GIF ---
            _bgContainer = new PictureBox();
            _bgContainer.Dock = DockStyle.Fill; // Tràn màn hình
            _bgContainer.SizeMode = PictureBoxSizeMode.StretchImage; // Co giãn ảnh

            // Đường dẫn ảnh GIF
            string gifPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Images\menu_bg.gif");

            if (System.IO.File.Exists(gifPath))
            {
                _bgContainer.Image = Image.FromFile(gifPath); // Tự động chạy GIF
            }
            else
            {
                _bgContainer.BackColor = Color.FromArgb(20, 20, 30); // Màu đen dự phòng nếu không thấy ảnh
            }

            // Thêm khung nền vào Form
            this.Controls.Add(_bgContainer);

            // ==================================================================================
            // 2. TẠO TIÊU ĐỀ (ADD VÀO _bgContainer)
            // ==================================================================================
            Font titleFont = new Font("Arial Black", 28, FontStyle.Bold);
            string titleText = "DEFENSE OF THE TOWER";
            Point titlePos = new Point(30, 40);

            // Lớp BÓNG (Shadow)
            Label lblTitleShadow = new Label();
            lblTitleShadow.Text = titleText;
            lblTitleShadow.Font = titleFont;
            lblTitleShadow.AutoSize = true;
            lblTitleShadow.Location = new Point(titlePos.X + 4, titlePos.Y + 4);
            lblTitleShadow.ForeColor = Color.FromArgb(100, 0, 0, 0); // Đen bán trong suốt
            lblTitleShadow.BackColor = Color.Transparent;

            // QUAN TRỌNG: Add vào _bgContainer
            _bgContainer.Controls.Add(lblTitleShadow);

            // Lớp CHÍNH (Main)
            Label lblTitleMain = new Label();
            lblTitleMain.Text = titleText;
            lblTitleMain.Font = titleFont;
            lblTitleMain.AutoSize = true;
            lblTitleMain.Location = titlePos;
            lblTitleMain.ForeColor = Color.Gold;
            lblTitleMain.BackColor = Color.Transparent;

            _bgContainer.Controls.Add(lblTitleMain); // QUAN TRỌNG
            lblTitleMain.BringToFront();

            // Subtitle
            Label lblSub = new Label();
            lblSub.Text = "ULTIMATE STRATEGY GAME";
            lblSub.Font = new Font("Arial", 12, FontStyle.Italic | FontStyle.Bold);
            lblSub.AutoSize = true;
            lblSub.Location = new Point(180, 100);
            lblSub.ForeColor = Color.LightGoldenrodYellow;
            lblSub.BackColor = Color.Transparent;

            _bgContainer.Controls.Add(lblSub); // QUAN TRỌNG

            // ==================================================================================
            // 3. TẠO CÁC NÚT BẤM
            // ==================================================================================
            int startY = 160;
            int gap = 75;

            var btnPlay = CreateMenuButton("PLAY GAME", startY, Color.OrangeRed);
            btnPlay.Click += (s, e) => {
                this.Hide();
                new LevelSelectForm().ShowDialog();
                this.Show();
            };

            var btnShop = CreateMenuButton("SHOP & UPGRADE", startY + gap, Color.Purple);
            btnShop.Click += (s, e) => {
                this.Hide();
                new ShopForm().ShowDialog();
                this.Show();
            };

            var btnScore = CreateMenuButton("HIGH SCORES", startY + gap * 2, Color.DodgerBlue);
            btnScore.Click += (s, e) => { new HighScoreForm().ShowDialog(); };

            var btnBestiary = CreateMenuButton("BESTIARY", startY + gap * 3, Color.ForestGreen);
            btnBestiary.Click += (s, e) => { new BestiaryForm().ShowDialog(); };

            var btnHistory = CreateMenuButton("MATCH HISTORY", startY + gap * 4, Color.Teal);
            btnHistory.Click += (s, e) => { new HistoryForm().ShowDialog(); };

            var btnAbout = CreateMenuButton("ℹ  ABOUT GAME", startY + gap * 5, Color.DodgerBlue);
            btnAbout.Click += (s, e) => { new AboutForm().ShowDialog(); };

            var btnExit = CreateMenuButton("🚪  EXIT GAME", startY + gap * 6, Color.DarkRed);
            btnExit.Click += (s, e) => Application.Exit();

            // Tăng kích thước Form một chút
            this.Size = new Size(600, 750);
        }

        // Hàm tạo nút đã được sửa để add vào _bgContainer
        private Button CreateMenuButton(string text, int y, Color baseColor)
        {
            GameButton btn = new GameButton();
            btn.Text = text;
            btn.Size = new Size(240, 60);
            btn.Location = new Point(180, y);
            btn.Font = new Font("Arial", 12, FontStyle.Bold);

            // Màu sắc
            btn.Color1 = ControlPaint.Light(baseColor);
            btn.Color2 = ControlPaint.Dark(baseColor);
            btn.HoverColor1 = baseColor;
            btn.HoverColor2 = ControlPaint.Light(baseColor);
            btn.BorderRadius = 20;

            // Icon giả lập
            if (text.Contains("PLAY")) btn.Text = "▶  " + text;
            else if (text.Contains("SHOP")) btn.Text = "🛒  " + text;
            else if (text.Contains("SCORES")) btn.Text = "🏆  " + text;
            else if (text.Contains("BESTIARY")) btn.Text = "👹  " + text;
            else if (text.Contains("HISTORY")) btn.Text = "📜  " + text;
            else if (text.Contains("EXIT")) btn.Text = "🚪  " + text;

            // --- QUAN TRỌNG NHẤT: Add nút vào trong khung ảnh nền ---
            _bgContainer.Controls.Add(btn);

            return btn;
        }
    }
}