using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveSplit.Model;
using System.Xml;
using LiveSplit.TimeFormatters;
using LiveSplit.Model.Comparisons;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.IO;

namespace LiveSplit.SpeedGuidesLive
{
    public partial class SGLSettings : UserControl
    {
        private const string s_xmlURL = "Components/LiveSplit.SpeedGuidesLive.xml";

        public static string XmlURL { get { return s_xmlURL; } }
        private static Size s_startingSize = new Size(469, 265);
        public static Size StartingSize { get { return s_startingSize; } }

        private SGLComponent m_component = null;

        public String Path { get; set; }

        private SGLGuideEditor m_editorWindow = null;

        private string m_fontName = "Consolas";
        private int m_fontSize = 12;

        private Font m_guideFont = null;

        private Color m_backgroundColor = Color.FromArgb(16, 16, 16);
        private Color m_textColor = Color.FromArgb(255, 255, 255);

        private Point m_windowPos = new Point(20, 20);
        private Size m_windowSize = new Size(StartingSize.Width, StartingSize.Height);

        private string m_activeSplitTxtOutputPath = "";

        public Font GuideFont { get { return m_guideFont; } }
        public delegate void FontChangedEventHandler(Font font);
        public FontChangedEventHandler FontChangedEvent { get; set; }

        public Color BackgroundColor { get { return m_backgroundColor; } }
        public Color TextColor { get { return m_textColor; } }

        public delegate void ColorChangedEventHandler(Color color);
        public ColorChangedEventHandler BackgroundColorChangedEvent { get; set; }
        public ColorChangedEventHandler TextColorChangedEvent { get; set; }

        public Point WindowPos { get { return m_windowPos; } set { m_windowPos = value; } }
        public Size WindowSize { get { return m_windowSize; } set { m_windowSize = value; } }

        public string ActiveSplitTxtOutputPath { get { return m_activeSplitTxtOutputPath; } }

        public bool MarkdownEnabled { get { return m_markdownEnabled.Checked; } }
        public delegate void MardownEnableChangeEventHandler(bool isEnabled);
        public MardownEnableChangeEventHandler MardownEnableChangedEvent { get; set; }

        // Debug
        public delegate void DebugCenterEventHandler();
        public DebugCenterEventHandler DebugCenterEvent { get; set; }
        public delegate void DebugResizeEventHandler();
        public DebugResizeEventHandler DebugResizeEvent { get; set; }

