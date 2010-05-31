//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace JGR.SystemVerifier.Plugins
{
	public class DefaultDisplayItem : IDisplayItem
	{
		private DisplayItemSeverity severity;
		private string name;
		private string description;

		public DefaultDisplayItem(DisplayItemSeverity severity, string name, string description) {
			this.severity = severity;
			this.name = name;
			this.description = description;
		}

		#region IDisplayItem Members

		public DisplayItemSeverity Severity {
			get {
				return severity;
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public string Description {
			get {
				return description;
			}
		}

		#endregion
	}
}
