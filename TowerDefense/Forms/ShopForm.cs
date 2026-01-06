using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Data;
using TowerDefense.Utils;

namespace TowerDefense.Forms
{
    public partial class ShopForm : Form
    {
        private Label _lblDiamonds;
        private FlowLayoutPanel _pnlContainer;

        public ShopForm()
        {
            // 1. Gọi hàm của Visual Studio (trong Designer.cs)
            // Dòng này bắt buộc phải có để khởi tạo Form
            InitializeComponent();

            // 2. Thiết lập giao diện tùy chỉnh của chúng ta
            SetupUI();

            // 3. Tải danh sách vật phẩm
            LoadShopItems();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "BLACKSMITH SHOP";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 40);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(20, 20, 30) };
            this.Controls.Add(pnlHeader);

            Label lblTitle = new Label
            {
                Text = "ARMORY & UPGRADES",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            pnlHeader.Controls.Add(lblTitle);

            _lblDiamonds = new Label
            {
                Text = "💎 0",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.Cyan,
                AutoSize = true,
                Location = new Point(600, 25)
            };
            pnlHeader.Controls.Add(_lblDiamonds);

            // Container (Danh sách cuộn)
            _pnlContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(40, 40, 50),
                Padding = new Padding(20)
            };
            this.Controls.Add(_pnlContainer);

            // Footer
            Panel pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.FromArgb(20, 20, 30) };
            this.Controls.Add(pnlFooter);

            GameButton btnBack = new GameButton
            {
                Text = "BACK TO MENU",
                Size = new Size(200, 50),
                Location = new Point(300, 10),
                Color1 = Color.DarkRed,
                Color2 = Color.Maroon
            };
            btnBack.Click += (s, e) => this.Close();
            pnlFooter.Controls.Add(btnBack);
        }

        private void LoadShopItems()
        {
            _pnlContainer.Controls.Clear();
            UpdateDiamondDisplay();

            // --- CÁC MỤC NÂNG CẤP ---

            // ARCHER
            AddSectionHeader("ARCHER TOWER");
            AddItem("Sharp Arrows", "ArcherDamage", "Tăng sát thương gốc (+5 Dmg)", Color.Blue);
            AddItem("Eagle Eye", "ArcherRange", "Tăng tầm bắn (+10 Range)", Color.LightBlue);
            AddItem("Quick Draw", "ArcherSpeed", "Tăng tốc độ bắn (-5% Cooldown)", Color.DeepSkyBlue);

            // CANNON
            AddSectionHeader("CANNON TOWER");
            AddItem("Heavy Gunpowder", "CannonDamage", "Tăng sát thương nổ (+10 Dmg)", Color.Black);
            AddItem("Long Barrel", "CannonRange", "Tăng tầm bắn (+10 Range)", Color.Gray);

            // SNIPER
            AddSectionHeader("SNIPER TOWER");
            AddItem("Headshot Mastery", "SniperDamage", "Tăng sát thương cực đại (+20 Dmg)", Color.ForestGreen);

            // CONSUMABLES
            AddSectionHeader("CONSUMABLES");
            AddItem("Repair Kit", "BaseRepair", "Hồi phục 5 máu cho nhà chính.", Color.Red);
        }

        private void AddSectionHeader(string title)
        {
            Label lbl = new Label
            {
                Text = title,
                Font = new Font("Arial", 14, FontStyle.Bold | FontStyle.Underline),
                ForeColor = Color.Yellow,
                AutoSize = false,
                Width = 700,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 20, 0, 10)
            };
            _pnlContainer.Controls.Add(lbl);
        }

        private void AddItem(string name, string key, string desc, Color themeColor)
        {
            int currentLevel = GetCurrentLevel(key);
            int cost = 50 + (currentLevel * 25);
            if (key == "BaseRepair") { cost = 100; currentLevel = 0; }

            Panel pnlItem = new Panel
            {
                Size = new Size(720, 100),
                BackColor = Color.FromArgb(60, 60, 80),
                Margin = new Padding(0, 0, 0, 10)
            };

            pnlItem.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlItem.ClientRectangle, themeColor, 4, ButtonBorderStyle.Solid, themeColor, 4, ButtonBorderStyle.Solid, themeColor, 4, ButtonBorderStyle.Solid, themeColor, 4, ButtonBorderStyle.Solid);

            Label lblName = new Label { Text = name, Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = themeColor, Location = new Point(20, 15), AutoSize = true };
            pnlItem.Controls.Add(lblName);

            Label lblDesc = new Label { Text = desc, Font = new Font("Arial", 10, FontStyle.Italic), ForeColor = Color.WhiteSmoke, Location = new Point(20, 45), AutoSize = true };
            pnlItem.Controls.Add(lblDesc);

            string lvText = (key == "BaseRepair") ? "Consumable" : $"Current Lv: {currentLevel}";
            Label lblLevel = new Label { Text = lvText, Font = new Font("Arial", 10, FontStyle.Bold), ForeColor = Color.Gold, Location = new Point(400, 40), AutoSize = true };
            pnlItem.Controls.Add(lblLevel);

            GameButton btnBuy = new GameButton
            {
                Text = $"{cost} 💎",
                Size = new Size(120, 50),
                Location = new Point(580, 25),
                Color1 = Color.Green,
                Color2 = Color.DarkGreen,
                BorderRadius = 15
            };

            btnBuy.Click += (s, e) => { if (ProcessPurchase(key, cost)) LoadShopItems(); };

            if (UserProfile.Instance.Diamonds < cost) { btnBuy.Color1 = Color.Gray; btnBuy.Color2 = Color.Black; btnBuy.Enabled = false; }

            pnlItem.Controls.Add(btnBuy);
            _pnlContainer.Controls.Add(pnlItem);
        }

        private void UpdateDiamondDisplay()
        {
            _lblDiamonds.Text = $"💎 {UserProfile.Instance.Diamonds}";
        }

        private int GetCurrentLevel(string key)
        {
            var p = UserProfile.Instance;
            switch (key)
            {
                case "ArcherDamage": return p.ArcherDamageLevel;
                case "ArcherRange": return p.ArcherRangeLevel;
                case "ArcherSpeed": return p.ArcherSpeedLevel;
                case "CannonDamage": return p.CannonDamageLevel;
                case "CannonRange": return p.CannonRangeLevel;
                case "SniperDamage": return p.SniperDamageLevel;
                default: return 0;
            }
        }

        private bool ProcessPurchase(string key, int cost)
        {
            var p = UserProfile.Instance;
            if (p.Diamonds < cost) return false;

            p.Diamonds -= cost;

            switch (key)
            {
                case "ArcherDamage": p.ArcherDamageLevel++; break;
                case "ArcherRange": p.ArcherRangeLevel++; break;
                case "ArcherSpeed": p.ArcherSpeedLevel++; break;
                case "CannonDamage": p.CannonDamageLevel++; break;
                case "CannonRange": p.CannonRangeLevel++; break;
                case "SniperDamage": p.SniperDamageLevel++; break;
                case "BaseRepair": MessageBox.Show("Repair Kit Bought!"); break;
            }

            UserProfile.Save();
            Managers.SoundManager.Play("upgrade");
            return true;
        }
    }
}