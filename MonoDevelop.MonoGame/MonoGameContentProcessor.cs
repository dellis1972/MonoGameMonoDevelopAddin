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
			addInPath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetAssembly(typeof(MonoGameContentProcessor)).Location));		
			if (Environment.OSVersion.Platform == PlatformID.MacOSX) {
				builder = new ContentBuilderMac(addInPath);
			} else if (Environment.OSVersion.Platform == PlatformID.Unix) {
				// Well, there are chances MacOSX is reported as Unix instead of MacOSX.
				// Instead of platform check, we'll do a feature checks (Mac specific root folders)
				if (Directory.Exists("/Applications")
				    & Directory.Exists("/System")
				    & Directory.Exists("/Users")
				    & Directory.Exists("/Volumes"))
					builder = new ContentBuilderMac(addInPath);
				else
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
				builder.Arguments.WorkingDirectory = Path.GetFullPath(Path.GetDirectoryName(project.FileName));
				builder.Arguments.Platform = project.MonoGamePlatform;
				builder.Arguments.OutputDir = buildData.Configuration.OutputDirectory;
				builder.Arguments.Importer = (string)file.ExtendedProperties["Importer"];
				builder.Arguments.Processor = (string)file.ExtendedProperties["Processor"];
				// append any assembly references for content projects
				builder.Arguments.References.AddRange(GetContentPipelineReferences(project, buildData));
				monitor.Log.WriteLine("WorkingDirectory : "+builder.Arguments.WorkingDirectory);
				monitor.Log.WriteLine("WorkingDirectory : "+Path.Combine(builder.Path, builder.SubDirectory, builder.MGCB ));
				monitor.Log.WriteLine("Args : "+builder.Arguments.ToArgs());
				try {
				   if (!builder.RunBuilder()) {
						result.AddError(builder.Output);
						result.AddError(builder.ErrorOutput);
				   }
				}
				catch (Exception ex) {
					result.AddError(ex.ToString());
				}
				return result;
			default:
				return new BuildResult();
			}
			
		}
	}
}

