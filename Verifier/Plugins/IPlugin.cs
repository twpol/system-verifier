//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace JGR.SystemVerifier.Plugins
{
	public interface IPlugin
	{
		string Name { get; }
		string Description { get; }
		string[] Authors { get; }
	}

	public interface IPluginWithHost
	{
		void Init(IPluginHost host);
	}

	public interface IPluginWithSections
	{
		List<KeyValuePair<string, long>> Sections { get; }
		void SetSections(List<long> sections);
	}
}
