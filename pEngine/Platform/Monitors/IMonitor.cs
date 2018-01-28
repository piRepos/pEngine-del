using System.Collections.Generic;

namespace pEngine.Platform.Monitors
{
    public interface IMonitor
    {
        /// <summary>
        /// Monitor identifier name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Monitor position.
        /// </summary>
        Vector2i Position { get; }

		/// <summary>
		/// Current monitor resolution.
		/// </summary>
		Resolution CurrentResolution { get; }

		/// <summary>
		/// List of all avaiable resolutions.
		/// </summary>
		IEnumerable<Resolution> SupportedResolutions { get; }
	}
}