        public SGLSettings(SGLComponent component)
        {
            m_component = component;
            m_guideFont = new Font(m_fontName, (float)m_fontSize);

            //Init setting to default values
            InitializeComponent();

            fontComboBox.Items.Clear();
            foreach (FontFamily font in System.Drawing.FontFamily.Families)
            {
                fontComboBox.Items.Add(font.Name);
            }
            fontComboBox.SelectedItem = m_fontName;
            fontSizeNumeric.Value = m_fontSize;

            backgroundColorDisplay.BackColor = m_backgroundColor;
            textColorDisplay.BackColor = m_textColor;

            Load += Settings_Load;
            try
            {
                if(null != TopLevelControl)
                    ((Form)TopLevelControl).FormClosing += OnClosing;
            }
            catch (Exception)
            {

            }

            SGLGuideWindow.WindowCreatedEvent += OnSGLGuideWindowCreated;
            SGLGuideWindow.WindowClosedEvent += OnSGLGuideWindowClosed;
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            SGLGuideWindow.WindowCreatedEvent += OnSGLGuideWindowCreated;
            SGLGuideWindow.WindowClosedEvent += OnSGLGuideWindowClosed;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            labelVersionText.Text = SGLFactory.VersionString;
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            var settingsNode = document.CreateElement("Settings");
            settingsNode.AppendChild(ToElement(document, "Version", SGLFactory.VersionString));
            settingsNode.AppendChild(ToElement(document, "FontName", m_fontName));
            settingsNode.AppendChild(ToElement(document, "FontSize", m_fontSize));

            settingsNode.AppendChild(ToElement(document, "BGColor.R", m_backgroundColor.R));
            settingsNode.AppendChild(ToElement(document, "BGColor.G", m_backgroundColor.G));
            settingsNode.AppendChild(ToElement(document, "BGColor.B", m_backgroundColor.B));
            settingsNode.AppendChild(ToElement(document, "TXTColor.R", m_textColor.R));
            settingsNode.AppendChild(ToElement(document, "TXTColor.G", m_textColor.G));
            settingsNode.AppendChild(ToElement(document, "TXTColor.B", m_textColor.B));

            if (activeSplitTextCheckBox.Checked)
            {
                settingsNode.AppendChild(ToElement(document, "ActiveSplitTxtPath", m_activeSplitTxtOutputPath));
            }
            else
            {
                settingsNode.AppendChild(ToElement(document, "ActiveSplitTxtPath", ""));
            }

            settingsNode.AppendChild(ToElement(document, "WindowPos.X", m_windowPos.X));
            settingsNode.AppendChild(ToElement(document, "WindowPos.Y", m_windowPos.Y));
            settingsNode.AppendChild(ToElement(document, "WindowSize.Width", m_windowSize.Width));
            settingsNode.AppendChild(ToElement(document, "WindowSize.Height", m_windowSize.Height));

            settingsNode.AppendChild(ToElement(document, "MarkdownEnabled", m_markdownEnabled.Checked));

            return settingsNode;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Version version = Version.Parse(settings["Version"].InnerText);

            fontComboBox.SelectedItem = m_fontName = settings["FontName"] != null ? settings["FontName"].InnerText : m_fontName;
            fontSizeNumeric.Value =  m_fontSize = settings["FontSize"] != null ? int.Parse(settings["FontSize"].InnerText) : m_fontSize;
            UpdateFont();

            int bgColorR = settings["BGColor.R"] != null ? int.Parse(settings["BGColor.R"].InnerText) : m_backgroundColor.R;
            int bgColorG = settings["BGColor.G"] != null ? int.Parse(settings["BGColor.G"].InnerText) : m_backgroundColor.G;
            int bgColorB = settings["BGColor.B"] != null ? int.Parse(settings["BGColor.B"].InnerText) : m_backgroundColor.B;
            backgroundColorDisplay.BackColor = m_backgroundColor = Color.FromArgb(bgColorR, bgColorG, bgColorB);

            int txtColorR = settings["TXTColor.R"] != null ? int.Parse(settings["TXTColor.R"].InnerText) : m_textColor.R;
            int txtColorG = settings["TXTColor.G"] != null ? int.Parse(settings["TXTColor.G"].InnerText) : m_textColor.G;
            int txtColorB = settings["TXTColor.B"] != null ? int.Parse(settings["TXTColor.B"].InnerText) : m_textColor.B;
            textColorDisplay.BackColor = m_textColor = Color.FromArgb(txtColorR, txtColorG, txtColorB);

            m_activeSplitTxtOutputPath = settings["ActiveSplitTxtPath"] != null ? settings["ActiveSplitTxtPath"].InnerText : "";

            int windowPosX = settings["WindowPos.X"] != null ? int.Parse(settings["WindowPos.X"].InnerText) : m_windowPos.X;
            int windowPosY = settings["WindowPos.Y"] != null ? int.Parse(settings["WindowPos.Y"].InnerText) : m_windowPos.Y;
            m_windowPos = new Point(windowPosX, windowPosY);
            
            int windowSizeWidth = settings["WindowSize.Width"] != null ? int.Parse(settings["WindowSize.Width"].InnerText) : m_windowSize.Width;
            int windowSizeHeight = settings["WindowSize.Height"] != null ? int.Parse(settings["WindowSize.Height"].InnerText) : m_windowSize.Height;
            m_windowSize = new Size(windowSizeWidth, windowSizeHeight);

            activeSplitTextCheckBox.Checked = 0 != m_activeSplitTxtOutputPath.Length;

            bool markdown = settings["MarkdownEnabled"] != null ? bool.Parse(settings["MarkdownEnabled"].InnerText) : true;
            m_markdownEnabled.Checked = markdown;
            UpdateActiveSplitTextComponents();
        }

        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }

        private T TryGetFromXML<T>(System.Xml.XmlNode settings, string name, T defaultValue) where T : class
        {
            XmlNode node = settings[name];
            if (null == node)
                return defaultValue;
            return node.InnerText as T;
        }

        private void OnGuideEditorClosed(object sender, FormClosedEventArgs e)
        {
            m_editorWindow = null;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if(null == m_editorWindow)
            {
                m_editorWindow = new SGLGuideEditor(m_component.State);
                m_editorWindow.Show();
                m_editorWindow.FormClosed += OnGuideEditorClosed;
            }
        }

