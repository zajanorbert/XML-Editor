using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XML_Editor
{
    class Validation
    {
        public static void ValidationReport(string fileName, ListBox listBox, int _issueCounter, List<string> _validationComments)
        {


            listBox.Items.Add("|=======================================|");
            if (_issueCounter > 0)
            {
                listBox.Items.Add(fileName + " is not valid");
            }
            else
            {
                listBox.Items.Add(fileName + " is valid");
            }
            //listBox1.Items.Add("Xml {0}", _issueCounter > 0 ? "is not valid" : "is valid");
            if (_issueCounter > 0)
            {
                listBox.Items.Add("Warnings or Errors: " + _issueCounter);
                foreach (var comment in _validationComments) { listBox.Items.Add(comment); }
            }
            listBox.Items.Add("|=======================================|");
        }
    }


}
