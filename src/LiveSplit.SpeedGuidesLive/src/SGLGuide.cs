using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LiveSplit.SpeedGuidesLive
{
    [XmlRoot("Guide")]
    [XmlInclude(typeof(Guide))]
    public class Guide
    {
        public delegate void GuideSavedEventHandler(Guide guide);
        public static GuideSavedEventHandler GuideSavedEvent { get; set; }

        public class Split
        {
            [XmlElement("Name")]
            public string Name = "";
            [XmlElement("Note")]
            public string Note = "";

            public Split(string name)
            {
                Name = name;
            }

            public Split()
            {
            }
        }

        [XmlArray("Splits")]
        [XmlArrayItem("Split")]
        public List<Split> Splits = new List<Split>();
        [XmlElement("Name")]
        public string Name = "";
        [XmlElement("Category")]
        public string Category = "";

        public Guide()
        {
        }

        public static string SplitsPathToGuidePath(string splitsPath)
        {
            string guidePath = splitsPath;
            guidePath = Path.GetDirectoryName(guidePath) + "/" + Path.GetFileNameWithoutExtension(splitsPath) + ".sgl";
            return guidePath;
        }

        public static Guide Load(string splitsPath)
        {
            // Get the guide path from the splits path
            string guidePath = SplitsPathToGuidePath(splitsPath);
            if (0 == guidePath.Length || guidePath == splitsPath)
            {
                return null;
            }

            try
            {
                // Make sure the file exists
                if (!File.Exists(guidePath))
                {
                    return null;
                }

                // Load the document
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(guidePath);

                string xmlString = xmlDocument.OuterXml;
                // Please.. don't ask why I'm checking for this...
                //  Address a deprecated XMLRoot name
                string deprecatedXMLRoot = "PersonenListe";
                if (xmlDocument.DocumentElement.Name == deprecatedXMLRoot)
				{
                    // First
                    int indexToModify = xmlString.IndexOf(deprecatedXMLRoot);
                    xmlString = xmlString.Remove(indexToModify, deprecatedXMLRoot.Length);
                    xmlString = xmlString.Insert(indexToModify, "Guide");
                    // Last
                    indexToModify = xmlString.LastIndexOf(deprecatedXMLRoot);
                    xmlString = xmlString.Remove(indexToModify, deprecatedXMLRoot.Length);
                    xmlString = xmlString.Insert(indexToModify, "Guide");
                }

                // Deserialize the Guide
                XmlSerializer serializer = new XmlSerializer(typeof(Guide));
                using (var stream = new StringReader(xmlString))
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        Guide guide = (Guide)serializer.Deserialize(reader);

                        reader.Close();
                        stream.Close();

                        // To normalize newlines
                        foreach (Split split in guide.Splits)
                        {
                            split.Note = split.Note.Replace("\r\n", "\n");
                            split.Note = split.Note.Replace("\n", "\r\n");
                        }

                        return guide;
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Failed to load the guide... Error: " + e.Message);
            }

            return null;
        }

        public void Save(string splitsPath)
        {
            // Get the guide path from the splits path
            string guidePath = SplitsPathToGuidePath(splitsPath);
            // Make sure the path is valid
            if (0 == guidePath.Length || guidePath == splitsPath)
            {
                return;
            }

            bool success = true;
            // To normalize newlines
            foreach (Split split in Splits)
            {
                split.Note = split.Note.Replace("\n", "\r\n");
                split.Note = split.Note.Replace("\r\n", "\n");
            }

            try
            {
                // Deserialize the guide
                XmlSerializer serializer = new XmlSerializer(typeof(Guide));
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, this);
                    stream.Position = 0;

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(stream);
                    xmlDocument.Save(guidePath);

                    stream.Close();
                }
            }
            catch(System.Exception e)
            {
                Console.WriteLine("Failed to save the guide... Error: " + e.Message);
                success = false;
            }

            // To normalize newlines
            foreach (Split split in Splits)
            {
                split.Note = split.Note.Replace("\r\n", "\n");
                split.Note = split.Note.Replace("\n", "\r\n");
            }

            // If this was a success then publish an event
            if (success && null != GuideSavedEvent)
            {
                GuideSavedEvent.Invoke(this);
            }
        }
    }
}
