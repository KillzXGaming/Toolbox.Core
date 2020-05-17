using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.GUI
{
    [Controls.Panel(FlowLayout.Vertical)]
    public class TexturePanel
    {
        [Controls.ComboBox("Format", Align.Left)]
        public TextureFormats Value { get; set; }

        public enum TextureFormats
        {
            RGBA,
            RGB,
        }
    }

    public enum Align
    {
        Left,
        Right,
        Center,
    }

    public enum FlowLayout
    {
        Vertical,
        Horizontal,
    }

    public class PanelProperties
    {

    }

    public class Controls
    {
        [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
        public class ComboBox : Attribute
        {
            /// <summary>
            /// The label displayed next to the combo box.
            /// </summary>
            public string Label { get; set; }

            public ComboBox(string label = "", Align align = Align.Left)
            {
                Label = label;
            }
        }

        [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
        public class Panel : Attribute
        {
            public Panel(FlowLayout layout)
            {
            }
        }
    }
}
