using System;
using MonoDevelop.Projects;

namespace MonoDevelop.MonoGame
{
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
}

