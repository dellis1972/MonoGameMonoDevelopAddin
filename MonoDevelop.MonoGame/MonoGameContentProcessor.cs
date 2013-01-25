using System;
using MonoDevelop.Projects;
using System.Reflection;

namespace MonoDevelop.MonoGame
{
	public class MonoGameContentProcessor 
	{	
		static string addInPath;
		static ContentBuilder builder;
		
		static MonoGameContentProcessor ()
		{
			addInPath = System.IO.Path.GetFullPath(Assembly.GetAssembly(typeof(MonoGameContentProcessor)).Location);		
			if (Environment.OSVersion.Platform == PlatformID.MacOSX) {
				builder = new ContentBuilderMac(addInPath);
			} else if (Environment.OSVersion.Platform == PlatformID.Unix) {
				builder = new ContentBuilderLinux(addInPath);
			} else {
				builder = new ContentBuilderWindows(addInPath);
			}
		}
		
		private static string GetXnbFileName (string filename)
		{
			return System.IO.Path.GetFileNameWithoutExtension(filename) + ".xnb";
		}
		
		public static BuildResult Compile (ProjectFile file, MonoDevelop.Core.IProgressMonitor monitor, BuildData buildData, string monoGamePlatform)
		{			
			// its not a file we know about so dont do anything
			switch (file.BuildAction) {
			case "MonoGameContent" :
				var result = new BuildResult();
				monitor.Log.WriteLine("Compiling Content");				
				monitor.Log.WriteLine("Compiling For : "+ monoGamePlatform);
				monitor.Log.WriteLine("File : "+System.IO.Path.GetFileName(file.FilePath));
				monitor.Log.WriteLine("From : "+file.FilePath);
				monitor.Log.WriteLine("To : "+ System.IO.Path.Combine( buildData.Configuration.OutputDirectory, GetXnbFileName(file.FilePath)));
				monitor.Log.WriteLine("Name : "+file.ExtendedProperties["Name"]);
				monitor.Log.WriteLine("Importer : "+file.ExtendedProperties["Importer"]);
				monitor.Log.WriteLine("Processor : "+file.ExtendedProperties["Processor"]);
				return result;
			default:
				return new BuildResult();
			}
			
		}
	}
}

