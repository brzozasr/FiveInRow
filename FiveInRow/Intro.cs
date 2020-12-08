using Atk;
using Gtk;
using Image = Gtk.Image;

namespace FiveInRow
{
    public class Intro : Window
    {
        public Intro() : base("INTRO")
        {
            SetDefaultSize(600, 400);
            SetPosition(WindowPosition.Center);
            DeleteEvent += delegate { Application.Quit(); };
            
            // var pixbuf = new Gdk.Pixbuf("intro.png");
            var pixbuf = new Gdk.Pixbuf (null, "Properties\\Resources\\intro.png");
            Image introLogo = new Image(pixbuf);
            Add(introLogo);
            ShowAll();
        }
    }
}

// Window intro = new Intro();
// intro.Resizable = false;
// intro.Decorated = false;
// intro.Show();