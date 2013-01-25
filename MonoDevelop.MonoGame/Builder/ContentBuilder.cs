using System;

namespace MonoDevelop.MonoGame
{
	public abstract class ContentBuilder
	{
		public abstract string MGCB { get; }

		public abstract bool RunBuilder();

		protected string Path { get; private set; }

		public ContentBuilder (string path)
		{
			this.Path = path;
		}
	}
}

