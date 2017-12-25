using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace pEngine.Resources.Files
{
    public class File : Resource
    {
		/// <summary>
		/// Makes a new <see cref="File"/> from a path.
		/// </summary>
		/// <param name="path">File path.</param>
		public File(string path)
			: base()
		{
			Path = path;
		}

		/// <summary>
		/// Makes a new <see cref="File"/>.
		/// </summary>
		public File()
			: base()
		{
			
		}

		/// <summary>
		/// Makes a new <see cref="File"/> from a byte buffer.
		/// This file doesn't needs a load call.
		/// </summary>
		public File(string name, byte[] content)
			: base()
		{
			Path = "local://" + name;
			Content = content;

			IsLoaded = true;
		}

		/// <summary>
		/// File path.
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// File name without extension.
		/// </summary>
		public string Name => System.IO.Path.GetFileName(Path).Split('.')[0];

		/// <summary>
		/// File extension without dot.
		/// </summary>
		public string Extension => System.IO.Path.GetExtension(Path).Substring(1);

		/// <summary>
		/// File size.
		/// </summary>
		public override uint UsedSpace => (uint)Content.Length * sizeof(byte);

		/// <summary>
		/// File size.
		/// </summary>
		public uint Size => (uint)Content.Length * sizeof(byte);

		/// <summary>
		/// File raw content.
		/// </summary>
		public byte[] Content { get; private set; }

		internal override void OnLoad()
		{
			Content = System.IO.File.ReadAllBytes(Path);
		}
	}

	/// <summary>
	/// File content type.
	/// </summary>
	public enum FileType
	{
		Text = 0x01,
		Binary = 0x00
	}
}
