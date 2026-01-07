using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TowerDefense.Utils
{
    public class CyberButton : Control
    {
        private bool _isHover = false;
        private bool _isDown = false;

        // Màu sắc cấu hình
        public Color DefaultColor { get; set; } = Color.FromArgb(20, 30, 40);
        public Color HoverColor { get; set; } = Color.FromArgb(0, 60, 80);
        public Color BorderColor { get; set; } = Color.Cyan;

        public CyberButton()
        {
            // ================================================================
            // [FIX LỖI] Kích hoạt chế độ hỗ trợ nền trong suốt
            // Dòng này BẮT BUỘC phải có trước khi gán BackColor = Transparent
            // ================================================================
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            // ================================================================

            this.Size = new Size(260, 55);
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.ForeColor = Color.White;

            // Giờ gán dòng này mới không bị lỗi
            this.BackColor = Color.Transparent;

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

            // 1. Xác định màu
            Color c1, c2;
            if (_isDown)
            {
                c1 = Color.FromArgb(0, 100, 100);
                c2 = Color.FromArgb(0, 40, 40);
            }
            else if (_isHover)
            {
                c1 = HoverColor;
                c2 = Color.Black;
            }
            else
            {
                c1 = DefaultColor;
                c2 = Color.Black;
            }

            // 2. Tạo hình vát góc
            int cut = 12;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(cut, 0, r.Width, 0);
            path.AddLine(r.Width, 0, r.Width, r.Height - cut);
            path.AddLine(r.Width - cut, r.Height, 0, r.Height);
            path.AddLine(0, r.Height, 0, cut);
            path.CloseFigure();

            // 3. Vẽ nền Gradient
            using (LinearGradientBrush brush = new LinearGradientBrush(r,
                   Color.FromArgb(220, c1),
                   Color.FromArgb(240, c2),
                   LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(brush, path);
            }

            // 4. Họa tiết Deco (Hover mới hiện)
            if (_isHover)
            {
                using (SolidBrush decoBrush = new SolidBrush(BorderColor))
                {
                    g.FillRectangle(decoBrush, 0, cut + 5, 4, Height - (cut * 2) - 10);
                }
            }

            // 5. Viền Neon
            using (Pen p = new Pen(_isHover ? Color.White : BorderColor, _isHover ? 2 : 1))
            {
                g.DrawPath(p, path);
            }

            // 6. Vẽ chữ
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Bóng chữ
            Rectangle shadowRect = new Rectangle(r.X + 2, r.Y + 2, r.Width, r.Height);
            g.DrawString(Text, Font, Brushes.Black, shadowRect, sf);

            // Chữ chính
            Brush textBrush = _isHover ? Brushes.Cyan : Brushes.LightGray;
            g.DrawString(Text, Font, textBrush, r, sf);
        }
    }
}