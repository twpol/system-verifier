using System;

namespace JGR.SystemVerifier.Plugins
{
	public interface IDisplay
	{
		bool Accepts(IScanItem item);
		IDisplayItem Process(IScanItem item);
	}
}
