using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace CNCLib.Wpf.Models
{
	public class EepromAxis4 : EepromAxis3
	{
		const int AXIS = 3;

		#region Axis

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisA { get { return _axis[AXIS]; } }

		#endregion

		#region Refmove-General

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 4")]
		[Description("Axis for reference-sequence 4")]
		public EReverenceSequence RefSeqence4 { get { return _refSeqences[AXIS]; } set { _refSeqences[AXIS] = value; } }

		#endregion
	}
}
