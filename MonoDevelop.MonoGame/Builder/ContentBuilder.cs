using System;

namespace MonoDevelop.MonoGame
{
	public abstract class ContentBuilder
	{
		public abstract string MGCB { get; }

		public abstract bool RunBuilder();

		protected string Path { get; private set; }

		public ContentBuilderArgs Arguments { get; set; }

		public ContentBuilder (string path)
		{
			this.Arguments = new ContentBuilderArgs();
			this.Path = path;
		}
	}
}

