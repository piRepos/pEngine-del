// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Diagnostics;
using System.Threading;

using pEngine.Common.Threading;

using pEngine.Core.Data;
using pEngine.Core.Data.Files;

namespace pEngine.Core.Network.Base
{

	public class FileTransfer : Common.Threading.Progress<long>
	{

		/// <summary>
		/// Makes a new instance of <see cref="FileTransfer"/> class.
		/// </summary>
		public FileTransfer(File fileToTransfer)
		{
			CurrentFile = fileToTransfer;
			timer = new Stopwatch();
		}

		/// <summary>
		/// Makes a new instance of <see cref="FileTransfer"/> class.
		/// </summary>
		/// <param name="handler">Handler to a function to execute on each update.</param>
		public FileTransfer(File fileToTransfer, Action<long> handler)
			: base(handler)
		{
			CurrentFile = fileToTransfer;
			timer = new Stopwatch();
		}
		
		/// <summary>
		/// Reports a progress change.
		/// </summary>
		/// <param name="value">The value of the updated progress. (-1 if complete)</param>
		public override void Report(long value)
		{
			lock (this)
			{
				Loaded += value;

				if (!timer.IsRunning)
					timer.Start();

				downloadedLastSec += value;
				if (avgTime + 1 >= ElapsedTime.Seconds)
				{
					downloadedLastSec = 0;
					avgTime = ElapsedTime.Seconds;
				}

				if (value < 0)
				{
					Loaded = CurrentFile.Size;
					downloadedLastSec = 0;
					TransferComplete?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Abord this transfer session.
		/// </summary>
		public void Abort()
		{
			lock (this)
			{
				IsAborted = true;
			}
		}

		private Stopwatch timer;
		private double avgTime;
		private long downloadedLastSec;

		/// <summary>
		/// Triggered on file transfer completed.
		/// </summary>
		public event EventHandler TransferComplete;

		/// <summary>
		/// Percentile processed.
		/// </summary>
		public double Percentile => (Loaded / (double)Size) * 100;

		/// <summary>
		/// Total file size.
		/// </summary>
		public long Size => CurrentFile.Size;

		/// <summary>
		/// Loaded bytes.
		/// </summary>
		public long Loaded { get; private set; }

		/// <summary>
		/// True when the transfer is in abort request or aborted.
		/// </summary>
		public bool IsAborted { get; private set; }

		/// <summary>
		/// Elapsed time from transfer start.
		/// </summary>
		public TimeSpan ElapsedTime => timer.Elapsed;

		/// <summary>
		/// Download speed in bytes per second.
		/// </summary>
		public long Speed => downloadedLastSec;

		/// <summary>
		/// Target file.
		/// </summary>
		public File CurrentFile { get; }

		/// <summary>
		/// Remaining bytes.
		/// </summary>
		public long Remains => Size - Loaded;
	}
}
