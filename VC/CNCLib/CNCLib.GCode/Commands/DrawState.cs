namespace CNCLib.GCode.Commands
{
	public class DrawState
    {
        public bool UseLaser { get; set; } = false;
        public bool LaserOn { get; set; } = false;
        public bool SpindleOn { get; set; } = false;
        public bool CoolantOn { get; set; } = false;
		public Pane CurrentPane { get; set; } = Pane.XYPane;
	}
}
