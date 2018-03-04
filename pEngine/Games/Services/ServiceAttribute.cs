using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace pEngine.Games
{
	/// <summary>
	/// Inject a service in the game object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ServiceAttribute : Attribute
	{
		/// <summary>
		/// Makes a new instance of <see cref="ServiceAttribute"/> class.
		/// </summary>
		/// <param name="service">Service type reference.</param>
		public ServiceAttribute()
		{
		}
	}
}
