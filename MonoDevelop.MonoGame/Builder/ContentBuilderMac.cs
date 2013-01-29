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

		public override string SubDirectory {
			get {
				return "tools/macos";
			}
		}

		public ContentBuilderMac (string path) : base(path)
		{

		}

	}
}

