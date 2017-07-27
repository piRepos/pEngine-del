using System;

namespace pEngine.Core.Data.Files
{
    public class TrueTypeFont : File
    {

		public TrueTypeFont(string path)
			: base(path)
		{
			switch (Extension.ToLower())
			{
				case "ttf":
				case "ttc":
					break;
				default:
					throw new InvalidOperationException("This file is not a ttf.");
			}
		}

		internal override void OnLoad()
		{
			base.OnLoad();
		}

		internal override void OnComplete()
		{
			base.OnComplete();
		}

		internal override bool OnAbort(IResource res, Exception e)
		{
			return base.OnAbort(res, e);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
