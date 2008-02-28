using System;
using System.Collections.Generic;
using System.Text;

namespace JGR.SystemVerifier.Plugins
{
	public class DisplayDisplay : IDisplay
	{
		#region IDisplay Members

		public bool Accepts(IScanItem item) {
			return true;
		}

		public IDisplayItem Process(IScanItem item) {
			if (item.Properties.ContainsKey("Description")) {
				return new DefaultDisplayItem(DisplayItemSeverity.Critical, item.Type, item.Properties["Description"].ToString());
			}
			return new DefaultDisplayItem(DisplayItemSeverity.Critical, item.Type, "");
		}

		#endregion
	}
}
