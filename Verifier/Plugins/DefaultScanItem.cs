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
