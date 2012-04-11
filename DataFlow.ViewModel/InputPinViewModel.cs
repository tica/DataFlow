using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataFlow.ViewModel
{
	public class InputPinViewModel : PinViewModel
	{
		public InputPinViewModel(FilterViewModel parent, Model.Pin pin)
			: base(parent, pin)
		{
		}

		public override Point ConnectionPoint
		{
			get
			{
				return new Point(_rect.Left + Parent.Left, _rect.Top + _rect.Height / 2 + Parent.Top);
			}
		}		
	}
}
