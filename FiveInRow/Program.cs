using System;
using Gtk;

namespace FiveInRow
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow(WindowType.Popup);
            win.Show();
            // new Board(10, 10);
            //new SettingWindow();
            Application.Run();
        }
    }
}