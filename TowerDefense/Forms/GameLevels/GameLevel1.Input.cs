using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Data;
using TowerDefense.Entities.Towers;
using TowerDefense.Configs;
using TowerDefense.Utils; // Để dùng GameButton

namespace TowerDefense.Forms.GameLevels
{
    public partial class GameLevel1
    {
        // =========================================================
        // 0. HÀM HỖ TRỢ TỌA ĐỘ
        // =========================================================
        private Point GetLogicMousePos(int mouseX, int mouseY)
        {
            // Chuyển đổi tọa độ chuột nếu có scale (mặc định scale = 1.0)
            return new Point((int)(mouseX / _scaleFactor), (int)(mouseY / _scaleFactor));
        }

        // =========================================================
        // 1. KHỞI TẠO UI ĐỘNG
        // =========================================================

        // --- TẠO DANH SÁCH SÚNG (SIDEBAR LIST) ---
        private void InitializeTowerSelector()
        {
            if (_flowTowerPanel == null) return;
            _flowTowerPanel.Controls.Clear();
            _flowTowerPanel.Padding = new Padding(10, 5, 0, 0); // Căn lề

            // Duyệt qua tất cả loại tháp trong Config
            for (int i = 0; i < GameConfig.Towers.Length; i++)
            {
                var stat = GameConfig.Towers[i];

                GameButton btn = new GameButton();

                // Kích thước chuẩn cho Sidebar 185px (2 cột)
                btn.Size = new Size(75, 75);
                btn.BorderRadius = 15;
                btn.Tag = i; // Lưu ID tháp
                btn.Margin = new Padding(3);

                // Set Hình ảnh
                Image towerImg = ResourceManager.GetImage(stat.Name);
                if (towerImg != null)
                {
                    btn.Image = ResizeImage(towerImg, 32, 32);
                    btn.TextImageRelation = TextImageRelation.ImageAboveText;
                }

                // Set Giá tiền
                btn.Text = $"{stat.Price}";
                btn.Font = new Font("Arial", 9, FontStyle.Bold);
                btn.ForeColor = Color.White;

                // Màu mặc định
                btn.Color1 = ControlPaint.Dark(stat.Color);
                btn.Color2 = Color.Black;

                // Sự kiện Click
                btn.Click += (s, e) => SelectTower((int)btn.Tag);

                _flowTowerPanel.Controls.Add(btn);
            }
        }

