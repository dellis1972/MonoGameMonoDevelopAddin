using System;
using System.Collections.Generic;
using System.Text;

namespace MonoDevelop.MonoGame
{
	public class ContentBuilderArgs
	{
		public ContentBuilderArgs ()
		{
			References = new List<string>();
			ProcessorParams = new List<string>();
		}

		public string WorkingDirectory { get; set; }

		public string OutputDir { get; set; }

		public string IntermediateDir { get;set;}

		public bool Rebuild {get;set;}

		public bool Clean {get;set;}

		public List<string> References {get;set;}

		public string Importer {get;set;}

		public string Processor {get;set;}

		public List<string> ProcessorParams {get;set;}

		public string FilePath {get;set;}

		public string Platform { get;set; }
		/// <summary>
		/// Process the parameters to arguments
		/// </summary>
		/// <returns>
		/// The arguments for the MGCB app
		/// </returns>
		public string ToArgs()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("/outputDir:{0} ", OutputDir);

			if (!string.IsNullOrEmpty(IntermediateDir))
				sb.AppendFormat("/intermediateDir:{0} ", IntermediateDir);

			if (!string.IsNullOrEmpty(FilePath))
				sb.AppendFormat("/build:{0} ", FilePath);

			if (!string.IsNullOrEmpty(Importer))
				sb.AppendFormat("/Importer:{0} ", Importer);

			if (!string.IsNullOrEmpty(Processor))
				sb.AppendFormat("/Processor:{0} ", Processor);

			if (!string.IsNullOrEmpty(Platform))
				sb.AppendFormat("/Platform:{0} ", Platform);

			return sb.ToString();
		}
	}
}

