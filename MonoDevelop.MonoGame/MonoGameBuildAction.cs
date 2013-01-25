using System;

namespace MonoDevelop.MonoGame
{
	public static class MonoGameBuildAction
	{
		public static readonly string MonoGameContent;
		public static readonly string[] knownTypes = { ".fx", ".png", ".spritefont", ".wav" };
		
		public static bool IsMonoGameBuildAction(string action)
		{
			return action == MonoGameContent;
		}
		
		static MonoGameBuildAction ()
		{
			MonoGameContent = "MonoGameContent";
		}
		
		public static bool IsKnownFileType (string extension)
		{
			foreach (string knowntype in knownTypes) {
				if (knowntype == extension) return true;
			}
			return false;
		}
	}
}

