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
using XML_Editor.Properties;
using System.Threading;
using System.Diagnostics;

namespace XML_Editor
{
    public partial class XMLEditor9000 : Form
    {
        private int _issueCounter;
        private List<string> _validationComments;
        private static RichTextBoxSynchronizedScroll focusedRichTextBox;
        private static TabPage currentTab;
        private bool listShow = false;
        private string keyword = "<";
        private int count = 0;
        private string currentxsd = null;
        private bool isvalidated = false;
        private List<int> lineNumbs;
        private static TabControl staticTabcontrol;
        private WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
        private List<string> crayonsSet;


        #region XMLEditor9000
        public XMLEditor9000()
        {
            InitializeComponent();
            foreach (RichTextBox rtb in this.Controls.OfType<RichTextBox>())
            {
                rtb.Enter += richTextBox_Enter;
            }

            button1.Text = "\uD83D\uDDD1";//kuka
            newTab.Text = "\uD83D\uDC1C";//ant
            wplayer.URL = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\")) + @"\Properties\1-02 Do You Want to Build a Snowman.mp3";



            //xmlHandler.createConfig();
        }

        private void XMLEditor9000_Load(object sender, EventArgs e)
        {
            
            loadPrevious();
        }

        private void XMLEditor9000_KeyDown(object sender, KeyEventArgs e)
        {

            if (listShow == true) //[COLOR =#000080]/*Section 1*/[/COLOR]
            {

                keyword += '<';
                count++;
                Point point = focusedRichTextBox.GetPositionFromCharIndex(focusedRichTextBox.SelectionStart);
                point.Y += (int)Math.Ceiling(focusedRichTextBox.Font.GetHeight()) + 13; //13 is the .y postion of the richtectbox
                point.X += 105; //105 is the .x postion of the richtectbox
                listBox2.Location = point;
                listBox2.Show();
                listBox2.SelectedIndex = 0;
                listBox2.SelectedIndex = listBox2.FindString(keyword);
                focusedRichTextBox.Focus();


            }

            if ((e.Control == true && e.KeyCode == Keys.Space)) //|| (e.Control == true && e.Alt == true && e.KeyCode == Keys.)) //[COLOR =#000080]/*Section 2*/[/COLOR]
            {
                try
                {

                    listShow = true;
                    Point point = focusedRichTextBox.GetPositionFromCharIndex(focusedRichTextBox.SelectionStart);
                    point.Y += (int)Math.Ceiling(focusedRichTextBox.Font.GetHeight()) + 13; //13 is the .y postion of the richtectbox
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
                    //listBox1.Items.Add("No love in your life. Go home and die! <3");
                    Console.WriteLine(ex.Message);
                    //listBox1.Items.Add("Document doesn't contain node elements");

                    listBox2.Hide();
                    listShow = false;
                }

            }

        }

        private void XMLEditor9000_FormClosing(object sender, FormClosingEventArgs e)
        {

            DialogResult dialog = MessageBox.Show("Do you really want to close?", "Exit", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                //Application.Exit();
                xmlHandler.updateXml(tabControl1, crayonsSet);

                //Thread.CurrentThread.Abort();
                //Application.Exit();
                var prc = Process.GetProcessesByName("XML-Editor");
                if (prc.Length > 0) prc[prc.Length - 1].Kill();
                Environment.Exit(0);
                
                return;
            }
            else if (dialog == DialogResult.No)
            {
                e.Cancel = true;
            }

        }

        

        #endregion XMLEditor9000

