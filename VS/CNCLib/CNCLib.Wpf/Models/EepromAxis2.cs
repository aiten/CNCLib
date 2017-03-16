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
	public class EepromAxis2 : Eeprom
	{
		#region Axis

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisX { get { return _axis[0]; } }

		[ExpandableObject]
		[Category("Axis")]
		[Description("Definition of axis")]
		public SAxis AxisY { get { return _axis[1]; } }

		#endregion

		#region Refmove-General

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 1")]
		[Description("Axis for reference-sequence 1")]
		public EReverenceSequence RefSeqence1 { get { return _refSeqences[0]; } set { _refSeqences[0] = value; } }

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Ref-Sequence 2")]
		[Description("Axis for reference-sequence 2")]
		public EReverenceSequence RefSeqence2 { get { return _refSeqences[1]; } set { _refSeqences[1] = value; } }

		#endregion
	}
}
