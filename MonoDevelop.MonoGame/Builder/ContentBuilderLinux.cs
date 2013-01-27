using System;
using System.Diagnostics;

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

		}

		public override bool RunBuilder ()
		{
			ProcessStartInfo info = new ProcessStartInfo();
			info.Arguments = System.IO.Path.Combine(Path, "tools", MGCB ) + " " + Arguments.ToArgs();
			info.FileName = "mono"; 
			Process p = Process.Start(info);		
			p.WaitForExit();
			return true;
		}
	}
}

