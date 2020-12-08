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
            //new Board(6, 6);
            Window intro = new Intro();
            intro.Resizable = false;
            intro.Decorated = false;
            intro.Show();
            Application.Run();
        }
    }
}