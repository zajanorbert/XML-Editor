﻿using System;
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

        public XMLEditor9000()
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

        private void XMLEditor9000_Load(object sender, EventArgs e)
        {

        }

        private void openXMLFileToolStripMenuItem_Click_1(object sender, MouseEventArgs me)
        {
            Stream xmlStream;
            Stream xsdStream;
            string xsdPath = "";
            OpenFileDialog xmlFileDialog = new OpenFileDialog();
            xmlFileDialog.Filter = "XML files (*.xml)|*.xml";
            OpenFileDialog xsdFileDialog = new OpenFileDialog();
            xsdFileDialog.Filter = "XSD files (*.xsd)|*.xsd";
            _validationComments = new List<string>();


            bool isValid = false;
            string xmlPath = "";
            try
            {
                XmlReaderSettings rearderSettings = new XmlReaderSettings();
                rearderSettings.ValidationType = ValidationType.Schema;
                rearderSettings.ValidationFlags |=
                  XmlSchemaValidationFlags.ProcessSchemaLocation |
                  XmlSchemaValidationFlags.ReportValidationWarnings |
                  XmlSchemaValidationFlags.ProcessIdentityConstraints |
                  XmlSchemaValidationFlags.AllowXmlAttributes;


                
                if (xmlFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if ((xmlStream = xmlFileDialog.OpenFile()) != null)
                    {
                        //string title = "TabPage " + (tabControl1.TabCount + 1).ToString();
                        xmlPath = xmlFileDialog.FileName;
                        RichTextBox box = new RichTextBox();
                        box.Dock = DockStyle.Fill;
                        TabPage tab = addTab(sender, xmlPath, me);

                        if (xsdFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            if ((xsdStream = xsdFileDialog.OpenFile()) != null)
                            {
                                xsdPath = xsdFileDialog.FileName;
                                string xsdText = File.ReadAllText(xsdPath);
                                listBox1.Items.Add("XSD file loaded");
                            }
                        }

                        rearderSettings.ValidationEventHandler += new ValidationEventHandler(XmlValidationEventHandler);
                        rearderSettings.Schemas.Add(null, XmlReader.Create(xsdPath));

                        using (XmlReader xmlValidatingReader = XmlReader.Create(xmlPath, rearderSettings))
                        {
                            while (xmlValidatingReader.Read())
                            {
                                box.Text = xmlValidatingReader.ReadOuterXml();

                            }
                        }

                        //string xmlText = File.ReadAllText(xmlPath);

                        tab.Controls.Add(box);
                        focusedRichTextBox = box;
                        listBox1.Items.Add("XML file loaded");

                    }
                }


            }
            catch (Exception error)
            {
                isValid = false;
                listBox1.Items.Add("Exception " + error.Message);
            }

            ValidationReport(System.IO.Path.GetFileName(xmlPath));
            //return isValid;
        }

        private void ValidationReport(string fileName)
        {


            listBox1.Items.Add("|=======================================|");
            if (_issueCounter > 0)
            {
                listBox1.Items.Add(fileName +" is not valid");
            }
            else
            {
                listBox1.Items.Add(fileName + " is valid");
            }
            //listBox1.Items.Add("Xml {0}", _issueCounter > 0 ? "is not valid" : "is valid");
            if (_issueCounter > 0)
            {
                listBox1.Items.Add("Warnings or Errors: " + _issueCounter);
                foreach (var comment in _validationComments) { listBox1.Items.Add(comment); }
            }
            listBox1.Items.Add("|=======================================|");
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
            }
        }
        #endregion tabcontrol

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