        #region tabcontrol
        private void closeTab(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Equals(newTab))
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);


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
            Font fontSize = listBox1.Font;
            if (e.TabPage.HasChildren)
            {
                richTextBox2.Text = "";
                currentTab.Controls.Add(richTextBox2);
                focusedRichTextBox = e.TabPage.Controls.OfType<RichTextBoxSynchronizedScroll>().First();
                try
                {
                    if (focusedRichTextBox.Text != "" && focusedRichTextBox.Lines.Length > 1)
                    {
                        elementGetter();
                    }
                }
                catch (XmlException xe)
                {
                    listBox1.Items.Add(xe.Message);
                }


                focusedRichTextBox.Font = fontSize;
                HighLight.hLRTF(focusedRichTextBox);
                focusedRichTextBox.vScroll += scrollSyncTxtBox1_vScroll;
                focusedRichTextBox.MouseWheel += FocusedRichTextBox_MouseWheel;
                currentxsd = null;
                isvalidated = false;
                lineNumbering();
            }



        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.O)
            {
                openXMLFileToolStripMenuItem.PerformClick();
            }

            if ((!tabControl1.SelectedTab.Equals(newTab)) && (e.Control == true && e.KeyCode == Keys.W))
            {
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            }

            if ((!currentTab.Text.Contains("*")) && (!tabControl1.SelectedTab.Equals(newTab)))
            {
                var oldText = currentTab.Text;
                currentTab.Text = oldText + "*";
            }

            richTextBox2.Clear();
            lineNumbering();


            if ((listShow == true) && (e.KeyCode == Keys.Enter)) //[COLOR = red]/*Section 1*/[/ COLOR]
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
                    Console.WriteLine(xe.Message);
                    listBox1.Items.Add("Invalid XML formation, can't save");
                }
            }
        }

        private void openXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream xmlStream = null;
            OpenFileDialog xmlFileDialog = new OpenFileDialog();
            xmlFileDialog.Filter = "XML files (*.xml)|*.xml";
            //bool isValid = false;
            string xmlPath = "";
            richTextBox2.Text = "";

            try
            {
                if (xmlFileDialog.ShowDialog() == DialogResult.OK)
                {

                    xmlPath = xmlFileDialog.FileName;
                    //using (xmlStream = xmlFileDialog.OpenFile())
                    openXMLFile(xmlPath, xmlStream);
                }
            }
            catch (XmlException xe)
            {
                Console.WriteLine(xe.Message);
                listBox1.Items.Add("Invalid XML formation, can't load");
            }


            //return isValid;
        }

        private void saveOverride(object sender, EventArgs e)
        {
            string lpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\config.xml";
            try
            {
                if (currentTab.Tag != null)
                {

                    string path = currentTab.Tag.ToString();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(focusedRichTextBox.Text);
                    doc.Save(path);
                    
                    listBox1.Items.Add("File saved");
                    currentTab.Text = Path.GetFileName(path);
                }
                else
                {
                    saveFileToolStripMenuItem_Click(sender, e);
                }
                if (currentTab.Tag != null && currentTab.Tag.Equals(lpath))
                {
                    crayonsSet = new List<string>();
                    using (XmlReader xmlReader = XmlReader.Create(lpath))
                    {
                        while (xmlReader.Read())
                        {
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "nodeColor"))
                            {
                                crayonsSet.Add(xmlReader.ReadInnerXml());
                            }
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "stringColor"))
                            {
                                crayonsSet.Add(xmlReader.ReadInnerXml());
                            }
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "attributeColor"))
                            {
                                crayonsSet.Add(xmlReader.ReadInnerXml());
                            }
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "textColor"))
                            {
                                crayonsSet.Add(xmlReader.ReadInnerXml());
                            }
                        }
                    }
                    
                    HighLight.config(crayonsSet);
                    listBox1.Font = xmlHandler.loadFontConfig();
                    tabControl1.TabPages.Remove(currentTab);
                    tabControl1.TabPages[0].Select();
                    currentTab = tabControl1.TabPages[0];
                }
            }
            catch (XmlException xe)
            {
                Console.WriteLine(xe.Message);
                listBox1.Items.Add("Invalid XML formation, can't save");
                listBox1.Items.Add(xe.Message);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you really want to close?", "Exit", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (dialog == DialogResult.No)
            {
                e.Equals(false);
            }
        }

        private void FontSizeOption_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();

            if (fd.ShowDialog() == DialogResult.OK)
            {
                listBox1.Font = fd.Font;
                focusedRichTextBox.Font = fd.Font;
                HighLight.hLRTF(focusedRichTextBox);
                if (isvalidated)
                {
                    validationToolStripMenuItem.PerformClick();
                }

            }
        }

        private void newXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lastIndex = this.tabControl1.TabCount - 1;
            makeNewTab(lastIndex);
        }

        private void validationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            focusedRichTextBox.SelectAll();
            focusedRichTextBox.SelectionBackColor = Color.White;
            focusedRichTextBox.DeselectAll();
            try
            {
                string xmlPath = currentTab.Tag.ToString();
                Stream xsdStream;
                string xsdPath = "";


                _issueCounter = 0;
                OpenFileDialog xsdFileDialog = new OpenFileDialog();
                xsdFileDialog.Filter = "XSD files (*.xsd)|*.xsd";
                _validationComments = new List<string>();
                lineNumbs = new List<int>();
                try
                {
                    XmlReaderSettings rearderSettings = new XmlReaderSettings();
                    rearderSettings.ValidationType = ValidationType.Schema;
                    rearderSettings.ValidationFlags |=
                      XmlSchemaValidationFlags.ProcessSchemaLocation |
                      XmlSchemaValidationFlags.ReportValidationWarnings |
                      XmlSchemaValidationFlags.ProcessIdentityConstraints |
                      XmlSchemaValidationFlags.AllowXmlAttributes;
                    if (currentxsd != null)
                    {

                        xsdPath = currentxsd;
                    }
                    else
                    {
                        if (xsdFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            if ((xsdStream = xsdFileDialog.OpenFile()) != null)
                            {
                                xsdPath = xsdFileDialog.FileName;
                                currentxsd = xsdPath;
                                string xsdText = File.ReadAllText(xsdPath);
                                listBox1.Items.Add("XSD file loaded");
                                isvalidated = true;
                            }
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

                staticTabcontrol = Validation.ValidationReport(tabControl1, Path.GetFileName(xmlPath), listBox1, _issueCounter, _validationComments, currentTab, lineNumbs);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                /*listBox1.Items.Add("You tried...");
                listBox1.Items.Add("Good luck next time...");*/
                listBox1.Items.Add("Save the file before validation");
            }

        }

        private void openConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openXMLFile(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\config.xml", null);
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
            lineNumbs.Add(e.Exception.LineNumber);
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
                Console.WriteLine(ex.Message);
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

        public static void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Console.WriteLine("{0}", e.Link.LinkData);
            string[] path = e.Link.LinkData.ToString().Split(';');

            foreach (TabPage tab in staticTabcontrol.TabPages)
            {


                if (tab.Text.Equals(Path.GetFileName(path[0])))
                {
                    currentTab = tab;
                    staticTabcontrol.SelectedTab = tab;
                    focusedRichTextBox = tab.Controls.OfType<RichTextBoxSynchronizedScroll>().First();
                    if (int.Parse(path[1]) > focusedRichTextBox.Lines.Count()) return;

                    focusedRichTextBox.SelectionStart = focusedRichTextBox.Find(focusedRichTextBox.Lines[int.Parse(path[1])]);
                    focusedRichTextBox.ScrollToCaret();
                    int firstcharindex = focusedRichTextBox.GetFirstCharIndexOfCurrentLine();
                    string currenttext = focusedRichTextBox.Lines[int.Parse(path[1])];
                    focusedRichTextBox.Select(firstcharindex, currenttext.Length);
                    focusedRichTextBox.SelectionBackColor = Color.YellowGreen;

                }

            }

        }

        #endregion XMLStuffs

        #region privateMethods

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

        private void lineNumbering()
        {
            for (int i = 1; i <= focusedRichTextBox.Lines.Count(); i++)
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
            string xnode = "<?xml version=\"1.0\" encoding=\"UTF - 16\"?>";
            TabPage myTabPage = new TabPage("Untitled");
            RichTextBoxSynchronizedScroll box = new RichTextBoxSynchronizedScroll();
            box.Dock = DockStyle.Fill;
            box.Multiline = true;
            box.AcceptsTab = true;
            myTabPage.Controls.Add(box);
            myTabPage.Controls.Add(richTextBox2);
            myTabPage.Controls.SetChildIndex(box, 0);
            box.Text = xnode;
            tabControl1.TabPages.Insert(lastIndex, myTabPage);
            this.tabControl1.SelectedIndex = lastIndex;
            focusedRichTextBox = box;
            focusedRichTextBox.vScroll += scrollSyncTxtBox1_vScroll;
            focusedRichTextBox.MouseWheel += FocusedRichTextBox_MouseWheel;
            listBox2.Items.Add("szavak");
            listBox2.Items.Add("asd");
            listBox2.Items.Add("random");
            listBox2.Items.Add("lol");
            listBox2.Items.Add("node");
        }

        private void richTextBox_Enter(object sender, EventArgs e)
        {
            focusedRichTextBox = (RichTextBoxSynchronizedScroll)sender;
        }

        private void elementGetter()
        {
            XDocument doc = XDocument.Parse(focusedRichTextBox.Text);
            foreach (var name in doc.Root.DescendantNodes().OfType<XElement>().Select(x => x.Name).Distinct())
            {

                listBox2.Items.Add(name.LocalName);
            }
        }

        private void FocusedRichTextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            //((HandledMouseEventArgs)e).Handled = true;//disable default mouse wheel


            Message msg = new Message
            {
                Msg = 0x115

            };

            if (e.Delta > 1)
            {
                msg.WParam = new IntPtr(0x0);
            }
            else
            {
                msg.WParam = new IntPtr(0x1);

            }
            scrollSyncTxtBox1_vScroll(msg);
        }

        private void openXMLFile(string xmlPath, Stream xmlStream)
        {
            Font font = listBox1.Font;
            using (StreamReader sr = new StreamReader(xmlPath))
            {
                /*if (xmlStream != null)
                {*/
                RichTextBoxSynchronizedScroll box = new RichTextBoxSynchronizedScroll();
                box.Dock = DockStyle.Fill;
                box.Multiline = true;
                box.AcceptsTab = true;
                box.Text = sr.ReadToEnd();
                //box.BackColor = Color.Black;

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
                focusedRichTextBox.Font = font;

                focusedRichTextBox.vScroll += scrollSyncTxtBox1_vScroll;
                focusedRichTextBox.MouseWheel += FocusedRichTextBox_MouseWheel;

                HighLight.hLRTF(focusedRichTextBox);
                listBox1.Items.Add("XML file loaded");
                lineNumbering();
                //}
            }
        }

        private void loadPrevious()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            List<string> xmlPaths;
            try
            {
                if (File.Exists(path + "\\config.xml"))
                {
                    //do you wanna build a snowman?
                    crayonsSet = xmlHandler.loadColorConfig();
                    HighLight.config(crayonsSet);
                    listBox1.Font = xmlHandler.loadFontConfig();
                    wplayer.controls.play();

                    DialogResult dialog = MessageBox.Show("Do you wanna build a snowman?", "Last session", MessageBoxButtons.YesNo);
                    if (dialog == DialogResult.Yes)
                    {

                        xmlPaths = xmlHandler.XmlLoad();
                        if (xmlPaths.Count > 0)
                        {
                            foreach (string xmlPath in xmlPaths)
                            {
                                openXMLFile(xmlPath, null);

                            }
                            tabControl1.TabPages.Remove(tabPage1);
                        }

                        focusedRichTextBox.Font = listBox1.Font;
                        Snowman snowman = new Snowman();
                        snowman.Show();
                        snowman.TopMost = true;
                        snowman.FormClosed += Snowman_FormClosed;
                        
                    }
                    
                }
                else
                {

                    xmlHandler.createConfig();
                    crayonsSet = HighLight.defaultColors();
                    currentTab = tabPage1;
                    focusedRichTextBox = richTextBox1;
                    focusedRichTextBox.MouseWheel += FocusedRichTextBox_MouseWheel;
                    focusedRichTextBox.vScroll += scrollSyncTxtBox1_vScroll;
                    lineNumbering();
                }
            }
            catch(ArgumentNullException ae)
            {
                listBox1.Items.Add("The config was empty, we made you another one.");
                xmlHandler.createConfig();
                crayonsSet = HighLight.defaultColors();
                currentTab = tabPage1;
                focusedRichTextBox = richTextBox1;
                focusedRichTextBox.MouseWheel += FocusedRichTextBox_MouseWheel;
                focusedRichTextBox.vScroll += scrollSyncTxtBox1_vScroll;
                lineNumbering();
            }
            return;
        }

        private void Snowman_FormClosed(object sender, FormClosedEventArgs e)
        {
            wplayer.controls.stop();
            wplayer.enabled = false;
            wplayer.close();
            var prc = Process.GetProcessesByName("wmplayer");
            if (prc.Length > 0) prc[prc.Length - 1].Kill();
        }

        private void scrollSyncTxtBox1_vScroll(Message msg)
        {

            msg.HWnd = richTextBox2.Handle;
            // msg.WParam = new IntPtr(0x1);
            //Console.WriteLine(msg.ToString());
            richTextBox2.PubWndProc(ref msg);
        }


        #endregion privateMethods

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Controls.Clear();
        }

        
    }
}
