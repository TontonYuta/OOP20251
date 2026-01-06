using System;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels; // Nhớ using namespace này

namespace TowerDefense
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Load ảnh trước
            TowerDefense.Managers.ResourceManager.LoadResources();

            // Chạy Main Menu
            Application.Run(new Forms.MainMenuForm());
        }
    }
}