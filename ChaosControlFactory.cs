using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AnyTeller.Services
{
    public class ChaosControlFactory
    {
        private Random _random = new Random();

        public Control CreateInputControl(string fieldName, string currentValue, bool usePlaceholder, out Label label)
        {
            label = null;
            string randomId = GetRandomString(8);
            Control inputControl = new TextBox { Text = currentValue, Name = "txt_" + randomId, Width = 200 };

            if (usePlaceholder)
            {
                ((TextBox)inputControl).PlaceholderText = fieldName;
            }
            else
            {
                label = new Label { Text = fieldName, AutoSize = true, Name = "lbl_" + GetRandomString(8) };
            }

            return inputControl;
        }

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }

        public Control WrapIncontainer(Control control, Label label)
        {
            // Randomly nest in Panel or UserControl or just return as is (if we want to vary depth).
            // But we need to return a single container that holds both (if label exists) or just the control.
            
            Panel container = new Panel();
            container.AutoSize = true;
            container.Name = "pnl" + control.Name;

            // Randomize positions within this mini-container? 
            // The "Spatial Randomization" applies to the specific Location within the "Active GroupBox".
            // So this wrapper is just for "Control Tree Complexity".
            
            if (label != null)
            {
                // Randomly place label above or below? 
                if (_random.Next(2) == 0) // Above
                {
                    label.Location = new Point(0, 0);
                    control.Location = new Point(0, label.Height + 5);
                }
                else // Left
                {
                     label.Location = new Point(0, 5);
                     control.Location = new Point(label.Width + 5, 0);
                }
                container.Controls.Add(label);
                container.Controls.Add(control);
            }
            else
            {
                control.Location = new Point(0, 0);
                container.Controls.Add(control);
            }

            // Nesting logic
            int depth = _random.Next(0, 3); // 0 to 2 extra layers
            Control current = container;
            for(int i=0; i<depth; i++)
            {
                Panel wrapper = new Panel();
                wrapper.AutoSize = true;
                wrapper.Controls.Add(current);
                current.Location = new Point(0, 0);
                current = wrapper;
            }

            return current;
        }
    }
}
