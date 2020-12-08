using System;
using Gtk;

namespace FiveInRow
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            //MainWindow win = new MainWindow();
            //win.Show();
            // new Board(10, 10);
            new SettingWindow();
            Application.Run();
        }
    }
}