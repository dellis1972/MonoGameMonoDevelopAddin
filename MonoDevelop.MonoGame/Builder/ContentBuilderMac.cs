using System;

namespace MonoDevelop.MonoGame
{
	public class ContentBuilderMac : ContentBuilder
	{
		public override string MGCB {
			get {
				return "MGCB.exe";
			}
		}

		public ContentBuilderMac (string path) : base(path)
		{
			throw new NotImplementedException ();
		}

		public override bool RunBuilder ()
		{
			throw new System.NotImplementedException ();
		}
	}
}

