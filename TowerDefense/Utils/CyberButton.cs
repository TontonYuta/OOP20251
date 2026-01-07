using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TowerDefense.Utils // <--- Namespace phải chuẩn là cái này
{
    public class CyberButton : Control
    {
        private bool _isHover = false;
        private bool _isDown = false;

        // Các thuộc tính màu sắc (để Menu và Setting gọi không bị lỗi)
        public Color DefaultColor { get; set; } = Color.FromArgb(20, 30, 40);
        public Color HoverColor { get; set; } = Color.FromArgb(0, 60, 80);
        public Color BorderColor { get; set; } = Color.Cyan;

        public CyberButton()
        {
            // Kích hoạt chế độ trong suốt
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.Size = new Size(260, 55);
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.ForeColor = Color.White;
            this.BackColor = Color.Transparent; // Nền trong suốt

            // Sự kiện chuột
            this.MouseEnter += (s, e) => { _isHover = true; Invalidate(); };
            this.MouseLeave += (s, e) => { _isHover = false; _isDown = false; Invalidate(); };
            this.MouseDown += (s, e) => { _isDown = true; Invalidate(); };
            this.MouseUp += (s, e) => { _isDown = false; Invalidate(); };
        }

        public CyberButton(string text) : this()
        {
            this.Text = text;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. Xác định màu nền
            Color c1, c2;
            if (_isDown) { c1 = Color.FromArgb(0, 100, 100); c2 = Color.FromArgb(0, 40, 40); }
            else if (_isHover) { c1 = HoverColor; c2 = Color.Black; }
            else { c1 = DefaultColor; c2 = Color.Black; }

            // 2. Vẽ hình dáng nút
            int cut = 12;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            GraphicsPath path = new GraphicsPath();
            path.AddLine(cut, 0, r.Width, 0);
            path.AddLine(r.Width, 0, r.Width, r.Height - cut);
            path.AddLine(r.Width - cut, r.Height, 0, r.Height);
            path.AddLine(0, r.Height, 0, cut);
            path.CloseFigure();

            // 3. Tô màu Gradient
            using (LinearGradientBrush brush = new LinearGradientBrush(r,
                   Color.FromArgb(220, c1), Color.FromArgb(240, c2),
                   LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(brush, path);
            }

            // 4. Vẽ họa tiết trang trí khi Hover
            if (_isHover)
            {
                using (SolidBrush decoBrush = new SolidBrush(BorderColor))
                    g.FillRectangle(decoBrush, 0, cut + 5, 4, Height - (cut * 2) - 10);
            }

            // 5. Vẽ viền Neon
            using (Pen p = new Pen(_isHover ? Color.White : BorderColor, _isHover ? 2 : 1))
                g.DrawPath(p, path);

            // 6. Vẽ chữ
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(Text, Font, Brushes.Black, new Rectangle(r.X + 2, r.Y + 2, r.Width, r.Height), sf); // Bóng chữ
            g.DrawString(Text, Font, _isHover ? Brushes.Cyan : Brushes.LightGray, r, sf); // Chữ chính
        }
    }
}