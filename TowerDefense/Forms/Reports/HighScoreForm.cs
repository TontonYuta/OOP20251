using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Utils;

namespace TowerDefense.Forms.Reports
{
    public partial class HighScoreForm : Form
    {
        private DataGridView _grid;

        public HighScoreForm()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "HALL OF FAME";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Nền Gradient
            this.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                       Color.FromArgb(40, 40, 60), Color.FromArgb(10, 10, 20), 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "🏆 LEADERBOARD 🏆",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(110, 20),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitle);

            // Custom DataGridView
            _grid = new DataGridView();
            _grid.Location = new Point(25, 70);
            _grid.Size = new Size(435, 400);
            _grid.BackgroundColor = Color.FromArgb(30, 30, 40);
            _grid.BorderStyle = BorderStyle.None;
            _grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _grid.GridColor = Color.DimGray;

            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.AllowUserToResizeRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.ScrollBars = ScrollBars.Vertical;

            // Style Header
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 20, 30);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 40;

            // Style Rows
            _grid.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 50);
            _grid.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 60, 80);
            _grid.DefaultCellStyle.SelectionForeColor = Color.White;
            _grid.RowTemplate.Height = 35;

            // --- ĐÃ SỬA: BỎ CỘT DATE VÀ TĂNG ĐỘ RỘNG CÁC CỘT KHÁC ---
            // Tổng chiều rộng grid là 435 (trừ scrollbar ~20 còn khoảng 415)
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "RANK", Width = 70, DataPropertyName = "Rank" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "PLAYER NAME", Width = 230, DataPropertyName = "Name" }); // Tăng từ 180 lên 230
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SCORE", Width = 115, DataPropertyName = "Score" });
            // Đã xóa dòng add cột DATE

            // Căn giữa Rank và Căn phải Score
            _grid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            _grid.CellFormatting += Grid_CellFormatting;

            this.Controls.Add(_grid);

            // Nút Đóng
            GameButton btnClose = new GameButton
            {
                Text = "CLOSE",
                Size = new Size(150, 45),
                Location = new Point(175, 500),
                Color1 = Color.DarkRed,
                Color2 = Color.Maroon,
                BorderRadius = 20
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadData()
        {
            var scores = HighScoreManager.LoadScores();

            // Fake data nếu chưa có file save (để test hiển thị)
            if (scores.Count == 0)
            {
                scores = new System.Collections.Generic.List<PlayerScore>
                {
                    new PlayerScore { Name = "The Legend", Score = 10000 },
                    new PlayerScore { Name = "Dragon Slayer", Score = 8500 },
                    new PlayerScore { Name = "Tower King", Score = 7200 },
                    new PlayerScore { Name = "Pro Gamer", Score = 6000 },
                    new PlayerScore { Name = "Rookie 01", Score = 4500 },
                    new PlayerScore { Name = "NoobMaster", Score = 3000 },
                    new PlayerScore { Name = "Guest User", Score = 1500 },
                };
            }

            // Chuyển đổi dữ liệu hiển thị
            var displayList = new System.Collections.Generic.List<object>();

            for (int i = 0; i < scores.Count; i++)
            {
                displayList.Add(new
                {
                    Rank = i + 1,
                    Name = scores[i].Name,
                    Score = scores[i].Score
                    // Đã xóa dòng Date = scores[i].Date
                });
            }

            _grid.DataSource = displayList;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = _grid.Rows[e.RowIndex];

                // Rank 1: Vàng
                if (e.RowIndex == 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.Gold;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    if (e.ColumnIndex == 1) e.Value = "👑 " + e.Value;
                }
                // Rank 2: Bạc
                else if (e.RowIndex == 1)
                {
                    row.DefaultCellStyle.ForeColor = Color.Silver;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                }
                // Rank 3: Đồng
                else if (e.RowIndex == 2)
                {
                    row.DefaultCellStyle.ForeColor = Color.Chocolate;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                }
            }
        }
    }
}