using System;
using MonoDevelop.Projects;
using System.Xml;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.Core.Serialization;

namespace MonoDevelop.MonoGame
{	
	public static class MonoGameBuildAction
	{
		public static readonly string Shader;
		
		public static bool IsMonoGameBuildAction(string action)
		{
			return action == Shader;
		}
		
		static MonoGameBuildAction ()
		{
			Shader = "MonoGameShader";
		}
	}
	
	public class MonoGameProject :  DotNetAssemblyProject
	{
		[ItemProperty ("MonoGamePlatform")]
	    string monoGamePlatform;

		public string MonoGamePlatform {
			get { return monoGamePlatform;}
			set {
				if (monoGamePlatform != value) {
					monoGamePlatform = value;
					NotifyModified("MonoGamePlatform");
				}
			}
		}

		public MonoGameProject ()
		{
			Init ();
		}
		
		public MonoGameProject (string languageName)
			: base (languageName)
		{
			Init ();
		}
		
		public MonoGameProject (string languageName, ProjectCreateInformation info, XmlElement projectOptions)
			: base (languageName, info, projectOptions)
		{
			Init ();

			var monoGamePlatformAttrib = projectOptions.Attributes ["MonoGamePlatform"];
			if (monoGamePlatformAttrib != null)
				monoGamePlatform = monoGamePlatformAttrib.Value;
		}
		
		private void Init()
		{
		}
		
		public override SolutionItemConfiguration CreateConfiguration (string name)
		{
			var conf = new MonoGameProjectConfiguration (name);
			conf.CopyFrom (base.CreateConfiguration (name));
			return conf;
		}
			
		public override bool SupportsFormat (FileFormat format)
		{
			return format.Id == "MSBuild10";
		}
		
		public override TargetFrameworkMoniker GetDefaultTargetFrameworkForFormat (FileFormat format)
		{
			return new TargetFrameworkMoniker("4.0");
		}
		
		public override bool SupportsFramework (MonoDevelop.Core.Assemblies.TargetFramework framework)
		{
			if (!framework.IsCompatibleWithFramework (MonoDevelop.Core.Assemblies.TargetFrameworkMoniker.NET_4_0))
				return false;
			else
				return base.SupportsFramework (framework);
		}
		
		protected override System.Collections.Generic.IList<string> GetCommonBuildActions ()
		{			
			var actions = new System.Collections.Generic.List<string>(base.GetCommonBuildActions());
			actions.Add(MonoGameBuildAction.Shader);
			return actions;
		}
		
		public override string GetDefaultBuildAction (string fileName)
		{
			if (System.IO.Path.GetExtension(fileName) == ".fx")
			{
				return MonoGameBuildAction.Shader;
			}
			return base.GetDefaultBuildAction (fileName);
		}        

        protected override void PopulateSupportFileList (FileCopySet list, ConfigurationSelector solutionConfiguration)
		{
			System.Diagnostics.Debug.WriteLine("MonoGamePlatform=" +this.MonoGamePlatform);
			base.PopulateSupportFileList (list, solutionConfiguration);
			//HACK: workaround for MD not local-copying package references
            foreach (var projectReference in References)
            {
				if (projectReference.Reference.Contains("MonoGame.Framework"))
				{
					// because of a weird bug in the way monodevelop resolves the assemblies
					// we do it manually. We combine the monogame-<MonoGamePlatform> 
					// to resolve the internal packages.
					foreach(var p in this.AssemblyContext.GetPackages(this.TargetFramework))
					{
						if (p.Name == string.Format("monogame-{0}", this.MonoGamePlatform.ToLower()))
						{
							foreach(var assem in p.Assemblies) {
								list.Add(assem.Location);
								var cfg = (MonoGameProjectConfiguration)solutionConfiguration.GetConfiguration(this);
								if (cfg.DebugMode)
								{
									var mdbFile = TargetRuntime.GetAssemblyDebugInfoFile(assem.Location);
									if (System.IO.File.Exists(mdbFile))
										list.Add(mdbFile);
								}   
								var assemDir = System.IO.Path.GetDirectoryName (assem.Location);
								// opentk needs a config file on linux and mac, so we just copy it over anyway
								if (assem.Location.ToLower().Contains("opentk")) {
									if (System.IO.File.Exists(System.IO.Path.Combine(assemDir, "OpenTK.dll.config"))) {
										list.Add(System.IO.Path.Combine(assemDir, "OpenTK.dll.config"));
									}
								}
								// we are a Tao.SDL project we need the sdl support libraries as well
								if (assem.Location.ToLower().Contains("tao.sdl")) {
									if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
										if (System.IO.File.Exists(System.IO.Path.Combine(assemDir, "sdl.dll"))) {
											list.Add(System.IO.Path.Combine(assemDir, "sdl.dll"));
										}
										if (System.IO.File.Exists(System.IO.Path.Combine(assemDir, "sdl_mixer.dll"))) {
											list.Add(System.IO.Path.Combine(assemDir, "sdl_mixer.dll"));
										}
									}
									if (System.IO.File.Exists(System.IO.Path.Combine(assemDir, "Tao.Sdl.dll.config"))) {
										list.Add(System.IO.Path.Combine(assemDir, "Tao.Sdl.dll.config"));
									}
								}
								
							}
						}
					}                
                    break;
                }
            }
        }
				
	}
	
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
				   return base.Compile (monitor, item, buildData);
				}
				monitor.Log.WriteLine("Compiling for {0}", proj.MonoGamePlatform);
				var results = new System.Collections.Generic.List<BuildResult>();
				foreach(var file in proj.Files)
				{
					if (MonoGameBuildAction.IsMonoGameBuildAction(file.BuildAction))					
					{												
						buildData.Items.Add(file);
						var buildResult = MonoGameContentProcessor.Compile(file, monitor, buildData);
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
	
	public class MonoGameProjectBinding : IProjectBinding
	{
		public Project CreateProject (ProjectCreateInformation info, System.Xml.XmlElement projectOptions)
		{ 
			string lang = projectOptions.GetAttribute ("language");
			return new MonoGameProject (lang, info, projectOptions);
		}
	
		public Project CreateSingleFileProject (string sourceFile)
		{
			throw new InvalidOperationException ();
		}
		
		public bool CanCreateSingleFileProject (string sourceFile)
		{
			return false;
		}
		
		public string Name {
			get { return "MonoGame"; }
		}
	}
	
	public class MonoGameProjectConfiguration : DotNetProjectConfiguration
	{
		public MonoGameProjectConfiguration () : base ()
		{
		}
		
		public MonoGameProjectConfiguration (string name) : base (name)
		{
		}		
		
		public override void CopyFrom (ItemConfiguration configuration)
		{
			base.CopyFrom (configuration);
		}
	}
	
	public class MonoGameContentProcessor 
	{		
		
		public static BuildResult Compile(ProjectFile file,MonoDevelop.Core.IProgressMonitor monitor,BuildData buildData)
		{			
			switch (file.BuildAction) {
			case "MonoGameShader" :
				var result = new BuildResult();
				monitor.Log.WriteLine("Compiling Shader");					
				monitor.Log.WriteLine("Shader : "+buildData.Configuration.OutputDirectory);
				monitor.Log.WriteLine("Shader : "+file.FilePath);
				monitor.Log.WriteLine("Shader : "+file.ToString());
				return result;
			default:
				return new BuildResult();
			}
			
		}
	}
}

