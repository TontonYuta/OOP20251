using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;
using TowerDefense.Utils;

namespace TowerDefense.Forms
{
    public partial class LevelSelectForm : Form
    {
        private FlowLayoutPanel _pnlLevels;
        private PictureBox _bgContainer;

        // --- BÍ KÍP CHỐNG NHÁY/ĐEN MÀN HÌNH ---
        // Ép hệ thống vẽ chồng lớp (Composited) để xử lý ảnh động mượt mà
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED
                return cp;
            }
        }

        public LevelSelectForm()
        {
            this.Text = "SELECT BATTLEFIELD";
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Đặt màu nền trùng màu tối để nếu ảnh chưa load kịp thì không bị trắng xóa
            this.BackColor = Color.FromArgb(30, 30, 40);

            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // 1. TẠO NỀN GIF
            _bgContainer = new PictureBox();
            _bgContainer.Dock = DockStyle.Fill;
            _bgContainer.SizeMode = PictureBoxSizeMode.StretchImage;

            // Đường dẫn ảnh
            string gifPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Images\level_select_bg.gif");

            if (System.IO.File.Exists(gifPath))
            {
                _bgContainer.Image = Image.FromFile(gifPath);
            }
            else
            {
                _bgContainer.BackColor = Color.FromArgb(30, 30, 40);
            }

            this.Controls.Add(_bgContainer);

            // 2. TIÊU ĐỀ
            Label lbl = new Label
            {
                Text = "CAMPAIGN MAP",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(280, 20),
                BackColor = Color.Transparent
            };
            _bgContainer.Controls.Add(lbl); // <--- Add vào BG

            // 3. CONTAINER CHỨA NÚT
            _pnlLevels = new FlowLayoutPanel
            {
                Location = new Point(50, 90),
                Size = new Size(760, 400),
                // Mẹo: Nếu Transparent vẫn bị lỗi đen, hãy đổi thành màu đen mờ (Alpha thấp)
                BackColor = Color.FromArgb(100, 0, 0, 0),
                AutoScroll = true
            };
            _bgContainer.Controls.Add(_pnlLevels); // <--- Add vào BG

            for (int i = 1; i <= 10; i++)
            {
                CreateLevelButton(i);
            }

            // 4. NÚT BACK
            GameButton btnBack = new GameButton
            {
                Text = "BACK TO MENU",
                Size = new Size(200, 50),
                Location = new Point(325, 500),
                Color1 = Color.DarkSlateGray,
                Color2 = Color.Black
            };
            btnBack.Click += (s, e) => this.Close();
            _bgContainer.Controls.Add(btnBack); // <--- Add vào BG
        }

        private void CreateLevelButton(int id)
        {
            Color baseColor = Color.LightGreen;
            string difficulty = "Easy";

            if (id >= 4 && id <= 6) { baseColor = Color.Orange; difficulty = "Normal"; }
            else if (id >= 7 && id <= 9) { baseColor = Color.OrangeRed; difficulty = "Hard"; }
            else if (id == 10) { baseColor = Color.Purple; difficulty = "EXTREME"; }

            GameButton btn = new GameButton
            {
                Text = $"LEVEL {id}\n{difficulty}",
                Size = new Size(130, 100),
                Color1 = ControlPaint.Light(baseColor),
                Color2 = baseColor,
                BorderRadius = 20,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Margin = new Padding(10)
            };

            btn.Click += (s, e) => OpenGameLevel(id);
            _pnlLevels.Controls.Add(btn);
        }

        private void OpenGameLevel(int levelId)
        {
            this.Hide();
            GameLevel1 game = new GameLevel1(levelId);
            game.ShowDialog();
            this.Show();
        }

        private void InitializeComponent() { }
    }
}