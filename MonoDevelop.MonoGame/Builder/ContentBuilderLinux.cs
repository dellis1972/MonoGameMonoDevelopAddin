using System;

namespace MonoDevelop.MonoGame
{
	public class ContentBuilderLinux : ContentBuilder
	{
		public override string MGCB {
			get {
				return "MGCB.exe";
			}
		}

		public ContentBuilderLinux (string path) : base(path)
		{
			throw new NotImplementedException ();
		}

		public override bool RunBuilder ()
		{
			throw new System.NotImplementedException ();
		}
	}
}

