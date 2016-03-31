using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TextEditor.Visual
{
    class SelectionTextRunProperties : CustomTextRunProperties
    {
        public SelectionTextRunProperties(TextEditorConfiguration configuration) : base(configuration)
        {
        }

        public override Brush BackgroundBrush
        {
            get
            {
                return Brushes.LightBlue;
            }
        }
    }
}
