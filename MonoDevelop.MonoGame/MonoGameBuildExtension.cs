using System;
using MonoDevelop.Projects;

namespace MonoDevelop.MonoGame
{
	public class MonoGameBuildExtension : ProjectServiceExtension
	{
		
		protected override BuildResult Build (MonoDevelop.Core.IProgressMonitor monitor, SolutionEntityItem item, ConfigurationSelector configuration)
		{
#if DEBUG			
			monitor.Log.WriteLine("MonoGame Extension Build Called");	
#endif			
			try
			{
				return base.Build (monitor, item, configuration);
			}
			finally
			{
#if DEBUG				
				monitor.Log.WriteLine("MonoGame Extension Build Ended");	
#endif				
			}
		}
		
		protected override BuildResult Compile (MonoDevelop.Core.IProgressMonitor monitor, SolutionEntityItem item, BuildData buildData)
		{
#if DEBUG			
			monitor.Log.WriteLine("MonoGame Extension Compile Called");	
#endif			
			try
			{				
				var proj = item as MonoGameProject;
				if (proj == null)
				{
					monitor.Log.WriteLine("Compiling for Unknown MonoGame Project");
					return base.Compile (monitor, item, buildData);
				}
				monitor.Log.WriteLine("Detected {0} MonoGame Platform", proj.MonoGamePlatform);
				var results = new System.Collections.Generic.List<BuildResult>();
				foreach(var file in proj.Files)
				{
					if (MonoGameBuildAction.IsMonoGameBuildAction(file.BuildAction))					
					{												
						buildData.Items.Add(file);
						var buildResult = MonoGameContentProcessor.Compile(file, monitor, buildData, proj);
						results.Add(buildResult);
					}
				}
				return base.Compile (monitor, item, buildData).Append(results);
			}
			finally
			{
#if DEBUG				
				monitor.Log.WriteLine("MonoGame Extension Compile Ended");	
#endif				
			}
		}
	}
}

