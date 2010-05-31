//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;

namespace JGR.SystemVerifier.Plugins
{
	public enum DisplayItemSeverity {
		Verbose,
		Information,
		Warning,
		Error,
		Critical
	}

	public interface IDisplayItem
	{
		DisplayItemSeverity Severity { get; }
		string Name { get; }
		string Description { get; }
	}
}
