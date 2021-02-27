using LiveSplit.SpeedGuidesLive;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveSplit.Model;

[assembly: ComponentFactory(typeof(SGLFactory))]

namespace LiveSplit.SpeedGuidesLive
{
    class SGLFactory : IComponentFactory
    {
        private const int s_versionMajor = 1;
        private const int s_versionMinor = 3;
        private const int s_versionPatch = 1;

        public static string VersionString
        {
            get { return s_versionMajor.ToString() + '.' + s_versionMinor + '.' + s_versionPatch; }
        }

        private const string s_updateURL = "https://github.com/iNightfaller/SpeedGuidesLive";

#region IComponentFactory Interface
        public string ComponentName { get { return SGLComponent.Name; } }
        public string Description { get { return SGLComponent.Description; } }
        public ComponentCategory Category { get { return ComponentCategory.Other; } }
        public string UpdateName { get { return "TODO: SGLFactory.UpdateName"; } }
        public string XMLURL { get { return SGLSettings.XmlURL; } }
        public string UpdateURL { get { return s_updateURL; } }
        public Version Version { get { return Version.Parse(VersionString); } }
        public IComponent Create(LiveSplitState state) { return new SGLComponent(state); }
#endregion
    }
}
