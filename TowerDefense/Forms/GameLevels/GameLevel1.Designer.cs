namespace TowerDefense.Forms.GameLevels
{
    partial class GameLevel1
    {
        private System.ComponentModel.IContainer components = null;

        // UI Controls
        private TowerDefense.Utils.GameButton _btnStartWave;
        private TowerDefense.Utils.GameButton _btnPause;
        private TowerDefense.Utils.GameButton _btnSave;
        private TowerDefense.Utils.GameButton _btnLoad;
        private TowerDefense.Utils.GameButton _btnSpeed;
        private TowerDefense.Utils.GameButton _btnAutoWave;

        // Skill Buttons
        private TowerDefense.Utils.GameButton _btnSkillMeteor;
        private TowerDefense.Utils.GameButton _btnSkillFreeze;

        // Container chọn tháp
        private System.Windows.Forms.FlowLayoutPanel _flowTowerPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._btnStartWave = new TowerDefense.Utils.GameButton();
            this._btnPause = new TowerDefense.Utils.GameButton();
            this._btnSave = new TowerDefense.Utils.GameButton();
            this._btnLoad = new TowerDefense.Utils.GameButton();
            this._btnSpeed = new TowerDefense.Utils.GameButton();
            this._btnAutoWave = new TowerDefense.Utils.GameButton();
            this._flowTowerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._btnSkillMeteor = new TowerDefense.Utils.GameButton();
            this._btnSkillFreeze = new TowerDefense.Utils.GameButton();
            this.SuspendLayout();

            // ============================================================
            // 1. NHÓM HỆ THỐNG (Top Right)
            // ============================================================

            // Speed
            this._btnSpeed.Text = "x1";
            this._btnSpeed.Location = new System.Drawing.Point(820, 10);
            this._btnSpeed.Size = new System.Drawing.Size(40, 40);
            this._btnSpeed.Color1 = System.Drawing.Color.Khaki;
            this._btnSpeed.Color2 = System.Drawing.Color.DarkKhaki;
            this._btnSpeed.BorderRadius = 10;
            this._btnSpeed.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this._btnSpeed.Click += new System.EventHandler(this.BtnSpeed_Click);

            // Pause
            this._btnPause.Text = "II";
            this._btnPause.Location = new System.Drawing.Point(870, 10);
            this._btnPause.Size = new System.Drawing.Size(40, 40);
            this._btnPause.Color1 = System.Drawing.Color.Gold;
            this._btnPause.Color2 = System.Drawing.Color.Goldenrod;
            this._btnPause.BorderRadius = 10;
            this._btnPause.Click += new System.EventHandler(this.BtnPause_Click);

            // Save
            this._btnSave.Text = "SAVE";
            this._btnSave.Location = new System.Drawing.Point(920, 10);
            this._btnSave.Size = new System.Drawing.Size(60, 40);
            this._btnSave.Color1 = System.Drawing.Color.Gray;
            this._btnSave.Color2 = System.Drawing.Color.DimGray;
            this._btnSave.BorderRadius = 10;
            this._btnSave.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this._btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // Load
            this._btnLoad.Text = "LOAD";
            this._btnLoad.Location = new System.Drawing.Point(920, 55);
            this._btnLoad.Size = new System.Drawing.Size(60, 40);
            this._btnLoad.Color1 = System.Drawing.Color.Gray;
            this._btnLoad.Color2 = System.Drawing.Color.DimGray;
            this._btnLoad.BorderRadius = 10;
            this._btnLoad.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this._btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);

            // ============================================================
            // 2. DANH SÁCH THÁP (Dọc bên phải)
            // ============================================================

            // --- FIX MÀU TRẮNG TẠI ĐÂY: Dùng Transparent ---
            this._flowTowerPanel.BackColor = System.Drawing.Color.Transparent;
            // -----------------------------------------------

            this._flowTowerPanel.AutoScroll = true;
            this._flowTowerPanel.Location = new System.Drawing.Point(810, 140);
            this._flowTowerPanel.Name = "_flowTowerPanel";
            this._flowTowerPanel.Size = new System.Drawing.Size(180, 280);
            this._flowTowerPanel.TabIndex = 5;
            this._flowTowerPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;

            // ============================================================
            // 3. SKILL & CONTROL (Dưới cùng bên phải)
            // ============================================================

            // Meteor
            this._btnSkillMeteor.Text = "METEOR";
            this._btnSkillMeteor.Location = new System.Drawing.Point(820, 440);
            this._btnSkillMeteor.Size = new System.Drawing.Size(75, 60);
            this._btnSkillMeteor.Color1 = System.Drawing.Color.OrangeRed;
            this._btnSkillMeteor.Color2 = System.Drawing.Color.Maroon;
            this._btnSkillMeteor.BorderRadius = 20;
            this._btnSkillMeteor.Click += new System.EventHandler(this.BtnMeteor_Click);

            // Freeze
            this._btnSkillFreeze.Text = "FREEZE";
            this._btnSkillFreeze.Location = new System.Drawing.Point(905, 440);
            this._btnSkillFreeze.Size = new System.Drawing.Size(75, 60);
            this._btnSkillFreeze.Color1 = System.Drawing.Color.Cyan;
            this._btnSkillFreeze.Color2 = System.Drawing.Color.Teal;
            this._btnSkillFreeze.BorderRadius = 20;
            this._btnSkillFreeze.Click += new System.EventHandler(this.BtnFreeze_Click);

            // Auto Wave
            this._btnAutoWave.Text = "AUTO: OFF";
            this._btnAutoWave.Location = new System.Drawing.Point(820, 510);
            this._btnAutoWave.Size = new System.Drawing.Size(160, 30);
            this._btnAutoWave.Color1 = System.Drawing.Color.SlateGray;
            this._btnAutoWave.Color2 = System.Drawing.Color.Black;
            this._btnAutoWave.BorderRadius = 10;
            this._btnAutoWave.Click += new System.EventHandler(this.BtnAutoWave_Click);

            // Start Wave
            this._btnStartWave.Text = "START WAVE";
            this._btnStartWave.Location = new System.Drawing.Point(820, 550);
            this._btnStartWave.Size = new System.Drawing.Size(160, 40);
            this._btnStartWave.Color1 = System.Drawing.Color.Orange;
            this._btnStartWave.Color2 = System.Drawing.Color.DarkOrange;
            this._btnStartWave.BorderRadius = 20;
            this._btnStartWave.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this._btnStartWave.Click += new System.EventHandler(this.BtnStartWave_Click);

            // 
            // GameLevel1 (FORM SETUP)
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            // --- FIX MÀU TRẮNG TOÀN CỤC: Set màu nền Form thành màu tối ---
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 40);
            // -------------------------------------------------------------

            this.ClientSize = new System.Drawing.Size(1000, 600);

            this.Controls.Add(this._flowTowerPanel);
            this.Controls.Add(this._btnAutoWave);
            this.Controls.Add(this._btnSpeed);
            this.Controls.Add(this._btnLoad);
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this._btnPause);
            this.Controls.Add(this._btnSkillFreeze);
            this.Controls.Add(this._btnSkillMeteor);
            this.Controls.Add(this._btnStartWave);

            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GameLevel1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tower Defense Legends";
            this.ResumeLayout(false);
        }

        #endregion
    }
}