using System;
using System.Diagnostics;

namespace MonoDevelop.MonoGame
{
	public class ContentBuilderWindows : ContentBuilder
	{
		public override string MGCB {
			get {
				return "MGCB.exe";
			}
		}

		public ContentBuilderWindows (string path) : base(path)
		{

		}

		public override bool RunBuilder ()
		{
			ProcessStartInfo info = new ProcessStartInfo();
			info.Arguments = Arguments.ToArgs();			
			info.FileName = System.IO.Path.Combine(Path, "tools", MGCB );
			Process p = Process.Start(info);		
			p.WaitForExit();
			return true;
		}
	}
}

