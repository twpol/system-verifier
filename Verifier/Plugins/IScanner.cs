using System;
using System.Collections.Generic;

namespace JGR.SystemVerifier.Plugins
{
	public interface IScanner
	{
		long Current { get; }
		long Maximum { get; }
		List<IScanItem> Process();
	}
}
