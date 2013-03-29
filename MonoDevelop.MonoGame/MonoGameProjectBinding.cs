using System;
using MonoDevelop.Projects;

namespace MonoDevelop.MonoGame
{
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


}

