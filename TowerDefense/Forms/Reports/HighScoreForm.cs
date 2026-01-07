using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers; // <--- Cần dòng này để gọi HighScoreManager
using TowerDefense.Utils;    // <--- Cần dòng này để gọi CyberButton

namespace TowerDefense.Forms.Reports
{
    public partial class HighScoreForm : CyberFormBase
    {
        private DataGridView _grid;

        public HighScoreForm()
        {
            // 1. Cấu hình Form (Base đã lo phần viền)
            this.Size = new Size(550, 700);

            // Set tiêu đề cho Form cha
            if (lblTitle != null) lblTitle.Text = "HALL OF FAME // RANKING";

            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            // 2. TẠO GRIDVIEW (Giao diện Terminal)
            _grid = new DataGridView();
            _grid.Location = new Point(25, 80);
            _grid.Size = new Size(500, 500);

            // Màu sắc Dark Mode
            _grid.BackgroundColor = Color.FromArgb(20, 20, 25); // Nền bảng
            _grid.BorderStyle = BorderStyle.None;
            _grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _grid.GridColor = Color.FromArgb(50, 50, 60); // Màu đường kẻ mờ

            // Cấu hình hành vi
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.AllowUserToResizeRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.ScrollBars = ScrollBars.Vertical;

            // Style Header (Tiêu đề cột)
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 60, 80); // Xanh đậm
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Cyan; // Chữ Neon
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 45;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Style Rows (Dòng dữ liệu)
            _grid.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 40);
            _grid.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            _grid.DefaultCellStyle.Font = new Font("Consolas", 11); // Font kiểu code
            _grid.DefaultCellStyle.SelectionBackColor = Color.Cyan;
            _grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            _grid.DefaultCellStyle.Padding = new Padding(5);
            _grid.RowTemplate.Height = 40;

            // Thêm Cột
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "RANK", DataPropertyName = "Rank", Width = 70 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "OPERATOR", DataPropertyName = "Name", Width = 250 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SCORE", DataPropertyName = "Score", Width = 150 });

            // Căn lề và định dạng số
            _grid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _grid.Columns[2].DefaultCellStyle.Format = "N0"; // Thêm dấu phẩy (vd: 1,000)

            // Đăng ký sự kiện tô màu Top 3
            _grid.CellFormatting += Grid_CellFormatting;

            this.Controls.Add(_grid);

            // 3. NÚT ĐÓNG (CyberButton)
            CyberButton btnClose = new CyberButton("RETURN TO MENU");
            btnClose.Size = new Size(200, 50);
            btnClose.Location = new Point(175, 610);
            btnClose.DefaultColor = Color.FromArgb(40, 20, 20);
            btnClose.BorderColor = Color.Red;
            btnClose.HoverColor = Color.FromArgb(80, 20, 20);

            btnClose.Click += (s, e) => {
                SoundManager.Play("click");
                this.Close();
            };
            this.Controls.Add(btnClose);
        }

        private void LoadData()
        {
            var displayList = new List<object>();

            // 1. Tải dữ liệu thật từ file
            var realScores = HighScoreManager.LoadScores();

            if (realScores.Count > 0)
            {
                for (int i = 0; i < realScores.Count; i++)
                {
                    displayList.Add(new
                    {
                        Rank = i + 1,
                        Name = realScores[i].Name,
                        Score = realScores[i].Score
                    });
                }
            }
            else
            {
                // 2. Nếu chưa có dữ liệu -> Hiển thị Fake Data (Demo)
                displayList.Add(new { Rank = 1, Name = "CYBER_KING", Score = 999999 });
                displayList.Add(new { Rank = 2, Name = "DRAGON_SLAYER", Score = 850000 });
                displayList.Add(new { Rank = 3, Name = "TOWER_GOD", Score = 720000 });
                displayList.Add(new { Rank = 4, Name = "Pro_Gamer_VN", Score = 500000 });
                displayList.Add(new { Rank = 5, Name = "Rookie_01", Score = 300000 });
            }

            _grid.DataSource = displayList;
            _grid.ClearSelection();
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = _grid.Rows[e.RowIndex];

                // Tô màu Top 3
                if (e.RowIndex == 0) // Top 1: Vàng
                {
                    row.DefaultCellStyle.ForeColor = Color.Gold;
                    row.DefaultCellStyle.Font = new Font("Consolas", 12, FontStyle.Bold);
                    // Thêm vương miện nếu chưa có
                    if (e.ColumnIndex == 1 && e.Value != null && !e.Value.ToString().Contains("👑"))
                        e.Value = "👑 " + e.Value;
                }
                else if (e.RowIndex == 1) // Top 2: Bạc
                {
                    row.DefaultCellStyle.ForeColor = Color.Silver;
                    row.DefaultCellStyle.Font = new Font("Consolas", 11, FontStyle.Bold);
                }
                else if (e.RowIndex == 2) // Top 3: Đồng
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(205, 127, 50);
                    row.DefaultCellStyle.Font = new Font("Consolas", 11, FontStyle.Bold);
                }
            }
        }
    }
}