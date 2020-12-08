using Gtk;

namespace FiveInRow
{
    public class Intro : Window
    {
        public Intro() : base("INTRO")
        {
            SetDefaultSize(600, 400);
            SetPosition(WindowPosition.Center);
            DeleteEvent += delegate { Application.Quit(); };


            //introLogo.Pixbuf = new Gdk.Pixbuf("/FiveInRow/Properties/Resources/intro.png");
            Image introLogo = new Image();
            var pixbuf = new Gdk.Pixbuf("\\FiveInRow\\Properties\\Resources\\intro.png");
            introLogo.Pixbuf = pixbuf;

            ShowAll();
        }
    }
}