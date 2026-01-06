using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Utils;

namespace TowerDefense.Forms
{
    public partial class ResultForm : Form
    {
        public string PlayerName { get; private set; } = "Unknown";
        public bool IsRetry { get; private set; } = false;

        private TextBox _txtName;
        private bool _isVictory;
        private int _score;

        public ResultForm(bool isVictory, int wave, int gold)
        {
            _isVictory = isVictory;
            _score = (wave * 100) + (gold / 10); // Công thức tính điểm

            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent() { } // Hàm giả

        private void SetupUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 400);
            this.BackColor = _isVictory ? Color.FromArgb(20, 40, 20) : Color.FromArgb(30, 10, 10);

            // Viền Form
            Color borderColor = _isVictory ? Color.Gold : Color.Red;
            this.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, borderColor, 4, ButtonBorderStyle.Solid, borderColor, 4, ButtonBorderStyle.Solid, borderColor, 4, ButtonBorderStyle.Solid, borderColor, 4, ButtonBorderStyle.Solid);

            // 1. Tiêu đề
            Label lblTitle = new Label
            {
                Text = _isVictory ? "VICTORY!" : "DEFEAT",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = borderColor,
                AutoSize = true,
                Location = new Point(_isVictory ? 130 : 145, 30)
            };
            this.Controls.Add(lblTitle);

            // 2. Điểm số
            Label lblScore = new Label
            {
                Text = $"TOTAL SCORE: {_score}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(140, 100)
            };
            this.Controls.Add(lblScore);

            // 3. Nhập tên
            Label lblName = new Label { Text = "Enter Your Name:", ForeColor = Color.Gray, Location = new Point(100, 160), AutoSize = true, Font = new Font("Arial", 10) };
            this.Controls.Add(lblName);

            _txtName = new TextBox
            {
                Location = new Point(100, 185),
                Size = new Size(300, 35),
                Font = new Font("Arial", 14),
                BackColor = Color.FromArgb(50, 50, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };
            this.Controls.Add(_txtName);

            // 4. Các nút bấm
            int btnY = 280;

            // Nút Save & Menu
            GameButton btnMenu = new GameButton
            {
                Text = "MAIN MENU",
                Location = new Point(50, btnY),
                Size = new Size(180, 50),
                Color1 = Color.Gray,
                Color2 = Color.Black
            };
            btnMenu.Click += (s, e) => { SaveAndClose(false); };
            this.Controls.Add(btnMenu);

            // Nút Retry (Chơi lại)
            GameButton btnRetry = new GameButton
            {
                Text = "RETRY",
                Location = new Point(270, btnY),
                Size = new Size(180, 50),
                Color1 = _isVictory ? Color.Green : Color.OrangeRed,
                Color2 = _isVictory ? Color.DarkGreen : Color.Maroon
            };
            btnRetry.Click += (s, e) => { SaveAndClose(true); };
            this.Controls.Add(btnRetry);
        }

        private void SaveAndClose(bool retry)
        {
            if (!string.IsNullOrWhiteSpace(_txtName.Text))
                PlayerName = _txtName.Text;

            IsRetry = retry;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}