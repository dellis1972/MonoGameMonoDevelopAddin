using System;
using MonoDevelop.Projects;
using MonoDevelop.MonoMac;
using System.Xml;
using MonoDevelop.Core.Serialization;

namespace MonoDevelop.MonoGame
{
	public class MacMonoGameProject : MonoDevelop.MonoMac.XamMacProjectBase, IMonoGameProject
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

		public MacMonoGameProject (string languageName, 
		                           ProjectCreateInformation info, 
		                           XmlElement projectOptions) 
			: base (languageName, info, projectOptions)
		{
			var monoGamePlatformAttrib = projectOptions.Attributes ["MonoGamePlatform"];
			if (monoGamePlatformAttrib != null)
				monoGamePlatform = monoGamePlatformAttrib.Value;
		}
		
		public MacMonoGameProject (string languageName) 
			: base (languageName)
		{
		}
		
		public MacMonoGameProject ()
		{
		}

		protected override System.Collections.Generic.IList<string> GetCommonBuildActions ()
		{			
			var actions = new System.Collections.Generic.List<string>(base.GetCommonBuildActions());
			actions.Add(MonoGameBuildAction.MonoGameContent);
			return actions;
		}	
		
		public override string GetDefaultBuildAction (string fileName)
		{
			if (MonoGameBuildAction.IsKnownFileType(System.IO.Path.GetExtension(fileName)))
			{
				return MonoGameBuildAction.MonoGameContent;
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
								var cfg = (DotNetProjectConfiguration)solutionConfiguration.GetConfiguration(this);
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
	
		public bool IsProcessorNameValid (string str)
		{
			return true;
		}
		
		public System.Collections.ICollection GetProcessorNames ()
		{
			return new string[] {"TextureProcessor", "SpriteFontProcessor", "SoundEffectProcessor", "SongProcessor"};
		}
		
		public bool IsImporterNameValid (string str)
		{
			return true;
		}
		
		public System.Collections.ICollection GetImporterNames ()
		{
			return new string[] {"TextureImporter", "FontDescriptionImporter", "WavImporter", "Mp3Importer"};
		}
	}
	

}

