using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TowerDefense.Utils
{
    public class GameButton : Button
    {
        // =========================================================
        // 1. CẤU HÌNH MÀU SẮC & GIAO DIỆN
        // =========================================================
        public Color Color1 { get; set; } = Color.RoyalBlue;
        public Color Color2 { get; set; } = Color.MidnightBlue;
        public Color HoverColor1 { get; set; } = Color.DodgerBlue;
        public Color HoverColor2 { get; set; } = Color.Blue;
        public int BorderRadius { get; set; } = 20;

        // Trạng thái chuột
        private bool _isHovering = false;
        private bool _isPressed = false;

        // --- BIẾN CACHE (TỐI ƯU HÓA HIỆU NĂNG) ---
        // Lưu trữ hình dáng nút để không phải tính lại mỗi lần vẽ (gây lag)
        private GraphicsPath _cachedPath;
        private Region _cachedRegion;

        // =========================================================
        // 2. CONSTRUCTOR
        // =========================================================
        public GameButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 50);
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.Cursor = Cursors.Hand;

            // Bật Double Buffer để chống nháy hình
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.UserPaint, true);
        }

        // =========================================================
        // 3. TỐI ƯU HÓA HÌNH DÁNG (SHAPE CACHING)
        // =========================================================

        // Chỉ tính toán lại hình dáng khi kích thước thay đổi
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateShape();
        }

        private void UpdateShape()
        {
            if (this.Width == 0 || this.Height == 0) return;

            Rectangle rect = this.ClientRectangle;
            rect.Width--; rect.Height--;

            // Xóa cache cũ
            _cachedPath?.Dispose();
            _cachedRegion?.Dispose();

            // Tạo cache mới
            _cachedPath = GetRoundedPath(rect, BorderRadius);
            _cachedRegion = new Region(_cachedPath);

            // Cập nhật vùng tương tác (để chuột chỉ click được trong phần bo tròn)
            this.Region = _cachedRegion;
        }

        // =========================================================
        // 4. HÀM VẼ CHÍNH (ON PAINT)
        // =========================================================
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;

            // --- CẤU HÌNH ĐỒ HỌA TỐC ĐỘ CAO ---
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;

            // Nếu cache bị mất (hiếm khi), tạo lại
            if (_cachedPath == null) UpdateShape();

            // A. TÔ MÀU NỀN (GRADIENT)
            Color c1 = _isHovering ? HoverColor1 : Color1;
            Color c2 = _isHovering ? HoverColor2 : Color2;
            if (_isPressed) { c1 = Color2; c2 = Color1; } // Đảo màu khi nhấn

            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, c1, c2, 90F))
            {
                g.FillPath(brush, _cachedPath);
            }

            // B. VẼ VIỀN SÁNG (HIGHLIGHT)
            using (Pen pen = new Pen(Color.FromArgb(80, 255, 255, 255), 2))
            {
                g.DrawPath(pen, _cachedPath);
            }

            // C. VẼ NỘI DUNG (ẢNH VÀ CHỮ)
            if (this.Image != null)
            {
                // --- TRƯỜNG HỢP CÓ ẢNH (VẼ TRÊN - DƯỚI) ---
                int imgW = this.Image.Width;
                int imgH = this.Image.Height;

                // Căn giữa ảnh ở nửa trên
                int imgX = (this.Width - imgW) / 2;
                int imgY = 5;
                g.DrawImage(this.Image, imgX, imgY, imgW, imgH);

                // Vẽ chữ ở nửa dưới
                Rectangle textRect = new Rectangle(0, imgH + 5, this.Width, this.Height - imgH - 5);
                TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
            }
            else
            {
                // --- TRƯỜNG HỢP KHÔNG CÓ ẢNH (VẼ GIỮA) ---
                TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        // =========================================================
        // 5. CÁC HÀM HỖ TRỢ
        // =========================================================

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = radius * 2.0F;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Sự kiện chuột
        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _isHovering = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _isHovering = false; Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs mevent) { base.OnMouseDown(mevent); _isPressed = true; Invalidate(); }
        protected override void OnMouseUp(MouseEventArgs mevent) { base.OnMouseUp(mevent); _isPressed = false; Invalidate(); }
    }
}