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
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\config.xml";

        //string projectName, Font font, int indent, string ncolor, string scolor, string acolor, string tcolor
        public static void updateXml(TabControl tabControl, List<string> crayonsSet)
        {

            #region writer
            /*XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(path + "\\config.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");
            foreach (TabPage tab in tabControl.TabPages)
            {
                if(tab.Tag != null)
                {
                    var box = tab.Controls.OfType<RichTextBoxSynchronizedScroll>().First();
                    writer.WriteStartElement("Setting");
                    writer.WriteElementString("projectName", tab.Tag.ToString());
                    writer.WriteElementString("fontSize", box.Font.Size.ToString());
                    writer.WriteElementString("fontFamily", box.Font.FontFamily.Name);
                    writer.WriteElementString("fontStyle", box.Font.Style.ToString());
                    //writer.WriteElementString("indent", indent.ToString());
                    writer.WriteEndElement();
                    
                }
                

            }
            writer.WriteStartElement("Setting");
            writer.WriteElementString("nodeColor", crayonsSet[0]);
            writer.WriteElementString("stringColor", crayonsSet[1]);
            writer.WriteElementString("attributeColor", crayonsSet[2]);
            writer.WriteElementString("textColor", crayonsSet[3]);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();*/
            #endregion writer
            string mark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            doc.RemoveAll();

            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlNode root = doc.CreateElement("Settings");

            foreach (TabPage tab in tabControl.TabPages)
            {
                if (tab.Tag != null)
                {
                    var box = tab.Controls.OfType<RichTextBoxSynchronizedScroll>().First();
                    XmlNode file = doc.CreateElement("File");
                    file.AppendChild(doc.CreateElement("projectName")).InnerText = tab.Tag.ToString();
                    file.AppendChild(doc.CreateElement("fontSize")).InnerText = box.Font.Size.ToString();
                    file.AppendChild(doc.CreateElement("fontFamily")).InnerText = box.Font.FontFamily.Name;
                    file.AppendChild(doc.CreateElement("fontStyle")).InnerText = box.Font.Style.ToString();
                    //file.AppendChild(doc.CreateElement("indent", indent.ToString()));
                    root.AppendChild(file);
                }

            }
            XmlNode colors = doc.CreateElement("Colors");
            colors.AppendChild(doc.CreateElement("nodeColor")).InnerText = crayonsSet[0];
            colors.AppendChild(doc.CreateElement("stringColor")).InnerText = crayonsSet[1];
            colors.AppendChild(doc.CreateElement("attributeColor")).InnerText = crayonsSet[2];
            colors.AppendChild(doc.CreateElement("textColor")).InnerText = crayonsSet[3];
            root.AppendChild(colors);
            doc.AppendChild(root);
            doc.Save(path);
            
        }

        public static List<string> XmlLoad()
        {
            List<string> paths = new List<string>();
            using (XmlReader xmlReader = XmlReader.Create(path))
            {
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "projectName"))
                    {
                        paths.Add(xmlReader.ReadInnerXml());

                    }
                }
            }
            return paths;
        }

        public static void createConfig()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(path);
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");


            writer.WriteStartElement("Font");
            /*writer.WriteElementString("projectName", @"C:\Users\baszd\Desktop\testxml\shiporder.xml");
            writer.WriteElementString("projectName", @"C:\Users\Licht\Desktop\shiporder.xml")*/
            writer.WriteElementString("fontSize", "12");
            writer.WriteElementString("fontFamily", "Times New Roman");
            writer.WriteElementString("fontStyle", "Regular");
            writer.WriteElementString("indent", "2");
            writer.WriteEndElement();
            writer.WriteStartElement("Color");
            /*writer.WriteElementString("projectName", @"C:\Users\baszd\Desktop\testxml\shiporder.xml");
            writer.WriteElementString("projectName", @"C:\Users\Licht\Desktop\shiporder.xml")*/
            writer.WriteElementString("nodeColor", "Aqua");
            writer.WriteElementString("stringColor", "Bisque");
            writer.WriteElementString("attributeColor", "Coral");
            writer.WriteElementString("textColor", "GreenYellow");
            writer.WriteEndElement();



            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        public static Font loadFontConfig()
        {
            Font font;
            string family = "";
            float size = 0;
            FontStyle style;
            string sStyle = "";

            using (XmlReader xmlReader = XmlReader.Create(path))
            {
                while (xmlReader.Read())
                {

                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "fontSize"))
                    {
                        size = float.Parse(xmlReader.ReadInnerXml());

                    }
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "fontFamily"))
                    {
                        family = xmlReader.ReadInnerXml();

                    }
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "fontStyle"))
                    {
                        sStyle = xmlReader.ReadInnerXml();

                    }
                }
            }
            style = (FontStyle)Enum.Parse(typeof(FontStyle), sStyle, true);
            font = new Font(family, size, style);
            return font;
        }

        public static List<string> loadColorConfig()
        {
            List<string> crayonsSet = new List<string>();
            using (XmlReader xmlReader = XmlReader.Create(path))
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
            return crayonsSet;
        }
    }
}
