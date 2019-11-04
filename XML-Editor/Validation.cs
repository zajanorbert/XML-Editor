using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using XML_Editor;

namespace XML_Editor
{

    
    class Validation : Form
    {


        

        public Validation()
        {
             
        }

        public static TabControl ValidationReport(TabControl tabControl1, string fileName, ListBox listBox, int _issueCounter, List<string> _validationComments, TabPage currentTab, List<int> lineNumbs)
        {
            
            listBox.Items.Clear();
            listBox.Controls.Clear();
            
            listBox.Items.Add("=======================================|");
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
                float i = listBox.Items.Count * listBox.ItemHeight;
                foreach (var comment in _validationComments)
                {
                    LinkLabel linkLabel1 = new LinkLabel();

                    linkLabel1.AutoSize = true;
                    linkLabel1.Cursor = Cursors.Hand;
                    LinkLabel.Link data = new LinkLabel.Link();
                    foreach (var line in lineNumbs)
                    {
                        if(comment.Contains(line.ToString()))
                        {
                            data.LinkData = @currentTab.Tag + ";" + line;
                        }
                    }
                    
                    linkLabel1.Links.Add(data);
                    linkLabel1.LinkClicked += XMLEditor9000.linkLabel1_LinkClicked;
                    linkLabel1.Text = comment;
                    listBox.Controls.Add(linkLabel1);
                    listBox.Items.Add(linkLabel1.Text);
                    linkLabel1.Location = new Point(0, int.Parse(i.ToString()));
                    i += listBox.ItemHeight;
                    

                }
            }
            
            listBox.Items.Add("|=======================================|");
            return tabControl1;
        }
    }


}
