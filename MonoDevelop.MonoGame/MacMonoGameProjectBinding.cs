using System;
using MonoDevelop.Projects;

namespace MonoDevelop.MonoGame
{
	public class MacMonoGameProjectBinding : IProjectBinding
	{
		public Project CreateProject (ProjectCreateInformation info, System.Xml.XmlElement projectOptions)
		{ 
			string lang = projectOptions.GetAttribute ("language");
			return new MacMonoGameProject (lang, info, projectOptions);
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
			get { return "MonoGameMonoMac"; }
		}
	}
}

