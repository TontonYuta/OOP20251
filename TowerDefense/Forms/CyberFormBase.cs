using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Utils;
using TowerDefense.Managers;

namespace TowerDefense.Forms
{
    // Form cha: Mọi form khác sẽ kế thừa từ đây
    public class CyberFormBase : Form
    {
        protected Label lblTitle;
        private Label btnClose;
        private bool _dragging = false;
        private Point _dragCursorPoint;
        private Point _dragFormPoint;

        public CyberFormBase()
        {
            // Cấu hình chung
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(10, 10, 15); // Đen tím than
            this.DoubleBuffered = true;
            this.Padding = new Padding(2); // Để chừa chỗ vẽ viền

            // Tiêu đề
            lblTitle = new Label
            {
                Text = "SYSTEM WINDOW",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Cyan,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblTitle);

            // Nút Đóng (X)
            btnClose = new Label
            {
                Text = "X",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => { SoundManager.Play("click"); this.Close(); };
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.Red;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.Gray;
            this.Controls.Add(btnClose);

            // Xử lý kéo thả cửa sổ
            this.MouseDown += (s, e) => { _dragging = true; _dragCursorPoint = Cursor.Position; _dragFormPoint = this.Location; };
            this.MouseMove += (s, e) => {
                if (_dragging)
                {
                    Point dif = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
                    this.Location = Point.Add(_dragFormPoint, new Size(dif));
                }
            };
            this.MouseUp += (s, e) => { _dragging = false; };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Cập nhật vị trí nút đóng khi form load xong
            btnClose.Location = new Point(this.Width - 40, 15);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Vẽ viền Neon
            using (Pen p = new Pen(Color.Cyan, 2))
            {
                g.DrawRectangle(p, 1, 1, Width - 2, Height - 2);
            }

            // Vẽ đường kẻ dưới tiêu đề
            using (Pen p = new Pen(Color.FromArgb(50, 50, 60), 2))
            {
                g.DrawLine(p, 20, 50, Width - 20, 50);
            }
        }
    }
}