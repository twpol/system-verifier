//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace JGR.SystemVerifier.Plugins
{
	public class DefaultRegistryDisplay : IDisplay
	{
		public DefaultRegistryDisplay() {
		}

		#region IDisplay Members

		public bool Accepts(IScanItem item) {
			return ((item.Type == "RegistryKey") || (item.Type == "RegistryValue") || (item.Type == "RegistryData"));
		}

		public IDisplayItem Process(IScanItem item) {
			DisplayItemSeverity severity = DisplayItemSeverity.Information;
			string name = item.Type;
			string description = "";

			switch (item.Type) {
				case "RegistryKey":
					switch ((string)item.Properties["ItemIssue"]) {
						case "Missing":
							severity = DisplayItemSeverity.Error;
							name = "Missing Key";
							description = "The registry key <" + item.Properties["Key"] + "> is missing from your system.";
							break;

						case "Extra":
							severity = DisplayItemSeverity.Verbose;
							name = "Extra Key";
							description = "The registry key <" + item.Properties["Key"] + "> was found on your system but it should not exist.";
							break;
					}
					break;

				case "RegistryValue":
					break;

				case "RegistryData":
					break;
			}

			return new DefaultDisplayItem(severity, name, description);
		}

		#endregion
	}
}