        private Image ResizeImage(Image img, int w, int h)
        {
            Bitmap b = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, w, h);
            }
            return b;
        }

        // Cập nhật trạng thái nút (Đỏ nếu thiếu tiền)
        public void RefreshTowerButtons()
        {
            if (_flowTowerPanel == null) return;

            foreach (Control c in _flowTowerPanel.Controls)
            {
                if (c is GameButton btn)
                {
                    int id = (int)btn.Tag;
                    var stat = GameConfig.Towers[id];
                    int currentMoney = GameManager.Instance.PlayerMoney;

                    if (GameManager.Instance.SelectedTowerType == id) continue;

                    if (currentMoney < stat.Price)
                    {
                        btn.ForeColor = Color.Red;
                        btn.Color1 = Color.FromArgb(60, 60, 60);
                    }
                    else
                    {
                        btn.ForeColor = Color.White;
                        btn.Color1 = ControlPaint.Dark(stat.Color);
                    }
                    btn.Invalidate();
                }
            }
        }

        // --- TẠO BẢNG NÂNG CẤP (UPGRADE PANEL) ---
        // --- 1. SỬA HÀM KHỞI TẠO UI ---
        private void InitializeDynamicControls()
        {
            // Thay vì tạo Panel, ta tạo các control và add thẳng vào Form
            // Mặc định Visible = false (Ẩn đi)

            _lblTowerInfo = new Label { ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Font = new Font("Arial", 9, FontStyle.Bold), Visible = false };
            this.Controls.Add(_lblTowerInfo);

            _btnUpgrade = new Button { Text = "UPGRADE", Size = new Size(80, 40), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat, Visible = false };
            _btnUpgrade.Click += (s, e) => PerformUpgrade();
            this.Controls.Add(_btnUpgrade);

            _btnSell = new Button { Text = "SELL", Size = new Size(80, 40), BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Visible = false };
            _btnSell.Click += (s, e) => PerformSell();
            this.Controls.Add(_btnSell);

            _btnCloseMenu = new Button { Text = "x", Size = new Size(20, 20), BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Visible = false };
            _btnCloseMenu.Click += (s, e) => DeselectTower();
            this.Controls.Add(_btnCloseMenu);
        }

        // =========================================================
        // 2. XỬ LÝ CLICK & DI CHUYỂN CHUỘT
        // =========================================================

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPaused) return;

            Point logicPos = GetLogicMousePos(e.X, e.Y);
            int logicX = logicPos.X;
            int logicY = logicPos.Y;

            // KIỂM TRA VÙNG BẢN ĐỒ (X < 800)
            if (logicX < 800)
            {
                _isHoveringBoard = true;

                int gridSize = GameConstants.TILE_SIZE;
                int snapX = (logicX / gridSize) * gridSize + GameConstants.HALF_TILE;
                int snapY = (logicY / gridSize) * gridSize + GameConstants.HALF_TILE;

                _ghostPos = new Point(snapX, snapY);

                // --- CHECK ID THÁP ---
                int typeId = GameManager.Instance.SelectedTowerType;
                if (typeId < 0 || typeId >= GameConfig.Towers.Length)
                {
                    _isGhostValid = false;
                    this.Invalidate();
                    return;
                }

                bool positionOk = GameManager.Instance.CanPlaceTower(snapX, snapY);
                var stat = GameConfig.Towers[typeId];
                bool moneyOk = GameManager.Instance.PlayerMoney >= stat.Price;

                _isGhostValid = positionOk && moneyOk;
            }
            else
            {
                _isHoveringBoard = false; // Chuột ở Sidebar -> Không vẽ bóng ma
            }

            RefreshTowerButtons();
            this.Invalidate();
        }

        private void OnBoardClick(object sender, MouseEventArgs e)
        {
            if (_isPaused) return;

            Point logicPos = GetLogicMousePos(e.X, e.Y);
            int logicX = logicPos.X;
            int logicY = logicPos.Y;

            // CHUỘT PHẢI -> HỦY CHỌN
            if (e.Button == MouseButtons.Right)
            {
                SelectTower(-1);
                DeselectTower();
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            // 1. Check Click Tháp cũ (Nâng cấp)
            foreach (var tower in GameManager.Instance.Towers)
            {
                if (Math.Abs(tower.X - logicX) < 20 && Math.Abs(tower.Y - logicY) < 20)
                {
                    SelectTowerOnBoard(tower);
                    SelectTower(-1);
                    return;
                }
            }

            // 2. Check Xây Mới (Chỉ trong vùng Map X < 800)
            if (logicX >= 800) return;

            DeselectTower();

            if (GameManager.Instance.SelectedTowerType == -1) return;

            int gridSize = GameConstants.TILE_SIZE;
            int snapX = (logicX / gridSize) * gridSize + GameConstants.HALF_TILE;
            int snapY = (logicY / gridSize) * gridSize + GameConstants.HALF_TILE;

            if (GameManager.Instance.CanPlaceTower(snapX, snapY))
            {
                if (GameManager.Instance.TryBuildTower(snapX, snapY))
                {
                    SoundManager.Play("build");
                    GameManager.Instance.ShowFloatingText("-$$$", snapX, snapY - 20, Color.Yellow);
                }
                else
                {
                    GameManager.Instance.ShowFloatingText("Need Gold!", snapX, snapY, Color.Red);
                }
            }
            else
            {
                GameManager.Instance.ShowFloatingText("Block!", snapX, snapY, Color.Red);
            }
        }

        // =========================================================
        // 3. LOGIC CHỌN THÁP (SELECTOR)
        // =========================================================
        private void SelectTower(int typeId)
        {
            if (GameManager.Instance.SelectedTowerType == typeId)
                GameManager.Instance.SelectedTowerType = -1;
            else
                GameManager.Instance.SelectedTowerType = typeId;

            if (_flowTowerPanel != null)
            {
                foreach (Control c in _flowTowerPanel.Controls)
                {
                    if (c is GameButton btn)
                    {
                        int id = (int)btn.Tag;
                        var stat = GameConfig.Towers[id];

                        if (id == GameManager.Instance.SelectedTowerType)
                        {
                            btn.Color1 = stat.Color;
                            btn.Color2 = ControlPaint.Light(stat.Color);
                        }
                        else
                        {
                            btn.Color1 = ControlPaint.Dark(stat.Color);
                            btn.Color2 = Color.Black;
                        }
                        btn.Invalidate();
                    }
                }
            }
            this.Focus();
        }

        // =========================================================
        // 4. LOGIC NÂNG CẤP / BÁN
        // =========================================================

        // --- 2. SỬA HÀM HIỆN MENU KHI CLICK THÁP ---
        private void SelectTowerOnBoard(Tower tower)
        {
            _selectedTower = tower;
            int pX = (int)(tower.X * _scaleFactor) + 25;
            int pY = (int)(tower.Y * _scaleFactor) - 50;

            // Kẹp vào màn hình Map (X < 800)
            if (pX + 200 > 800) pX = (int)(tower.X * _scaleFactor) - 225;
            if (pY < 0) pY = 0;
            if (pY + 110 > 600) pY = 600 - 110;

            // Lưu vị trí khung nền để bên Render vẽ
            _upgradeMenuRect = new Rectangle(pX, pY, 200, 110);

            // Cập nhật vị trí các nút bấm theo vị trí khung
            _lblTowerInfo.Location = new Point(pX + 10, pY + 10);
            _btnUpgrade.Location = new Point(pX + 10, pY + 40);
            _btnSell.Location = new Point(pX + 100, pY + 40);
            _btnCloseMenu.Location = new Point(pX + 175, pY + 5);

            // Hiện các nút lên và đưa lên trên cùng
            _lblTowerInfo.Visible = true; _lblTowerInfo.BringToFront();
            _btnUpgrade.Visible = true; _btnUpgrade.BringToFront();
            _btnSell.Visible = true; _btnSell.BringToFront();
            _btnCloseMenu.Visible = true; _btnCloseMenu.BringToFront();

            UpdateUpgradeUI();
        }

        // --- 3. SỬA HÀM ẨN MENU ---
        private void DeselectTower()
        {
            _selectedTower = null;
            // Ẩn tất cả nút đi
            if (_lblTowerInfo != null) _lblTowerInfo.Visible = false;
            if (_btnUpgrade != null) _btnUpgrade.Visible = false;
            if (_btnSell != null) _btnSell.Visible = false;
            if (_btnCloseMenu != null) _btnCloseMenu.Visible = false;

            // Xóa vùng vẽ (đặt về rỗng)
            _upgradeMenuRect = Rectangle.Empty;
        }

        private void UpdateUpgradeUI()
        {
            if (_selectedTower == null) return;

            _lblTowerInfo.Text = $"{_selectedTower.Name}\nLv.{_selectedTower.Level} | Dmg: {_selectedTower.BaseDamage}";

            if (_selectedTower.Level >= 3)
            {
                _btnUpgrade.Text = "MAXED";
                _btnUpgrade.Enabled = false;
                _btnUpgrade.BackColor = Color.Gray;
            }
            else
            {
                _btnUpgrade.Text = $"UPGRADE\n{_selectedTower.UpgradeCost} G";
                _btnUpgrade.Enabled = GameManager.Instance.PlayerMoney >= _selectedTower.UpgradeCost;
                _btnUpgrade.BackColor = _btnUpgrade.Enabled ? Color.LightGreen : Color.Gray;
            }

            _btnSell.Text = $"SELL\n+{_selectedTower.SellValue} G";
        }

        private void PerformUpgrade()
        {
            if (_selectedTower != null && GameManager.Instance.PlayerMoney >= _selectedTower.UpgradeCost)
            {
                GameManager.Instance.PlayerMoney -= _selectedTower.UpgradeCost;
                _selectedTower.Upgrade();
                SoundManager.Play("upgrade");
                GameManager.Instance.ShowFloatingText("UPGRADED!", _selectedTower.X, _selectedTower.Y - 30, Color.Cyan);
                UpdateUpgradeUI();
            }
        }

        private void PerformSell()
        {
            if (_selectedTower != null)
            {
                GameManager.Instance.PlayerMoney += _selectedTower.SellValue;
                GameManager.Instance.ShowFloatingText($"+{_selectedTower.SellValue} G", _selectedTower.X, _selectedTower.Y, Color.Gold);
                GameManager.Instance.Towers.Remove(_selectedTower);
                SoundManager.Play("build");
                DeselectTower();
            }
        }

        // =========================================================
        // 5. CÁC SỰ KIỆN NÚT BẤM
        // =========================================================

        private void BtnStartWave_Click(object sender, EventArgs e)
        {
            if (!GameManager.Instance.WaveMgr.IsWaveRunning)
            {
                GameManager.Instance.WaveMgr.StartNextWave();
                _btnStartWave.Enabled = false;
                _btnStartWave.Text = "RUNNING...";
                _btnStartWave.BackColor = Color.Orange;
            }
            this.Focus();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            _isPaused = !_isPaused;
            ((Button)sender).Text = _isPaused ? ">" : "II";
            this.Focus();
        }

        private void BtnSpeed_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.GameSpeed == 1.0f)
            {
                GameManager.Instance.GameSpeed = 2.0f;
                ((GameButton)sender).Text = "x2"; ((GameButton)sender).BackColor = Color.OrangeRed;
            }
            else
            {
                GameManager.Instance.GameSpeed = 1.0f;
                ((GameButton)sender).Text = "x1"; ((GameButton)sender).BackColor = Color.Khaki;
            }
            this.Focus();
        }

        private void BtnAutoWave_Click(object sender, EventArgs e)
        {
            GameManager.Instance.IsAutoWave = !GameManager.Instance.IsAutoWave;
            if (GameManager.Instance.IsAutoWave)
            {
                ((GameButton)sender).Text = "AUTO: ON"; ((GameButton)sender).BackColor = Color.Green;
            }
            else
            {
                ((GameButton)sender).Text = "AUTO: OFF"; ((GameButton)sender).BackColor = Color.Gray;
            }
            this.Focus();
        }

        private void BtnMeteor_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.SkillMgr.CastMeteor())
                GameManager.Instance.ShowFloatingText("METEOR!!!", 400, 300, Color.OrangeRed);
            this.Focus();
        }

        private void BtnFreeze_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.SkillMgr.CastFreeze())
                GameManager.Instance.ShowFloatingText("FREEZE!!!", 400, 300, Color.Cyan);
            this.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveLoadSystem.SaveGame();
            MessageBox.Show("Game Saved!");
            this.Focus();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            SaveLoadSystem.LoadGame();
            MessageBox.Show("Game Loaded!");
            this.Focus();
        }
    }
}