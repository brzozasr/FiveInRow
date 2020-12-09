using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow(WindowType popup) : base(Gtk.WindowType.Popup)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}