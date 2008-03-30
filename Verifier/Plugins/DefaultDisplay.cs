using System;
using System.Collections.Generic;
using System.Text;

namespace JGR.SystemVerifier.Plugins
{
	public class DefaultDisplay : IDisplay
	{
		#region IDisplay Members

		public bool Accepts(IScanItem item) {
			return true;
		}

		public IDisplayItem Process(IScanItem item) {
			DisplayItemSeverity severity = DisplayItemSeverity.Critical;
			if (item.Properties.ContainsKey("Severity")) {
				severity = (DisplayItemSeverity)Enum.Parse(typeof(DisplayItemSeverity), item.Properties["Severity"].ToString());
			}
			if (item.Properties.ContainsKey("Description")) {
				return new DefaultDisplayItem(severity, item.Type, item.Properties["Description"].ToString());
			}
			return new DefaultDisplayItem(severity, item.Type, "");
		}

		#endregion
	}
}
