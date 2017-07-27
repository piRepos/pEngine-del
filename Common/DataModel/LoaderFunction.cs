using System;


namespace pEngine.Common.DataModel
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class LoaderFunction : Attribute
	{
		public LoaderFunction()
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LoaderFunction"/> permit null services.
		/// </summary>
		public bool PermitNullServices { get; set; }
	}
}