        private void fontComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_fontName = fontComboBox.SelectedItem as string;
            UpdateFont();
        }

        private void fontSizeNumeric_ValueChanged(object sender, EventArgs e)
        {
            m_fontSize = (int)fontSizeNumeric.Value;
            UpdateFont();
        }

        private void UpdateFont()
        {
            m_guideFont = new Font(m_fontName, (float)m_fontSize);
            if(null != FontChangedEvent)
                FontChangedEvent.Invoke(m_guideFont);
        }

        private void backgroundColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = m_backgroundColor;
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                backgroundColorDisplay.BackColor = m_backgroundColor = colorDialog1.Color;
                if(null != BackgroundColorChangedEvent)
                    BackgroundColorChangedEvent.Invoke(m_backgroundColor);
            }
        }

        private void textColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = m_backgroundColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textColorDisplay.BackColor = m_textColor = colorDialog1.Color;
                if (null != TextColorChangedEvent)
                    TextColorChangedEvent.Invoke(m_textColor);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.nightgamedev.com/");
        }

        private void labelVersionText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/iNightfaller/SpeedGuidesLive");
        }

        private void debugCenterButton_Click(object sender, EventArgs e)
        {
            if(null != DebugCenterEvent)
                DebugCenterEvent.Invoke();
        }

        private void debugResizeButton_Click(object sender, EventArgs e)
        {
            if(null != DebugResizeEvent)
                DebugResizeEvent.Invoke();
        }

        private void OnSGLGuideWindowCreated()
        {
            SGLGuideWindow.GuideWindow.Move += OnSGLGuideWindowMoved;
            SGLGuideWindow.GuideWindow.SizeChanged += OnSGLGuideWindowResized;
            UpdateSGLWindowPosition();
            UpdateSGLWindowSize();
        }

        private void OnSGLGuideWindowClosed()
        {
            SGLGuideWindow.GuideWindow.Move -= OnSGLGuideWindowMoved;
            SGLGuideWindow.GuideWindow.SizeChanged -= OnSGLGuideWindowResized;
            UpdateSGLWindowPosition(true);
            UpdateSGLWindowSize(true);
        }

        private void OnSGLGuideWindowMoved(object sender, EventArgs e)
        {
            UpdateSGLWindowPosition();
        }

        private void OnSGLGuideWindowResized(object sender, EventArgs e)
        {
            UpdateSGLWindowSize();
        }

        private void UpdateSGLWindowPosition(bool clear = false)
        {
            int x = 0;
            int y = 0;
            if(!clear && null != SGLGuideWindow.GuideWindow)
            {
                x = SGLGuideWindow.GuideWindow.Location.X;
                y = SGLGuideWindow.GuideWindow.Location.Y;
            }
            debugWindowPosLabel.Text = "(" + x.ToString() + ", " + y.ToString() + ")";
        }

        private void UpdateSGLWindowSize(bool clear = false)
        {
            int width = 0;
            int height = 0;
            if (!clear && null != SGLGuideWindow.GuideWindow)
            {
                width = SGLGuideWindow.GuideWindow.ClientSize.Width;
                height = SGLGuideWindow.GuideWindow.ClientSize.Height;
            }
            debugWindowSizeLabel.Text = "(" + width.ToString() + ", " + height.ToString() + ")";
        }

        private void requestLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://goo.gl/forms/XUd9JOLfXMFVSaUJ2");
        }

        private void UpdateActiveSplitTextComponents()
        {
            activeSplitTextDesc.Visible = !activeSplitTextCheckBox.Checked;
            activeSplitTextPath.Visible = activeSplitTextCheckBox.Checked;
            activeSplitTextPathBtn.Visible = activeSplitTextCheckBox.Checked;

            activeSplitTextPath.Text = m_activeSplitTxtOutputPath;
        }

        private void activeSplitTextCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateActiveSplitTextComponents();
        }

        private void activeSplitTextPathBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if(DialogResult.OK == dlg.ShowDialog(this))
            {
                m_activeSplitTxtOutputPath = dlg.FileName;
                string ext = System.IO.Path.GetExtension(m_activeSplitTxtOutputPath);
                if (0 == ext.Length)
                {
                    m_activeSplitTxtOutputPath += ".txt";
                }
                else if (ext != ".txt")
                {
                    m_activeSplitTxtOutputPath.Remove(m_activeSplitTxtOutputPath.Length - ext.Length);
                    m_activeSplitTxtOutputPath += ".txt";
                }
                UpdateActiveSplitTextComponents();
            }
            dlg.Dispose();
        }

        private void activeSplitTextPath_TextChanged(object sender, EventArgs e)
        {
            m_activeSplitTxtOutputPath = activeSplitTextPath.Text;
        }


		private void spreadsheetImport_Click(object sender, EventArgs e)
		{
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "tab-separated files (*.tsv)|*.tsv|txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 3;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;


                        Stream fs = openFileDialog.OpenFile();
                        using (StreamReader reader = new StreamReader(fs))
                        {
                            Guide guide = m_component.Guide;
                            if (null != guide)
                            {
                                // Copy all of the possible splits...
                                //  We do this just in case we have more than one split of the same name
                                List<Guide.Split> remainingSplits = new List<Guide.Split>(guide.Splits);
                                while(!reader.EndOfStream)
								{
                                    string line = reader.ReadLine();
                                    int firstTabIndex = line.IndexOf('\t');
                                    if(-1 == firstTabIndex)
									{
                                        continue;
									}

                                    string name = line.Substring(0, firstTabIndex);
                                    string note = ((line.Length - 1) != firstTabIndex) ? line.Substring(firstTabIndex + 1) : "";

                                    // Look for the first split with this name
                                    for(int i = 0; i < remainingSplits.Count; ++i)
									{
                                        if(remainingSplits[i].Name == name)
										{
                                            remainingSplits[i].Note = note;
                                            remainingSplits.RemoveAt(i);
                                            break;
										}
									}
                                }

                                string existingFileName = Guide.SplitsPathToGuidePath(m_component.State.Run.FilePath);
                                string backupFileName = System.IO.Path.GetDirectoryName(m_component.State.Run.FilePath) + "/" + System.IO.Path.GetFileNameWithoutExtension(m_component.State.Run.FilePath) + ".backup.sgl";
                                if (File.Exists(existingFileName))
                                {
                                    try
                                    {
                                        File.Copy(existingFileName, backupFileName, true);
                                        guide.Save(m_component.State.Run.FilePath);
                                        System.Windows.Forms.MessageBox.Show(this, "We saved a backup of your old guide file at:\n\n\"" + backupFileName + "\"", "Guide succesfully imported");
                                    }
                                    catch (System.Exception)
                                    { 
                                    }
                                }
                                else
								{
                                    guide.Save(m_component.State.Run.FilePath);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception exception) 
            {
                System.Windows.Forms.MessageBox.Show(this, "Failed to import tsv: " + exception.Message, "Import Failed!");
            }
        }

		private void spreadsheetExport_Click(object sender, EventArgs e)
		{
            SaveFileDialog dlg = new SaveFileDialog();
            if (DialogResult.OK == dlg.ShowDialog(this))
            {
                string fileName = dlg.FileName;
                fileName = System.IO.Path.GetDirectoryName(fileName) + "/" + System.IO.Path.GetFileNameWithoutExtension(fileName) + ".tsv";

                string outputFile = "";
                Guide guide = m_component.Guide;
                if (null != guide)
                {
                    for (int i = 0; i < guide.Splits.Count; ++i)
                    {
                        if (0 != i)
                        {
                            outputFile += '\n';
                        }
                        string name = guide.Splits[i].Name;
                        string note = guide.Splits[i].Note;

                        if(name.Contains('\t'))
						{
                            System.Windows.Forms.MessageBox.Show(this, "You have a split name with a tab in it and cannot export...\nPlease remove any tabs from your split names, and try again\nSplit Name: \"" + name + "\"", "Export Failed!");
                            return;
                        }
                        outputFile += name;
                        outputFile += '\t';
                        outputFile += note;
                    }
                }

                if (!File.Exists(fileName))
                {
                    if (!Directory.Exists(System.IO.Path.GetDirectoryName(fileName)))
                        return;
                }
                try
                {
                    System.IO.File.WriteAllText(fileName, outputFile);
                }
                catch (Exception exception)
                {
                    System.Windows.Forms.MessageBox.Show(this, "Failed to export tsv: " + exception.Message, "Export Failed!");
                }
            }
            dlg.Dispose();
        }

        private void markdownEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MardownEnableChangedEvent.Invoke(m_markdownEnabled.Checked);
        }
    }
}
