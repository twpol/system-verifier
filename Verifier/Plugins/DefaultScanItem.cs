//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;

namespace JGR.SystemVerifier.Plugins
{
	public class DefaultScanItem : IScanItem
	{
		private string type;
		private Hashtable properties;

		public DefaultScanItem(string type) {
			this.type = type;
			this.properties = new Hashtable();
		}

		public DefaultScanItem(string type, DisplayItemSeverity severity, string description)
			: this(type)
		{
			this.properties["Severity"] = severity.ToString();
			this.properties["Description"] = description;
		}

		#region IScanItem Members

		public string Type {
			get {
				return type;
			}
		}

		public Hashtable Properties {
			get {
				return properties;
			}
		}

		#endregion
	}
}
