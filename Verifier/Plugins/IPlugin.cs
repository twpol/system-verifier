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
}
