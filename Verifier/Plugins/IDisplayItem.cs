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
