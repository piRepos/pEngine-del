using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Graphics.Renderer.Shading
{
    public interface IUniform
    {

		/// <summary>
		/// Apply this uniform to a program.
		/// </summary>
		/// <param name="program">Shader program.</param>
		void Apply(uint program);

    }

}
