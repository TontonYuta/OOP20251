using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Utils; // Dùng GameButton

namespace TowerDefense.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent() { } // Hàm giả

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "ABOUT THE GAME";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 40);

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
                Text = "DEFENSE OF THE TOWER",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(130, 20),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitle);

            // Container chứa nội dung (Cuộn được)
            FlowLayoutPanel pnlContent = new FlowLayoutPanel
            {
                Location = new Point(30, 80),
                Size = new Size(640, 400),
                BackColor = Color.FromArgb(100, 0, 0, 0), // Đen mờ
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(20)
            };
            this.Controls.Add(pnlContent);

            // --- THÊM NỘI DUNG ---
            AddHeader(pnlContent, "1. STORY (CỐT TRUYỆN)");
            AddText(pnlContent, "Vương quốc đang bị tấn công bởi quân đoàn quái vật hung hãn. Là chỉ huy tối cao, bạn phải xây dựng hệ thống phòng thủ tháp để ngăn chặn chúng xâm nhập vào Base, bảo vệ người dân của mình.");

            AddHeader(pnlContent, "2. HOW TO PLAY (CÁCH CHƠI)");
            AddText(pnlContent, "- Dùng chuột để tương tác");
            AddText(pnlContent, "- Sử dụng Vàng (Gold) để xây tháp.");
            AddText(pnlContent, "- Click vào tháp đã xây để Nâng cấp hoặc Bán.");
            AddText(pnlContent, "- Ngăn chặn quái đi hết bản đồ. Nếu mạng về 0, bạn thua.");
            AddText(pnlContent, "- Có thể sử dụng Kỹ năng (Meteor, Freeze) khi nguy cấp.");

            AddHeader(pnlContent, "3. FEATURES (TÍNH NĂNG)");
            AddText(pnlContent, "★ 4 Bản đồ với độ khó tăng dần (Cỏ, Cát, Tuyết, Lửa).");
            AddText(pnlContent, "★ 10 Loại tháp: Archer, Cannon, Sniper, Ice, Magic...");
            AddText(pnlContent, "★ 20 Loại quái: Nhiều loại quái đa dạng với chỉ số sức mạnh khác nhau");
            AddText(pnlContent, "★ Hệ thống Shop: Dùng Kim Cương để nâng cấp sức mạnh vĩnh viễn, mua bổ trợ.");

            AddHeader(pnlContent, "4. CREDITS");
            AddText(pnlContent, "Developed by: Nhóm 24( Nguyễn Ngọc Hoàng, Trần Huy Hoàng)");
            AddText(pnlContent, "Tools: Visual Studio 2022, C# WinForms.");

            // Khoảng trống cuối cùng
            Label spacer = new Label { Size = new Size(10, 50) };
            pnlContent.Controls.Add(spacer);

            // Nút Đóng
            GameButton btnBack = new GameButton
            {
                Text = "CLOSE",
                Size = new Size(150, 50),
                Location = new Point(275, 500),
                Color1 = Color.DarkRed,
                Color2 = Color.Maroon
            };
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);
        }

        private void AddHeader(FlowLayoutPanel pnl, string text)
        {
            Label lbl = new Label
            {
                Text = text,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Cyan,
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 5),
                BackColor = Color.Transparent
            };
            pnl.Controls.Add(lbl);
        }

        private void AddText(FlowLayoutPanel pnl, string text)
        {
            Label lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.WhiteSmoke,
                AutoSize = true,
                MaximumSize = new Size(580, 0), // Tự xuống dòng
                Margin = new Padding(0, 0, 0, 5),
                BackColor = Color.Transparent
            };
            pnl.Controls.Add(lbl);
        }
    }
}