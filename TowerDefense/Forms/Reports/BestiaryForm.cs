using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using TowerDefense.Configs;
using TowerDefense.Managers;
using TowerDefense.Utils;

namespace TowerDefense.Forms.Reports
{
    public partial class BestiaryForm : Form
    {
        private FlowLayoutPanel _pnlList; // Container cho danh sách quái
        private Panel _pnlDetails;       // Container cho chi tiết quái
        private EnemyStat[] _enemies = GameConfig.Enemies; // Lấy danh sách quái từ Config

        public BestiaryForm()
        {
            // Hàm khởi tạo cơ bản (Không dùng Designer.cs)
            InitializeBasicComponent();
            SetupUI();
            LoadEnemyList();

            // Hiển thị con quái đầu tiên khi mở form
            if (_enemies.Length > 0) DisplayEnemyDetails(0);
        }

        private void InitializeBasicComponent()
        {
            this.Text = "Bestiary - Monster Codex";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 40);
        }

        private void SetupUI()
        {
            // 1. Tiêu đề
            Label lblTitle = new Label
            {
                Text = "MONSTER CODEX",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitle);

            // 2. Nút đóng (Dùng GameButton bo tròn)
            GameButton btnClose = new GameButton
            {
                Text = "X",
                Location = new Point(740, 10),
                Size = new Size(40, 40),
                BorderRadius = 40,
                Color1 = Color.DarkRed,
                Color2 = Color.Maroon
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            // 3. Panel Danh sách (Left Side)
            _pnlList = new FlowLayoutPanel
            {
                Location = new Point(20, 70),
                Size = new Size(220, 490),
                BackColor = Color.FromArgb(40, 40, 60),
                AutoScroll = true, // Quan trọng để xem hết 20 con
                Padding = new Padding(5)
            };
            this.Controls.Add(_pnlList);

            // 4. Panel Chi tiết (Right Side)
            _pnlDetails = new Panel
            {
                Location = new Point(250, 70),
                Size = new Size(530, 490),
                BackColor = Color.FromArgb(50, 50, 70)
            };
            this.Controls.Add(_pnlDetails);
        }

        private void LoadEnemyList()
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                var stat = _enemies[i];
                // Button đại diện cho mỗi con quái (Sử dụng GameButton)
                GameButton btn = new GameButton
                {
                    Text = stat.Name,
                    Tag = i, // Lưu ID của quái vào Tag
                    Size = new Size(190, 45),
                    Margin = new Padding(5, 5, 5, 0),
                    Color1 = ControlPaint.Light(stat.Color),
                    Color2 = stat.Color,
                    BorderRadius = 10,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = (stat.Color.GetBrightness() < 0.3) ? Color.White : Color.Black // Tự động chọn màu chữ
                };

                // Gán sự kiện click để hiện chi tiết
                btn.Click += EnemyButtonClick;
                _pnlList.Controls.Add(btn);
            }
        }

        private void EnemyButtonClick(object sender, EventArgs e)
        {
            if (sender is GameButton btn && btn.Tag is int enemyId)
            {
                DisplayEnemyDetails(enemyId);
            }
        }

        private void DisplayEnemyDetails(int enemyId)
        {
            _pnlDetails.Controls.Clear(); // Xóa nội dung cũ

            if (enemyId < 0 || enemyId >= _enemies.Length) return;

            var stat = _enemies[enemyId];
            int y = 20;

            // 1. Tên Quái
            Label lblName = new Label
            {
                Text = stat.Name.ToUpper(),
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = stat.Color,
                Location = new Point(20, y),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            _pnlDetails.Controls.Add(lblName);
            y += 50;

            // 2. Phân loại & Mô tả
            string type = (stat.DamageToTower > 0) ? "Melee (Attacks Towers)" : "Runner (Ignores Towers)";
            if (stat.CanFly) type = "FLYING (Ignores ground path)";

            Label lblType = new Label
            {
                Text = $"Type: {type}",
                Font = new Font("Arial", 12, FontStyle.Italic),
                ForeColor = Color.LightGray,
                Location = new Point(25, y),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            _pnlDetails.Controls.Add(lblType);
            y += 40;

            // 3. Thông số cơ bản
            string details = $"HEALTH: {stat.MaxHealth}\n";
            details += $"SPEED: {stat.Speed} px/s\n";
            details += $"REWARD: {stat.Reward} Gold\n";
            details += "\n--- COMBAT ---\n";
            details += $"DAMAGE (vs Base): 1\n";
            details += $"DAMAGE (vs Tower): {stat.DamageToTower}\n";
            details += $"RANGE: {(stat.DamageToTower > 0 ? stat.AttackRange.ToString() + " px" : "N/A")}\n";

            Label lblStats = new Label
            {
                Text = details,
                Font = new Font("Consolas", 11),
                ForeColor = Color.LightGreen,
                Location = new Point(25, y),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            _pnlDetails.Controls.Add(lblStats);

            // 4. Ảnh đại diện (Sprite)
            Image sprite = ResourceManager.GetImage(stat.Name);
            if (sprite != null)
            {
                // Vị trí ảnh: Góc trên bên phải
                PictureBox pb = new PictureBox
                {
                    Image = sprite,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(350, 10),
                    Size = new Size(150, 150),
                    BackColor = Color.Transparent
                };
                _pnlDetails.Controls.Add(pb);
            }
        }
    }
}