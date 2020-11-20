using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveSplit.Model;
using LiveSplit.UI;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SpeedGuidesLive
{
    public class SGLComponent : LogicComponent
    {
        private const string s_componentName = "Speed Guides Live";
        public static string Name { get { return s_componentName; } }
        private const string s_componentDesc = "A tool used to relay information about the current split.";
        public static string Description { get { return s_componentDesc; } }

        private SGLSettings m_settings = null;
        protected LiveSplitState m_state = null;
        public LiveSplitState State { get { return m_state; } }

        private SGLGuideWindow m_guideWindow = null;
        private Guide m_guide = null;
        public Guide Guide { get { return m_guide; } }

#region LogicComponent Interface

        public override string ComponentName { get { return Name; } }
        public SGLSettings Settings {  get { return m_settings; } }

        public SGLComponent(LiveSplitState state)
        {
            m_settings = new SGLSettings(this);
            m_state = state;

            m_state.OnStart += M_state_OnStart;
            m_state.OnSplit += M_state_OnSplit;
            m_state.OnReset += M_state_OnReset;
            m_state.OnSkipSplit += M_state_OnSkipSplit;
            m_state.OnUndoSplit += M_state_OnUndoSplit;
            m_state.RunManuallyModified += M_state_RunManuallyModified;

            Guide.GuideSavedEvent += OnGuideUpdated;
            LoadGuide();
            UpdateGuideWindow();
        }

        public override void Dispose()
        {
            HideGuideWindow();

            m_state.OnStart -= M_state_OnStart;
            m_state.OnSplit -= M_state_OnSplit;
            m_state.OnReset -= M_state_OnReset;
            m_state.OnSkipSplit -= M_state_OnSkipSplit;
            m_state.OnUndoSplit -= M_state_OnUndoSplit;
            m_state.RunManuallyModified -= M_state_RunManuallyModified;

            Guide.GuideSavedEvent -= OnGuideUpdated;
        }

        public override System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return m_settings.GetSettings(document);
        }

        public override System.Windows.Forms.Control GetSettingsControl(UI.LayoutMode mode)
        {
            return m_settings;
        }

        public override void SetSettings(System.Xml.XmlNode settings)
        {
            m_settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (null != invalidator && (null == m_guideWindow || !m_guideWindow.Visible))
                UpdateGuideWindow();
        }

#endregion
        
        private void UpdateGuideWindow()
        {
            if (null == m_state || null == m_state.Form ||
                !m_state.Form.Visible || null == m_state.Layout)
            {
                return;
            }

            try
            {
                m_guideWindow = SGLGuideWindow.GuideWindow;
                if (null == m_guideWindow)
                {
                    m_guideWindow = new SGLGuideWindow(this, m_state.Form, m_state.Layout, m_guide);
                    m_guideWindow.Show(m_state.Form);
                    m_state.Form.Focus();
                }

                m_guideWindow.SetGuide(m_guide);
                m_guideWindow.SetSplit(m_state.CurrentSplit, m_state.CurrentSplitIndex);
            }
            catch(System.Exception)
            {
                if (null != m_guideWindow)
                {
                    m_guideWindow.Close();
                    m_guideWindow.Dispose();
                    m_guideWindow = null;
                }
            }
        }

        private void HideGuideWindow()
        {
            if (null != m_guideWindow)
            {
                m_guideWindow.Close();
                m_guideWindow = null;
            }
        }

        private void M_state_OnStart(object sender, EventArgs e)
        {
            UpdateGuideWindow();
        }

        private void M_state_OnSplit(object sender, EventArgs e)
        {
            UpdateGuideWindow();
        }

        private void M_state_OnUndoSplit(object sender, EventArgs e)
        {
            UpdateGuideWindow();
        }

        private void M_state_OnSkipSplit(object sender, EventArgs e)
        {
            UpdateGuideWindow();
        }

        private void M_state_OnReset(object sender, TimerPhase value)
        {
            UpdateGuideWindow();
        }

        private void M_state_RunManuallyModified(object sender, EventArgs e)
        {
            LoadGuide();
        }

        private void LoadGuide()
        {
            m_guide = Guide.Load(m_state.Run.FilePath);
        }

        private void OnGuideUpdated(Guide guide)
        {
            LoadGuide();
            UpdateGuideWindow();
        }
    }
}
