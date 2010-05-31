//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace JGR.SystemVerifier.Plugins
{
	public interface IScanner
	{
		/// <summary>
		/// Gets the current number of items which have been processed by the scanner.
		/// </summary>
		/// <remarks>
		/// The <see cref="Current"/> value should start out as <value>0</value> until Process has been called.
		/// </remarks>
		long Current { get; }

		/// <summary>
		/// Gets the current number of items expected to be processed by the scanner.
		/// </summary>
		/// <remarks>
		/// <para>
		///		Ideally, the scanner will calculate the correct number of items to be processed in the <see cref="PreProcess"/> method.
		///		If the number of items cannot be determined during <see cref="PreProcess"/>, you must only ensure that <see cref="Maximum"/>
		///		is greater than or equal to <see cref="Current"/> after returning from <see cref="Process"/> while there are more items.
		/// </para>
		/// <para>
		///		The value returned by <see cref="Maximum"/> is checked just before each call to <see cref="Process"/>.
		/// </para>
		/// </remarks>
		long Maximum { get; }

		/// <summary>
		/// Sets up the scanner's environment and, when possible, calculates the number of items expected to be processed.
		/// </summary>
		/// <remarks>
		/// Often, some resouces will be needed for the duration of the scanner. These should be created and initialized here, and
		/// closed, finalised, etc. in <see cref="PostProcess"/>.
		/// </remarks>
		void PreProcess();

		/// <summary>
		/// Processes one or more pending items in the scanner.
		/// </summary>
		/// <remarks>
		/// For most operations, <see cref="PreProcess"/> is ideal for creating a list of items that need processing, and then processing
		/// one in each call to <see cref="Process"/>.
		/// </remarks>
		/// <returns>A list of one or more objects implementing <see cref="IScanItem"/>. How many are returned is not important, and the scanner is free to return an empty list.</returns>
		List<IScanItem> Process();

		/// <summary>
		/// Cleans up and finalises any parts of the scanner that may need in.
		/// </summary>
		/// <remarks>
		/// Often, some resources will be opened (like Registry Keys) by <see cref="PreProcess"/> for the scanner to perform its operations.
		/// <see cref="PostProcess"/> is the recommended place to close any resources which need it.
		/// </remarks>
		void PostProcess();
	}
}
