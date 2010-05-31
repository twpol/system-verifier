//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;

namespace JGR.SystemVerifier.Plugins
{
	public interface IDisplay
	{
		bool Accepts(IScanItem item);
		IDisplayItem Process(IScanItem item);
	}
}
