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

		public override string SubDirectory {
			get {
				return "tools/linux";
			}
		}

		public ContentBuilderLinux (string path) : base(path)
		{

		}

		public override bool RunBuilder ()
		{
			Output = String.Empty;
			ErrorOutput = String.Empty;
			Process p = new Process();
			p.StartInfo.Arguments = System.IO.Path.Combine(Path, SubDirectory, MGCB ) + " " + Arguments.ToArgs();
			p.StartInfo.WorkingDirectory = Arguments.WorkingDirectory;
			p.StartInfo.FileName = "mono";
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.CreateNoWindow = true;
			p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				this.ErrorOutput += e.Data;
			};	
			p.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
				this.Output += e.Data;
			};
			p.Start();
			p.BeginErrorReadLine();
			p.BeginOutputReadLine();
			p.WaitForExit();
			return p.ExitCode == 0;
		}
	}
}

