using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace AppServices
{
    public  class ClassXml
    {
        public string GetXMLPath()
        {
             // return System.AppDomain.CurrentDomain.BaseDirectory;

             return @"C:\ForteXml\ForteVD";
        }
        public string XMLGdvFilePath
        {
            get { return Path.Combine(GetXMLPath(), "GridviewItems.xml"); }
        }
        public string XMLHdrFilePath
        {
            get { return Path.Combine(GetXMLPath(), "HdrItems.xml"); }
        }

        public ClassXml() 
        {
            CheckandCreateXMLFiles("GridviewItems.xml", "CustomGridView");
            CheckandCreateXMLFiles("GdvRealtimeList.xml", "CustomGridView");
        }

        public void CheckandCreateXMLFiles(string xmlfile, string StartElement)
        {
            try
            {
                bool exists = System.IO.Directory.Exists(GetXMLPath());
                if (!exists)
                    System.IO.Directory.CreateDirectory(GetXMLPath());


                String FileLocation = Path.Combine(GetXMLPath(), xmlfile);

                if (!System.IO.File.Exists(FileLocation))
                {
                    ClsSerilog.LogMessage(ClsSerilog.INFO, $"Create xml file " + FileLocation);

                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(FileLocation, settings))
                    {
                        //Begin write
                        writer.WriteStartDocument();
                        //Node
                        writer.WriteStartElement(StartElement);

                        writer.WriteEndDocument();
                        writer.Close();
                    }

                  
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in CheckandCreateXMLFiles {ex.Message}");
            }
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Checked all XML files -> {xmlfile}" );
        }

        public List<string> ReadXmlGridView(string FileLocation)
        {
            List<string> XmlGridView = new List<string>();
            XmlGridView.Clear();
            XmlDocument? doc = new();

            try
            {
                if (File.Exists(FileLocation))
                {
                    doc.Load(FileLocation);
                    XmlNodeList? xnl = doc.SelectNodes("CustomGridView/Field/Name");


                    if ((xnl != null) && (xnl.Count > 0))
                    {
                        foreach (XmlNode xn in xnl)
                        {
                            if (File.Exists(FileLocation))
                                XmlGridView.Add(xn.InnerText);
                        }
                    }
                }
                if (XmlGridView.Count == 0) XmlGridView.Add("Forte");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error in ReadXmlGridView - " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"Error in  ReadXmlGridView -> {ex.Message}");
            }
            return XmlGridView;
        }

        public void UpdateXMlcolumnList(ObservableCollection<string> selectedHdrList, string settingsGdvFile)
        {

            try
            {
                if (File.Exists(settingsGdvFile))
                {
                    File.SetAttributes(settingsGdvFile, FileAttributes.Normal);

                    File.Delete(settingsGdvFile);

                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(settingsGdvFile, settings))
                    {
                        //Begin write
                        writer.WriteStartDocument();
                        //Node
                        writer.WriteStartElement("CustomGridView");

                        foreach (var item in selectedHdrList)
                        {
                            writer.WriteStartElement("Field");
                            writer.WriteElementString("Name", item);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error in UpdateXMlcolumnList " + ex);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in  UpdateXMlcolumnList -> {ex.Message}");
            }
        }

        public void WriteXmlGridView(List<CheckedListItem> StringsListBox, string FileLocation)
        {

            try
            {
                if (System.IO.File.Exists(FileLocation))
                    System.IO.File.Delete(FileLocation);

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(FileLocation, settings))
                {
                    //Begin write
                    writer.WriteStartDocument();
                    //Node
                    writer.WriteStartElement("CustomGridView");

                    foreach (var item in StringsListBox)
                    {
                        writer.WriteStartElement("Field");
                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("Name", item.Name);
                        writer.WriteElementString("FieldType", item.FieldType);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndDocument();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error in WriteXmlGridView - " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in  WriteXmlGridView -> {ex.Message}");
            }
        }
    }
}
