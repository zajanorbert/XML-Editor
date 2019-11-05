using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace XML_Editor
{


    class xmlHandler
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        //string projectName, Font font, int indent, string ncolor, string scolor, string acolor, string tcolor
        public void updateXml(TabControl tabControl, string ncolor, string scolor, string acolor, string tcolor)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(path + "\\config.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");
            foreach (TabPage tab in tabControl.TabPages)
            {
                var box = tab.Controls.OfType<RichTextBoxSynchronizedScroll>().First();
                writer.WriteStartElement("Setting");
                writer.WriteElementString("projectName", tab.Tag.ToString());
                writer.WriteElementString("fontSize", box.Font.Size.ToString());
                writer.WriteElementString("fontFamily", box.Font.FontFamily.ToString());
                writer.WriteElementString("fontStyle", box.Font.Style.ToString());
                //writer.WriteElementString("indent", indent.ToString());
                writer.WriteEndElement();

            }
            writer.WriteStartElement("Setting");
            writer.WriteElementString("nodeColor", ncolor);
            writer.WriteElementString("stringColor", scolor);
            writer.WriteElementString("attributeColor", acolor);
            writer.WriteElementString("textColor", tcolor);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        public static List<string> XmlLoad()
        {
            List<string> paths = new List<string>();
            XmlReader xmlReader = XmlReader.Create(path + "\\config.xml");
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "projectName"))
                {
                    paths.Add(xmlReader.ReadInnerXml());
                    Console.WriteLine(xmlReader.ReadInnerXml());
                }
            }
            return paths;
        }

        public static void createConfig()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(path + "\\config.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");

            
            writer.WriteStartElement("Setting");
            /*writer.WriteElementString("projectName", @"C:\Users\baszd\Desktop\testxml\shiporder.xml");
            writer.WriteElementString("projectName", @"C:\Users\Licht\Desktop\shiporder.xml")*/
            writer.WriteElementString("fontSize", "12");
            writer.WriteElementString("fontFamily", "Times New Roman");
            writer.WriteElementString("fontStyle", "Italic");
            writer.WriteElementString("indent", "2");
            writer.WriteEndElement();


            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }
    }
}
