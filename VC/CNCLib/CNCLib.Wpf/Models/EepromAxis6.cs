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
	public class EepromAxis6 : EepromAxis4
	{
		#region Axis

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisB { get { return _axis[4]; } }
		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisC { get { return _axis[5]; } }

		#endregion

		#region Refmove-General

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 5")]
		[Description("Axis for reference-sequence 5")]
		public EReverenceSequence RefSeqence5 { get { return _refSeqences[4]; } set { _refSeqences[4] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 6")]
		[Description("Axis for reference-sequence 6")]
		public EReverenceSequence RefSeqence6 { get { return _refSeqences[5]; } set { _refSeqences[5] = value; } }

		#endregion
	}
}
