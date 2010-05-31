//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

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

		Dictionary<string, uint> VerifyCOMClassID(string classID, List<string> interfaces, long bitness);
	}
}
