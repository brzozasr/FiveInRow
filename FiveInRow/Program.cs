using System;
using Gtk;
using System.Threading;

namespace FiveInRow
{
    class MainClass
    {
        private static ThreadNotify _notify;
        private static MainWindow _win;

        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow(WindowType.Popup);
            win.Show();
            _win = win;

            Thread thr = new Thread(new ThreadStart(ThreadRoutine));
            thr.Start();
            _notify = new ThreadNotify(new ReadyEvent(ThreadCounter));

            //new ConfigGameWindow();
            Application.Run();
        }

        static void ThreadRoutine()
        {
            _notify.WakeupMain();
        }

        private static void ThreadCounter()
        {
            Thread.Sleep(4000);
            _win.HideAll();
            new ConfigGameWindow();
        }
    }
}