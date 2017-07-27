using System;
using System.ComponentModel;

using PropertyChanged;

#pragma warning disable 0067

namespace pEngine.Common.DataModel
{
	[AddINotifyPropertyChangedInterface]
	public class NotifyPropertyChanged
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TextValidation.NotifyPropertyChanged"/> class.
		/// </summary>
		public NotifyPropertyChanged()
		{
			
		}

		#region Events

		/// <summary>
		/// Occurs when property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
