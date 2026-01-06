using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Utils; // Để dùng GameButton

namespace TowerDefense.Forms
{
    public partial class GameOverForm : Form
    {
        public string PlayerName { get; private set; } = "Unknown";

        private TextBox _txtName;
        private Label _lblScore;

        public GameOverForm(int wave, int gold)
        {
            InitializeComponent();

            // Tính điểm
            int score = (wave * 100) + (gold / 10);
            _lblScore.Text = $"SCORE: {score}";
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 300);
            this.BackColor = Color.FromArgb(20, 20, 20); // Nền đen đậm

            // Viền đỏ báo hiệu thua
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.Red, 3, ButtonBorderStyle.Solid, Color.Red, 3, ButtonBorderStyle.Solid, Color.Red, 3, ButtonBorderStyle.Solid, Color.Red, 3, ButtonBorderStyle.Solid);
            };

            // Tiêu đề
            Label lblTitle = new Label { Text = "GAME OVER", Font = new Font("Arial", 24, FontStyle.Bold), ForeColor = Color.Red, AutoSize = true, Location = new Point(100, 30) };
            this.Controls.Add(lblTitle);

            // Điểm số
            _lblScore = new Label { Text = "SCORE: 0", Font = new Font("Arial", 16, FontStyle.Bold), ForeColor = Color.Yellow, AutoSize = true, Location = new Point(130, 80) };
            this.Controls.Add(_lblScore);

            // Nhập tên
            Label lblName = new Label { Text = "Enter Your Name:", ForeColor = Color.White, Location = new Point(50, 140), AutoSize = true };
            this.Controls.Add(lblName);

            _txtName = new TextBox { Location = new Point(50, 165), Size = new Size(300, 30), Font = new Font("Arial", 12) };
            this.Controls.Add(_txtName);

            // Nút Lưu & Thoát (Dùng GameButton)
            GameButton btnSave = new GameButton
            {
                Text = "SAVE & EXIT",
                Location = new Point(100, 220),
                Size = new Size(200, 50),
                Color1 = Color.DarkRed,
                Color2 = Color.Maroon
            };
            btnSave.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(_txtName.Text)) PlayerName = _txtName.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnSave);
        }
    }
}