using System;

namespace MonoDevelop.MonoGame
{
	public abstract class ContentBuilder
	{
		public abstract string MGCB { get; }

		public abstract bool RunBuilder();

		public string Path { get; private set; }

		public abstract string SubDirectory { get; }

		public ContentBuilderArgs Arguments { get; set; }

		public string ErrorOutput { get; set; }

		public string Output { get; set; }

		public ContentBuilder (string path)
		{
			this.Arguments = new ContentBuilderArgs();
			this.Path = path;
		}
	}
}

