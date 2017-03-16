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
	public class EepromAxis3 : EepromAxis2
	{
		const int AXIS = 2;

		#region Axis

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisZ { get { return _axis[AXIS]; } }

		#endregion

		#region Refmove-General

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 3")]
		[Description("Axis for reference-sequence 3")]
		public EReverenceSequence RefSeqence3 { get { return _refSeqences[AXIS]; } set { _refSeqences[AXIS] = value; } }

		#endregion
	}
}
