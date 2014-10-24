using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FTDI_Testing_App
{



    public static class Extensions
    {
        public static void Clear(this RichTextBox RTB)
        {
            RTB.Document.Blocks.Clear();
        }
    }
}
