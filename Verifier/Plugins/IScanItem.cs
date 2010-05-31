//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

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
