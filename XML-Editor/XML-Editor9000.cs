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

        public XMLEditor9000()
        {
            InitializeComponent();
            foreach (RichTextBox rtb in this.Controls.OfType<RichTextBox>())
            {
                rtb.Enter += richTextBox_Enter;
            }

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
        }

        private void DoShit(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Console.WriteLine("BUMMM {0}", e.Link.LinkData);
        }

        void richTextBox_Enter(object sender, EventArgs e)
        {
            focusedRichTextBox = (RichTextBox)sender;
        }

        private void XMLEditor9000_Load(object sender, EventArgs e)
        {
            focusedRichTextBox = richTextBox1;
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

        #region tabcontrol
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
                TabPage myTabPage = new TabPage("New Tab");
                RichTextBox box = new RichTextBox();
                box.Dock = DockStyle.Fill;
                myTabPage.Controls.Add(box);
                tabControl1.TabPages.Insert(lastIndex, myTabPage);
                this.tabControl1.SelectedIndex = lastIndex;
            }

            if (e.Button == MouseButtons.Middle)
            {
                closeTab(sender, e);
            }

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            currentTab = e.TabPage;
            if (e.TabPage.HasChildren)
            {
                focusedRichTextBox = e.TabPage.Controls.OfType<RichTextBox>().First();
                HighLight.hLRTF(focusedRichTextBox);
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

                    File.WriteAllText(saveFileDialog1.FileName, focusedRichTextBox.Text);
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

            #region modified_file_open
            /*if (xmlFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((xmlStream = xmlFileDialog.OpenFile()) != null)
                {
                    //string title = "TabPage " + (tabControl1.TabCount + 1).ToString();

                    RichTextBox box = new RichTextBox();
                    box.Dock = DockStyle.Fill;
                    xmlPath = xmlFileDialog.FileName;
                    TabPage tab = addTab(sender, xmlPath, me);

                    using (XmlReader xmlValidatingReader = XmlReader.Create(xmlPath, rearderSettings))
                    {
                        try
                        {

                            string xsdName;

                            bool found = false;
                            while (xmlValidatingReader.Read())
                            {
                                xsdName = xmlValidatingReader.GetAttribute("xsi:noNamespaceSchemaLocation");
                                if (xsdName != null)
                                {
                                    listBox1.Items.Add(xsdName);
                                    xsdPath = xmlPath.Replace("xml", "xsd");
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                throw new Exception();

                            }
                            else
                            {
                                rearderSettings.ValidationEventHandler += new ValidationEventHandler(XmlValidationEventHandler);
                                rearderSettings.Schemas.Add(null, XmlReader.Create(xsdPath));

                                box.Text = File.ReadAllText(xmlPath);
                                tab.Controls.Add(box);
                                focusedRichTextBox = box;
                                HighLight.hLRTF(focusedRichTextBox);
                                listBox1.Items.Add("XML file loaded");
                            }


                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("invalid xsd path or not exist, validation failed");

                            if (xsdFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                if ((xsdStream = xsdFileDialog.OpenFile()) != null)
                                {
                                    xsdPath = xsdFileDialog.FileName;
                                    //string xsdText = File.ReadAllText(xsdPath);
                                    listBox1.Items.Add("XSD file loaded");
                                }
                            }
                            xmlValidatingReader.Close();
                            using (XmlReader xmlValidating = XmlReader.Create(xmlPath, rearderSettings))
                            {

                                rearderSettings.ValidationEventHandler += new ValidationEventHandler(XmlValidationEventHandler);
                                rearderSettings.Schemas.Add(null, XmlReader.Create(xsdPath));
                            }

                            box.Text = File.ReadAllText(xmlPath);
                            tab.Controls.Add(box);
                            focusedRichTextBox = box;
                            HighLight.hLRTF(focusedRichTextBox);
                            listBox1.Items.Add("XML file loaded");
                        }

                    }
                }
            }*/
            #endregion modified_file_open

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
                        TabPage tab = addTab(sender, xmlPath, me);

                        box.Text = sr.ReadToEnd();

                        //...........................................................................

                        tab.Controls.Add(box);
                        focusedRichTextBox = box;
                        HighLight.hLRTF(focusedRichTextBox);
                        listBox1.Items.Add("XML file loaded");
                    }
                }
            }


            //return isValid;
        }

        private void saveOverride(object sender, EventArgs e)
        {
            try
            {
                if (currentTab != null)
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

        /*private void XMLEditor9000_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.S)
            {
                saveFileToolStripMenuItem.PerformClick();
            }
        }*/

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
            TabPage myTabPage = new TabPage("New Tab");
            RichTextBox box = new RichTextBox();
            box.Dock = DockStyle.Fill;
            myTabPage.Controls.Add(box);
            tabControl1.TabPages.Insert(lastIndex, myTabPage);
            this.tabControl1.SelectedIndex = lastIndex;
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

        #region Indenter
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

        #endregion Indenter

        
    }
        
}
