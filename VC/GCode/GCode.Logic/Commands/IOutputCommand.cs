using Framework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCode.Logic.Commands
{
	public interface IOutputCommand
	{
		void DrawLine(Command cmd, object param, Command.MoveType movetype, SpaceCoordinate ptFrom, SpaceCoordinate ptTo);
		void DrawEllipse(Command cmd, object param, Command.MoveType movetype, SpaceCoordinate ptFrom, int xradius, int yradius);
	}
}
