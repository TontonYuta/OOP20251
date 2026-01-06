using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Utils; // Dùng GameButton
using TowerDefense.Data;  // Dùng UserProfile

namespace TowerDefense.Forms
{
    public partial class VictoryForm : Form
    {
        public VictoryForm(int levelId, int lives, int maxLives)
        {
            InitializeComponent(levelId, lives, maxLives);
        }

        private void InitializeComponent(int levelId, int lives, int maxLives)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 350);
            this.BackColor = Color.FromArgb(20, 40, 20); // Xanh rêu đậm

            // Viền Vàng chiến thắng
            this.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.Gold, 4, ButtonBorderStyle.Solid, Color.Gold, 4, ButtonBorderStyle.Solid, Color.Gold, 4, ButtonBorderStyle.Solid, Color.Gold, 4, ButtonBorderStyle.Solid);

            // Tiêu đề
            Label lblTitle = new Label { Text = "VICTORY!", Font = new Font("Arial", 28, FontStyle.Bold), ForeColor = Color.Gold, AutoSize = true, Location = new Point(100, 20) };
            this.Controls.Add(lblTitle);

            // Tính số sao (3 sao = 100% máu, 2 sao > 50%, 1 sao > 0)
            int stars = 1;
            if (lives >= maxLives) stars = 3;
            else if (lives >= maxLives / 2) stars = 2;

            // Vẽ sao (Dùng Label text tạm)
            string starString = "";
            for (int i = 0; i < stars; i++) starString += "⭐ ";
            Label lblStars = new Label { Text = starString, Font = new Font("Segoe UI Emoji", 20), ForeColor = Color.Yellow, AutoSize = true, Location = new Point(130, 80) };
            this.Controls.Add(lblStars);

            // Phần thưởng
            int reward = 50 * levelId; // Thưởng theo độ khó map
            Label lblReward = new Label { Text = $"Reward: +{reward} 💎", Font = new Font("Arial", 14, FontStyle.Bold), ForeColor = Color.Cyan, AutoSize = true, Location = new Point(110, 140) };
            this.Controls.Add(lblReward);

            // Cộng tiền vào Profile
            UserProfile.Instance.Diamonds += reward;
            UserProfile.Save();

            // Nút Continue
            GameButton btnOk = new GameButton { Text = "CONTINUE", Location = new Point(100, 250), Size = new Size(200, 50), Color1 = Color.Gold, Color2 = Color.OrangeRed };
            btnOk.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };
            this.Controls.Add(btnOk);
        }
    }
}