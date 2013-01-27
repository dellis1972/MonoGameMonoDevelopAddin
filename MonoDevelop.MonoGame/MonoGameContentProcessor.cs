using System;
using MonoDevelop.Projects;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using MonoDevelop.Core;

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

		public static List<string> GetContentPipelineReferences (MonoGameProject project, BuildData buildData)
		{
			List<string> projectReferences = new List<string>();
			foreach (Project proj in project.ParentSolution.GetAllProjects()) {
				if (proj is MonoGameContentProject)
				{
					FilePath outputfile = proj.GetOutputFileName(buildData.Configuration.Selector);
					if (File.Exists(Path.GetFullPath(outputfile.FullPath)))
						projectReferences.Add( Path.GetFullPath(outputfile.FullPath));
				}
			}
			return projectReferences;
		}
		
		public static BuildResult Compile (ProjectFile file, MonoDevelop.Core.IProgressMonitor monitor, BuildData buildData, MonoGameProject project)
		{
			GetContentPipelineReferences(project, buildData);
			// its not a file we know about so dont do anything
			switch (file.BuildAction) {
			case "MonoGameContent" :
				var result = new BuildResult();
				monitor.Log.WriteLine("Compiling Content");				
				monitor.Log.WriteLine("Compiling For : "+ project.MonoGamePlatform);
				monitor.Log.WriteLine("File : "+System.IO.Path.GetFileName(file.FilePath));
				monitor.Log.WriteLine("From : "+file.FilePath);
				monitor.Log.WriteLine("To : "+ System.IO.Path.Combine( buildData.Configuration.OutputDirectory, GetXnbFileName(file.FilePath)));
				monitor.Log.WriteLine("Name : "+file.ExtendedProperties["Name"]);
				monitor.Log.WriteLine("Importer : "+file.ExtendedProperties["Importer"]);
				monitor.Log.WriteLine("Processor : "+file.ExtendedProperties["Processor"]);
				builder.Arguments.FilePath = file.FilePath;
				builder.Arguments.Platform = project.MonoGamePlatform;
				builder.Arguments.OutputDir = buildData.Configuration.OutputDirectory;
				builder.Arguments.Importer = (string)file.ExtendedProperties["Importer"];
				builder.Arguments.Processor = (string)file.ExtendedProperties["Processor"];
				// append any assembly references for content projects
				builder.Arguments.References.AddRange(GetContentPipelineReferences(project, buildData));
				builder.RunBuilder();
				return result;
			default:
				return new BuildResult();
			}
			
		}
	}
}

