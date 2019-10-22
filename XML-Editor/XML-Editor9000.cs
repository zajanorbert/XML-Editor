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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XML_Editor
{
    public partial class XMLEditor9000 : Form
    {
        private int _issueCounter;
        private List<string> _validationComments;
        private RichTextBox focusedRichTextBox;
        private TabPage currentTab;
        private bool listShow = false;
        private string keyword = "<";
        private int count = 0;



        public XMLEditor9000()
        {
            InitializeComponent();
            foreach (RichTextBox rtb in this.Controls.OfType<RichTextBox>())
            {
                rtb.Enter += richTextBox_Enter;
            }

            button1.Text = "\uD83D\uDDD1";

            #region link
            richTextBox1.Text = "Valami szar ";
            LinkLabel link = new LinkLabel();
            link.Text = "itt, ide kattincs";
            link.LinkClicked += DoShit;
            LinkLabel.Link data = new LinkLabel.Link();
            data.LinkData = @"C:\";
            link.Links.Add(data);
            link.AutoSize = true;
            link.Location =
                this.richTextBox1.GetPositionFromCharIndex(this.richTextBox1.TextLength);
            this.richTextBox1.Controls.Add(link);
            this.richTextBox1.AppendText(link.Text + " na most jo te szajha?");
            #endregion link

            focusedRichTextBox = richTextBox1;
            lineNumbering();
        }

        private void DoShit(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Console.WriteLine("BUMMM {0}", e.Link.LinkData);
        }

        private void XMLEditor9000_Load(object sender, EventArgs e)
        {
            focusedRichTextBox = richTextBox1;
        }

        #region tabcontrol
        private void closeTab(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Equals(newTab))
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private TabPage addTab(string strfilename)
        {
            var lastIndex = this.tabControl1.TabCount - 1;
            /*if (this.tabControl1.GetTabRect(lastIndex).Contains(me.Location))
            {*/
            string title = System.IO.Path.GetFileName(strfilename);
            TabPage myTabPage = new TabPage(title);
            myTabPage.Tag = strfilename;
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
                makeNewTab(lastIndex);
            }

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            listBox2.Items.Clear();
            currentTab = e.TabPage;
            if (e.TabPage.HasChildren)
            {

                currentTab.Controls.Add(richTextBox2);
                focusedRichTextBox = e.TabPage.Controls.OfType<RichTextBox>().First();
                if(focusedRichTextBox.Text != "")
                {
                    elementGetter();
                }
                
                HighLight.hLRTF(focusedRichTextBox);
            }

            richTextBox2.Text = "";
            lineNumbering();

        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            lineNumbering();
            if (e.KeyCode == Keys.Enter) //[COLOR = red]/*Section 1*/[/ COLOR]
            {
                count = 0;
                keyword = "<";
                listShow = false;
                listBox2.Hide();

            }
            if (e.Control == false && e.KeyCode == Keys.Space)
            {
                count = 0;
                keyword = "<";
                listShow = false;
                listBox2.Hide();
            }

            if (listShow == true) //[COLOR =#ff0000]/*Section 2*/[/COLOR]
            {

                if (e.KeyCode == Keys.Up)
                {
                    listBox2.Focus();
                    if (listBox2.SelectedIndex > 0)
                    {
                        listBox2.SelectedIndex -= 1;
                    }
                    else
                    {
                        listBox2.SelectedIndex = 0;
                    }
                    focusedRichTextBox.Focus();

                }
                else if (e.KeyCode == Keys.Down)
                {

                    listBox2.Focus();
                    try
                    {
                        listBox2.SelectedIndex += 1;
                    }
                    catch
                    {
                    }
                    focusedRichTextBox.Focus();
                }
                else if ((e.KeyCode == Keys.Enter))
                {

                    listBox2.Focus();
                    string autoText = string.Format("<{0}></{0}>", listBox2.SelectedItem.ToString());
                    int beginPlace = focusedRichTextBox.SelectionStart - count;
                    focusedRichTextBox.Select(beginPlace, count);
                    focusedRichTextBox.SelectedText = "";
                    focusedRichTextBox.Text += autoText;
                    focusedRichTextBox.Focus();
                    listShow = false;
                    listBox2.Hide();
                    int endPlace = autoText.Length + beginPlace;
                    focusedRichTextBox.SelectionStart = endPlace;
                    count = 0;
                    focusedRichTextBox.Focus();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    listBox2.Hide();
                    listShow = false;
                    focusedRichTextBox.Focus();
                }
            }
        }

        #endregion tabcontrol

        #region Menu
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML files (*.xml)|*.xml";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (saveFileDialog1.CheckFileExists)
                    {
                        using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                        using (StreamWriter sw = new StreamWriter(s))
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(focusedRichTextBox.Text);
                            doc.Save(saveFileDialog1.FileName);
                            //sw.Write(focusedRichTextBox.Text);
                            string filename = System.IO.Path.GetFileName(saveFileDialog1.FileName);
                            listBox1.Items.Add(filename + " saved");
                            currentTab.Tag = saveFileDialog1.FileName;
                            currentTab.Text = filename;
                        }


                    }
                    else
                    {

                        //File.WriteAllText(saveFileDialog1.FileName, focusedRichTextBox.Text);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(focusedRichTextBox.Text);
                        doc.Save(saveFileDialog1.FileName);
                        string filename = System.IO.Path.GetFileName(saveFileDialog1.FileName);
                        listBox1.Items.Add(filename + " saved");
                        currentTab.Tag = saveFileDialog1.FileName;
                        currentTab.Text = filename;
                    }
                }
                catch (XmlException xe)
                {
                    listBox1.Items.Add("Invalid XML formation, can't save");
                }
            }
        }

        private void openXMLFileToolStripMenuItem_Click_1(object sender, MouseEventArgs me)
        {
            Stream xmlStream;
            OpenFileDialog xmlFileDialog = new OpenFileDialog();
            xmlFileDialog.Filter = "XML files (*.xml)|*.xml";
            bool isValid = false;
            string xmlPath = "";
            try
            {
                if (xmlFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (xmlStream = xmlFileDialog.OpenFile())
                    using (StreamReader sr = new StreamReader(xmlStream))
                    {
                        if (xmlStream != null)
                        {
                            xmlPath = xmlFileDialog.FileName;
                            RichTextBox box = new RichTextBox();
                            box.Dock = DockStyle.Fill;
                            box.Multiline = true;
                            box.AcceptsTab = true;
                            box.Text = sr.ReadToEnd();
                            Console.WriteLine(box.Text);
                            XDocument doc = XDocument.Parse(box.Text);
                            TabPage tab = addTab(xmlPath);
                            foreach (var name in doc.Root.DescendantNodes().OfType<XElement>().Select(x => x.Name).Distinct())
                            {
                                listBox2.Items.Add(name);
                            }
                            tab.Controls.Add(richTextBox2);

                            //...........................................................................

                            tab.Controls.Add(box);
                            tab.Controls.SetChildIndex(box, 0);
                            focusedRichTextBox = box;
                            HighLight.hLRTF(focusedRichTextBox);
                            listBox1.Items.Add("XML file loaded");
                            lineNumbering();
                        }
                    }
                }
            }catch(XmlException xe)
            {
                listBox1.Items.Add("Invalid XML formation, can't load");
            }


            //return isValid;
        }

        private void saveOverride(object sender, EventArgs e)
        {
            try
            {
                if (currentTab.Tag != null)
                {

                    string path = currentTab.Tag.ToString();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(focusedRichTextBox.Text);
                    doc.Save(path);
                    listBox1.Items.Add("File saved");
                }
                else
                {
                    saveFileToolStripMenuItem_Click(sender, e);
                }
            }
            catch (XmlException xe)
            {
                listBox1.Items.Add("Invalid XML formation, can't save");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.Show();
        }

        private void XMLEditor9000_KeyDown(object sender, KeyEventArgs e)
        {
            if (listShow == true) //[COLOR =#000080]/*Section 1*/[/COLOR]
            {

                keyword += '<';
                count++;
                Point point = this.focusedRichTextBox.GetPositionFromCharIndex(focusedRichTextBox.SelectionStart);
                point.Y += (int)Math.Ceiling(this.focusedRichTextBox.Font.GetHeight()) + 13; //13 is the .y postion of the richtectbox
                point.X += 105; //105 is the .x postion of the richtectbox
                listBox2.Location = point;
                listBox2.Show();
                listBox2.SelectedIndex = 0;
                listBox2.SelectedIndex = listBox2.FindString(keyword);
                focusedRichTextBox.Focus();


            }

            if ((e.Control == true && e.KeyCode == Keys.Space)) //[COLOR =#000080]/*Section 2*/[/COLOR]
            {
                try
                {
                    listShow = true;
                    Point point = this.focusedRichTextBox.GetPositionFromCharIndex(focusedRichTextBox.SelectionStart);
                    point.Y += (int)Math.Ceiling(this.focusedRichTextBox.Font.GetHeight()) + 13; //13 is the .y postion of the richtectbox
                    point.X += 105; //105 is the .x postion of the richtectbox
                    listBox2.Location = point;
                    count++;
                    listBox2.Show();
                    listBox2.SelectedIndex = 0;
                    listBox2.SelectedIndex = listBox2.FindString(keyword);
                    focusedRichTextBox.Focus();

                }
                catch (Exception ex)
                {
                    listBox1.Items.Add("No love in your life. Go home and die! <3");
                    listBox2.Hide();
                    listShow = false;
                }

            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FontSizeOption_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();

            if (fd.ShowDialog() == DialogResult.OK)
            {
                listBox1.Font = fd.Font;
                focusedRichTextBox.Font = fd.Font;
            }
        }

        private void newXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lastIndex = this.tabControl1.TabCount - 1;
            makeNewTab(lastIndex);
        }

        private void validationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string xmlPath = currentTab.Tag.ToString();
                Stream xsdStream;
                string xsdPath = "";
                _issueCounter = 0;
                OpenFileDialog xsdFileDialog = new OpenFileDialog();
                xsdFileDialog.Filter = "XSD files (*.xsd)|*.xsd";
                _validationComments = new List<string>();
                try
                {
                    XmlReaderSettings rearderSettings = new XmlReaderSettings();
                    rearderSettings.ValidationType = ValidationType.Schema;
                    rearderSettings.ValidationFlags |=
                      XmlSchemaValidationFlags.ProcessSchemaLocation |
                      XmlSchemaValidationFlags.ReportValidationWarnings |
                      XmlSchemaValidationFlags.ProcessIdentityConstraints |
                      XmlSchemaValidationFlags.AllowXmlAttributes;

                    if (xsdFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if ((xsdStream = xsdFileDialog.OpenFile()) != null)
                        {
                            xsdPath = xsdFileDialog.FileName;
                            string xsdText = File.ReadAllText(xsdPath);
                            listBox1.Items.Add("XSD file loaded");
                        }
                    }
                    rearderSettings.Schemas.Add(null, XmlReader.Create(xsdPath));
                    rearderSettings.ValidationEventHandler += new ValidationEventHandler(XmlValidationEventHandler);
                    using (XmlReader xmlValidatingReader = XmlReader.Create(xmlPath, rearderSettings))
                    {
                        while (xmlValidatingReader.Read())
                        {
                            //box.Text = xmlValidatingReader.ReadOuterXml();
                        }

                    }

                }
                catch (Exception error)
                {
                    //isValid = false;
                    listBox1.Items.Add("Exception " + error.Message);
                }

                Validation.ValidationReport(System.IO.Path.GetFileName(xmlPath), listBox1, _issueCounter, _validationComments);
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("You tried...");
                listBox1.Items.Add("Good luck next time...");
            }
        }
        #endregion Menu

        #region XMLStuffs
        private string xmlIndenter(int whiteSpace, string xml)
        {
            int counter = 2;
            string whiteSp = "  ";

            while (counter != whiteSpace)
            {
                counter++;
                whiteSp += " ";
            }

            var stringBuilder = new StringBuilder();

            try
            {
                var element = XElement.Parse(xml);
                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = false;
                settings.Indent = true;
                settings.IndentChars = whiteSp;
                settings.NewLineOnAttributes = true;


                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    element.Save(xmlWriter);
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }


            return stringBuilder.ToString();
        }

        private void IndentSize_Click(object sender, EventArgs e)
        {
            var itemText = (sender as ToolStripMenuItem).Text;
            var intText = int.Parse(itemText);
            var xml = focusedRichTextBox.Text;
            focusedRichTextBox.Text = xmlIndenter(intText, xml);
            HighLight.hLRTF(focusedRichTextBox);
        }

        private void XmlValidationEventHandler(object sender, ValidationEventArgs e)
        {
            _issueCounter++;
            _validationComments.Add(string.Format("{0} @ line {1} position {2}: {3} \r\n",
              (e.Severity == XmlSeverityType.Error ? "ERROR" : "WARNING"),
              e.Exception.LineNumber,
              e.Exception.LinePosition,
              e.Message));
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            listBox2.Focus();
            string autoText = string.Format("<{0}></{0}>", listBox2.SelectedItem.ToString());
            int beginPlace = focusedRichTextBox.SelectionStart - count;
            try
            {
                focusedRichTextBox.Select(beginPlace, count);
                focusedRichTextBox.SelectedText = "";
                focusedRichTextBox.Text = focusedRichTextBox.Text.Insert(focusedRichTextBox.SelectionStart, autoText); ;
                focusedRichTextBox.Focus();
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("You forgot modify your other xml. Now you can modify this");
            }
            listShow = false;
            listBox2.Hide();
            int endPlace = autoText.Length + beginPlace;
            focusedRichTextBox.SelectionStart = endPlace;
            count = 0;
            HighLight.hLRTF(focusedRichTextBox);
            focusedRichTextBox.Focus();
        }

        #endregion XMLStuffs

        #region privateMethods

        private void lineNumbering()
        {
            for (int i = 0; i <= focusedRichTextBox.Lines.Count(); i++)
            {
                if (focusedRichTextBox.Text != "")
                {
                    if (!richTextBox2.Text.Contains(i.ToString()))
                    {
                        richTextBox2.Text += i.ToString() + "\n";
                    }
                }
                else
                {
                    richTextBox2.Clear();
                }
            }
        }

        private void makeNewTab(int lastIndex)
        {

            TabPage myTabPage = new TabPage("New Tab");
            RichTextBox box = new RichTextBox();
            box.Dock = DockStyle.Fill;
            box.Multiline = true;
            box.AcceptsTab = true;
            myTabPage.Controls.Add(box);
            myTabPage.Controls.Add(richTextBox2);
            myTabPage.Controls.SetChildIndex(box, 0);
            tabControl1.TabPages.Insert(lastIndex, myTabPage);
            this.tabControl1.SelectedIndex = lastIndex;
            focusedRichTextBox = box;
        }

        private void richTextBox_Enter(object sender, EventArgs e)
        {
            focusedRichTextBox = (RichTextBox)sender;
        }

        private void elementGetter()
        {
            
            XDocument doc = XDocument.Parse(focusedRichTextBox.Text);
            foreach (var name in doc.Root.DescendantNodes().OfType<XElement>().Select(x => x.Name).Distinct())
            {
                listBox2.Items.Add(name);
            }
        }
        #endregion privateMethods

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }
}
