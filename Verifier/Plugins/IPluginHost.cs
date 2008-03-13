using System;
using System.Collections.Generic;
using System.Text;

namespace JGR.SystemVerifier.Plugins
{
	public interface IPluginHost
	{
		string OS { get; }
		long OSMinor { get; }
		long OSMajor { get; }
		long Bitness { get; }
	}
}
