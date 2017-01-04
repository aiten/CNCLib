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

		[Category("General")]
		[DisplayName("MaxStepRate")]
		[Description("Maximum steprate in Hz")]
		[ConfigSlotAttribut(EConfigSlot.MaxStepRate)]
		public uint MaxStepRate { get; set; } = 27000;

		[Category("General")]
		[DisplayName("MaxSizeX")]
		[Description("Maximum size X in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize)]
		public uint MaxSizeX { get; set; } = 200000;

		[Category("General")]
		[DisplayName("MaxSizeY")]
		[Description("Maximum size of Y in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize+1)]
		public uint MaxSizeY { get; set; } = 200000;

		[Category("General")]
		[DisplayName("MaxSizeY")]
		[Description("Maximum size of Z in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize + 2)]
		//This custom editor is a Class that implements the ITypeEditor interface
		public uint MaxSizeZ { get; set; } = 200000;

		[Category("General")]
		[DisplayName("MaxSizeY")]
		[Description("Maximum size of A in mm1000")]
		[ConfigSlotAttribut(EConfigSlot.MaxSize + 3)]
		//This custom editor is a Class that implements the ITypeEditor interface
		public uint MaxSizeA { get; set; } = 200000;

		[Category("General")]
		[DisplayName("Scale mm to machine")]
		[Description("scalt mm to machin")]
		[ConfigSlotAttribut(EConfigSlot.ScaleMmToMachine)]
		//This custom editor is a Class that implements the ITypeEditor interface
		public float ScaleMMToMachine { get; set; } = 3200.0f;

		/*
				[Category("Information")]
				[DisplayName("First Name")]
				[Description("This property uses a TextBox as the default editor.")]
				//This custom editor is a Class that implements the ITypeEditor interface
				[Editor(typeof(FirstNameEditor), typeof(FirstNameEditor))]
				public string FirstName { get; set; }

		 * */
		/*
			   [Category("Information")]
			   [DisplayName("Last Name")]
			   [Description("This property uses a TextBox as the default editor.")]
			   //This custom editor is a UserControl that implements the ITypeEditor interface
			   [Editor(typeof(LastNameUserControlEditor), typeof(LastNameUserControlEditor))]
			   public string LastName { get; set; }
	   */
	}
	/*

		//Custom editors that are used as attributes MUST implement the ITypeEditor interface.
		public class FirstNameEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
		{
			public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
			{
				TextBox textBox = new TextBox();
				textBox.Background = new SolidColorBrush(Colors.Red);

				//create the binding from the bound property item to the editor
				var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
				_binding.Source = propertyItem;
				_binding.ValidatesOnExceptions = true;
				_binding.ValidatesOnDataErrors = true;
				_binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
				BindingOperations.SetBinding(textBox, TextBox.TextProperty, _binding);
				return textBox;
			}
		}
	*/
}
