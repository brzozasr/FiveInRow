using System;
using GLib;
using Gtk;

namespace FiveInRow
{
    public class SettingWindow : Window
    {
        public SettingWindow() : base(WindowType.Toplevel)
        {
            HBox topVBox;
            HBox middleVBox;
            HBox bottomVBox;
            VBox container;
            Frame frame;
            
            SetDefaultSize(500, 300);
            SetPosition(WindowPosition.Center);
            DeleteEvent += delegate { Application.Quit(); };
            Title = "The Game Configuration";

            container = new VBox(false, 0);
            
            topVBox = new HBox (false, 10);
            container.PackStart(topVBox, false, false, 20);
            
            Button btn1 = new Button("TEST");
            btn1.WidthRequest = 50;
            topVBox.PackStart(btn1);

            middleVBox = new HBox (false, 10);
            container.PackStart(middleVBox);

            frame = new Frame("Game Mode");
            frame.Shadow = ShadowType.In;
            
            middleVBox.PackEnd(frame);

            bottomVBox = new HBox (false, 10); ;
            container.PackEnd(bottomVBox);
            
            Button btn3 = new Button("TEST");
            bottomVBox.PackStart(btn3);
            
            Add(container);

            // RadioButton radiobutton = new RadioButton  (null, "AI");
            // middleVBox.PackStart(radiobutton, true, true, 0);
            // radiobutton.Show();

            // HSeparator separator = new HSeparator ();
            // box1.PackStart (separator,false, true, 0);
            // separator.Show();
            //
            // VBox box3 = new VBox(false, 10);
            // box3.BorderWidth = 10;
            // box1.PackStart(box3,false, true, 0);
            // box3.Show();
            //
            // Button button = new Button ("close");
            // // button.Clicked += exitbutton_event;
            //
            // box3.PackStart(button, true, true, 0);
            // button.CanDefault = true;
            // button.GrabDefault();
            // button.Show();

            ShowAll();
        }
    }
}