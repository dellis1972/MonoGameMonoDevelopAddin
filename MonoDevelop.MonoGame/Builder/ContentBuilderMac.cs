using System;

namespace MonoDevelop.MonoGame
{
	public class ContentBuilderMac : ContentBuilderLinux
	{
		public override string MGCB {
			get {
				return "MGCB.exe";
			}
		}

		public ContentBuilderMac (string path) : base(path)
		{

		}

	}
}

