using System;
using MonoDevelop.Projects;
using MonoDevelop.Core;

namespace MonoDevelop.MonoGame
{
	/// <summary>
	/// This interfaces defines what a MonoGameProject should expose. This is because
	/// not all platforms can derive directly from MonoGameProject. For Mac, iOS
	/// and Android a new class will need to be written and it will need to support
	/// this interface in order to have content pipeline support.
	/// </summary>
	public interface IMonoGameProject
	{
		FilePath FileName {
			get;
		}

		bool IsProcessorNameValid (string proessor);

		System.Collections.ICollection GetProcessorNames ();

		bool IsImporterNameValid (string importer);

		System.Collections.ICollection GetImporterNames ();

		Solution ParentSolution {
			get;
		}

		string MonoGamePlatform {
			get;
			set;
		}

		ProjectFileCollection Files {
			get;
		}
	}
}

