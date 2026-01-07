using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels; // Để gọi GameLevel1
using TowerDefense.Managers;         // Để gọi SoundManager
using TowerDefense.Utils;            // <--- QUAN TRỌNG: Để gọi CyberButton

namespace TowerDefense.Forms
{
    public partial class LevelSelectForm : Form // Hoặc kế thừa CyberFormBase nếu bạn đã tạo nó
    {
        private FlowLayoutPanel _pnlLevels;
        private PictureBox _bgContainer;

        // BÍ KÍP CHỐNG NHÁY
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
            this.BackColor = Color.FromArgb(30, 30, 40);

            // InitializeComponent(); // Nếu không dùng Designer thì bỏ dòng này
            SetupUI();
        }

        private void SetupUI()
        {
            // 1. TẠO NỀN GIF
            _bgContainer = new PictureBox();
            _bgContainer.Dock = DockStyle.Fill;
            _bgContainer.SizeMode = PictureBoxSizeMode.StretchImage;

            string gifPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\Images\level_select_bg.gif");
            if (!System.IO.File.Exists(gifPath))
                gifPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Images\level_select_bg.gif");

            if (System.IO.File.Exists(gifPath)) _bgContainer.Image = Image.FromFile(gifPath);
            else _bgContainer.BackColor = Color.FromArgb(30, 30, 40);

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
            _bgContainer.Controls.Add(lbl);

            // 3. CONTAINER CHỨA NÚT
            _pnlLevels = new FlowLayoutPanel
            {
                Location = new Point(50, 90),
                Size = new Size(760, 400),
                BackColor = Color.FromArgb(100, 0, 0, 0),
                AutoScroll = true
            };
            _bgContainer.Controls.Add(_pnlLevels);

            // Tạo 10 level
            for (int i = 1; i <= 10; i++)
            {
                CreateLevelButton(i);
            }

            // 4. NÚT BACK (Dùng CyberButton)
            CyberButton btnBack = new CyberButton("BACK TO MENU");
            btnBack.Size = new Size(200, 50);
            btnBack.Location = new Point(325, 520);
            btnBack.DefaultColor = Color.FromArgb(40, 40, 40);
            btnBack.BorderColor = Color.Gray;

            btnBack.Click += (s, e) => {
                SoundManager.Play("click"); // Đã thêm ở bước 1
                this.Close();
            };
            _bgContainer.Controls.Add(btnBack);
        }

        private void CreateLevelButton(int id)
        {
            // LOGIC MÀU SẮC CŨ CỦA BẠN (GIỮ NGUYÊN)
            Color baseColor = Color.LightGreen;
            string difficulty = "Easy";

            if (id >= 4 && id <= 6) { baseColor = Color.Orange; difficulty = "Normal"; }
            else if (id >= 7 && id <= 9) { baseColor = Color.OrangeRed; difficulty = "Hard"; }
            else if (id == 10) { baseColor = Color.Purple; difficulty = "EXTREME"; }

            // Dùng CyberButton thay vì GameButton
            CyberButton btn = new CyberButton
            {
                Text = $"LEVEL {id}\n{difficulty}",
                Size = new Size(130, 100),
                Margin = new Padding(10),
                Font = new Font("Arial", 10, FontStyle.Bold),

                // Mapping màu sang style Cyber
                BorderColor = baseColor,
                DefaultColor = Color.FromArgb(30, 30, 40),
                HoverColor = Color.FromArgb(baseColor.R / 2, baseColor.G / 2, baseColor.B / 2)
            };

            btn.Click += (s, e) => OpenGameLevel(id);
            _pnlLevels.Controls.Add(btn);
        }

        private void OpenGameLevel(int levelId)
        {
            SoundManager.Play("click");
            this.Hide();
            using (var game = new GameLevel1(levelId))
            {
                game.ShowDialog();
            }
            this.Show();
            SoundManager.PlayMusic("menu_theme.wav");
        }
    }
}