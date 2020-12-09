using System;
namespace FiveInRow
{
    public partial class ConfigGameWindow : Gtk.Window
    {
        public ConfigGameWindow() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }
    }
}
