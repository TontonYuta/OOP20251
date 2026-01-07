using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers; // <--- QUAN TRỌNG: Để gọi HighScoreManager và SoundManager
using TowerDefense.Utils;    // <--- QUAN TRỌNG: Để gọi CyberButton

namespace TowerDefense.Forms
{
    public partial class ResultForm : CyberFormBase
    {
        public string PlayerName { get; private set; } = "Unknown";
        public bool IsRetry { get; private set; } = false;

        private TextBox _txtName;
        private bool _isVictory;
        private int _score;

        public ResultForm(bool isVictory, int wave, int gold)
        {
            _isVictory = isVictory;
            _score = (wave * 100) + (gold / 10);

            // Cấu hình Form
            this.Size = new Size(500, 450);
            if (lblTitle != null) lblTitle.Visible = false;

            // Âm thanh kết quả
            if (_isVictory) SoundManager.Play("win");
            else SoundManager.Play("lose");

            SetupUI();
        }

        private void SetupUI()
        {
            Color themeColor = _isVictory ? Color.Gold : Color.Red;
            string titleText = _isVictory ? "MISSION ACCOMPLISHED" : "MISSION FAILED";

            // 1. TIÊU ĐỀ LỚN
            Label lblBigTitle = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", _isVictory ? 24 : 28, FontStyle.Bold),
                ForeColor = themeColor,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblBigTitle);

            // 2. ĐIỂM SỐ
            Label lblScore = new Label
            {
                Text = $"SCORE: {_score:N0}",
                Font = new Font("Consolas", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };
            lblScore.BringToFront();
            this.Controls.Add(lblScore);

            // 3. NHẬP TÊN
            int inputY = 180;
            Label lblInput = new Label
            {
                Text = "ENTER OPERATOR NAME:",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(100, inputY)
            };
            this.Controls.Add(lblInput);

            _txtName = new TextBox
            {
                Location = new Point(100, inputY + 25),
                Size = new Size(300, 30),
                Font = new Font("Consolas", 14),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = themeColor,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center,
                MaxLength = 15
            };
            this.Controls.Add(_txtName);

            // 4. CÁC NÚT BẤM
            int btnY = 320;

            // Nút Menu
            CyberButton btnMenu = new CyberButton("MAIN MENU");
            btnMenu.Size = new Size(180, 50);
            btnMenu.Location = new Point(50, btnY);
            btnMenu.DefaultColor = Color.FromArgb(30, 30, 30);
            btnMenu.BorderColor = Color.Gray;
            btnMenu.Click += (s, e) => {
                SoundManager.Play("click");
                SaveAndClose(false);
            };
            this.Controls.Add(btnMenu);

            // Nút Retry
            CyberButton btnRetry = new CyberButton("RETRY MISSION");
            btnRetry.Size = new Size(180, 50);
            btnRetry.Location = new Point(270, btnY);
            btnRetry.DefaultColor = _isVictory ? Color.FromArgb(20, 40, 20) : Color.FromArgb(40, 20, 20);
            btnRetry.BorderColor = themeColor;
            btnRetry.HoverColor = _isVictory ? Color.Green : Color.Maroon;
            btnRetry.Click += (s, e) => {
                SoundManager.Play("click");
                SaveAndClose(true);
            };
            this.Controls.Add(btnRetry);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color borderColor = _isVictory ? Color.Gold : Color.Red;
            using (Pen p = new Pen(borderColor, 3))
            {
                e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
            }
        }

        private void SaveAndClose(bool retry)
        {
            if (!string.IsNullOrWhiteSpace(_txtName.Text))
            {
                PlayerName = _txtName.Text;

                // Gọi hàm lưu điểm từ HighScoreManager
                try
                {
                    HighScoreManager.SaveScore(PlayerName, _score);
                }
                catch { }
            }

            IsRetry = retry;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}