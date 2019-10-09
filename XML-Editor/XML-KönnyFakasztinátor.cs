using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace XML_Editor
{
    public partial class XMLKönnyFakasztinátor : Form
    {

        private RichTextBox focusedRichTextBox;

        public XMLKönnyFakasztinátor()
        {
            InitializeComponent();
            foreach (RichTextBox rtb in this.Controls.OfType<RichTextBox>())
            {
                rtb.Enter += richTextBox_Enter;
            }

        }

        void richTextBox_Enter(object sender, EventArgs e)
        {
            focusedRichTextBox = (RichTextBox)sender;
        }

        private void XMLKönnyfakasztinátor_Load(object sender, EventArgs e)
        {

        }

        private void openXMLFileToolStripMenuItem_Click_1(object sender, MouseEventArgs me)
        {
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    //string title = "TabPage " + (tabControl1.TabCount + 1).ToString();
                    string strfilename = openFileDialog1.FileName;
                    string filetext = File.ReadAllText(strfilename);
                    RichTextBox box = new RichTextBox();
                    box.Dock = DockStyle.Fill;
                    TabPage tab = addTab(sender, strfilename, me);
                    box.Text = filetext;
                    tab.Controls.Add(box);
                    focusedRichTextBox = box;
                    
                }
            }
        }

        private void closeTab(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Equals(newTab))
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private TabPage addTab(object sender, string strfilename, MouseEventArgs me)
        {
            var lastIndex = this.tabControl1.TabCount - 1;
            /*if (this.tabControl1.GetTabRect(lastIndex).Contains(me.Location))
            {*/
            string dbf_File = System.IO.Path.GetFileName(strfilename);

            string title = dbf_File;
            TabPage myTabPage = new TabPage(title);
            tabControl1.TabPages.Insert(lastIndex, myTabPage);
            this.tabControl1.SelectedIndex = lastIndex;
            return myTabPage;
            /*}
            return null;*/
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {

            var lastIndex = this.tabControl1.TabCount - 1;
            if (this.tabControl1.GetTabRect(lastIndex).Contains(e.Location))
            {
                TabPage myTabPage = new TabPage("New Tab");
                RichTextBox box = new RichTextBox();
                box.Dock = DockStyle.Fill;
                myTabPage.Controls.Add(box);
                tabControl1.TabPages.Insert(lastIndex, myTabPage);
                this.tabControl1.SelectedIndex = lastIndex;
            }

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.HasChildren)
            {
                focusedRichTextBox = e.TabPage.Controls.OfType<RichTextBox>().First();
                //MessageBox.Show(focusedRichTextBox.Text);
            }
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.CheckFileExists)
                {
                    using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        sw.Write(focusedRichTextBox.Text);

                    }
                }
                else
                {
                    using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        sw.Write(focusedRichTextBox.Text);

                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            listBox1.Items.Add("Starter project for an XML-Editor applicaion ");
            listBox1.Items.Add("Programmed by Barna Dénes & Zaja Norbert");
        }
    }
}
