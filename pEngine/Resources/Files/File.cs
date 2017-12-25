using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using pEngine.Framework.Binding;

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

			State =  ResourceState.Loaded;
		}

		/// <summary>
		/// File path.
		/// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
		public string Path { get; }

		/// <summary>
		/// File name without extension.
		/// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
		public string Name => System.IO.Path.GetFileName(Path).Split('.')[0];

		/// <summary>
		/// File extension without dot.
		/// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
		public string Extension => System.IO.Path.GetExtension(Path).Substring(1);

		/// <summary>
		/// File size.
		/// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
        public override uint UsedSpace => base.UsedSpace + ((uint)Content.Length * sizeof(byte)) + ((uint)Path.Length * sizeof(char));

        /// <summary>
        /// Used space in bytes.
        /// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
		public uint Size => (uint)Content.Length * sizeof(byte);

		/// <summary>
		/// File raw content.
		/// </summary>
		public byte[] Content { get; private set; }

		protected override void OnLoad()
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
