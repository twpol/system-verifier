using System;
using System.Collections;

namespace JGR.SystemVerifier.Plugins
{
	public interface IScanItem
	{
		string Type { get; }
		Hashtable Properties { get; }
	}
}
