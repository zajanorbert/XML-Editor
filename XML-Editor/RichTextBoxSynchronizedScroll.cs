using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XML_Editor
{
    class RichTextBoxSynchronizedScroll : RichTextBox
    {
        public event vScrollEventHandler vScroll;
        public delegate void vScrollEventHandler(Message message);

        public const int WM_VSCROLL = 0x115;
        

        protected override void WndProc(ref Message msg)
        {
            
            if (msg.Msg == WM_VSCROLL)
            {
                
                if (vScroll != null)
                {
                    vScroll(msg);
                    
                }
             
            }
            base.WndProc(ref msg);
        }

        public void PubWndProc(ref Message msg)
        {
            base.WndProc(ref msg);
        }
    }
}
