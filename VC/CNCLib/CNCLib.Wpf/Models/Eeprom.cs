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
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace CNCLib.Wpf.Models
{
	public class Eeprom
	{
		const string CATEGORY_INTERNAL = "Internal";
		const string CATEGORY_SIZE = "Size";
		const string CATEGORY_FEATURES = "Features";
		const string CATEGORY_PROBE = "Probe";
		const string CATEGORY_GENERAL = "General";

		const string CATEGORY_AXIS_X = "Axis X";
		const string CATEGORY_AXIS_Y = "Axis Y";
		const string CATEGORY_AXIS_Z = "Axis Z";
		const string CATEGORY_AXIS_A = "Axis A";

		const int EEPROM_NUM_AXIS = 4;

		public enum EConfigSlot
		{
			Signature = 0,
			MaxSize = 1,
			ScaleMmToMachine = MaxSize + EEPROM_NUM_AXIS,
			ReferenceType = ScaleMmToMachine + 1,
			RefMove,

			MaxStepRate,
			Acc,
			Dec,
			RefMoveStepRate
		};

		public class ConfigSlotAttribut : Attribute
		{
			public EConfigSlot SlotId { get; private set; }

			public ConfigSlotAttribut(EConfigSlot slotid)
			{
				SlotId = slotid;
			}
		}

		[Category(CATEGORY_GENERAL)]
		[DisplayName("MaxStepRate")]
		[Description("Maximum steprate in Hz")]
		[ConfigSlotAttribut(EConfigSlot.MaxStepRate)]
		public uint MaxStepRate { get; set; } = 27000;

		[Category(CATEGORY_AXIS_X)]
		[DisplayName("Size-X")]
		[Description("Maximum size X in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize)]
		public uint SizeX { get; set; } = 200000;

		[Category(CATEGORY_AXIS_Y)]
		[DisplayName("Size-Y")]
		[Description("Maximum size of Y in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize + 1)]
		public uint SizeY { get; set; } = 200000;

		[Category(CATEGORY_AXIS_Z)]
		[DisplayName("Size-Z")]
		[Description("Maximum size of Z in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize + 2)]
		//This custom editor is a Class that implements the ITypeEditor interface
		public uint SizeZ { get; set; } = 200000;

		[Category(CATEGORY_AXIS_A)]
		[DisplayName("Size-A")]
		[Description("Maximum size of A in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize + 3)]
		//This custom editor is a Class that implements the ITypeEditor interface
		public uint SizeA { get; set; } = 200000;

		[Category(CATEGORY_GENERAL)]
		[DisplayName("Scale mm to machine")]
		[Description("scale mm to machine")]
		[ConfigSlotAttribut(EConfigSlot.ScaleMmToMachine)]
		//This custom editor is a Class that implements the ITypeEditor interface
		public float ScaleMMToMachine { get; set; } = 3200.0f;
	}
}
