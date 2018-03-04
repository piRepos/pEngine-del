using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Context
{
	public interface IPlatformWindow : IWindow, IContext, INotifyPropertyChanged
	{
	}
}
