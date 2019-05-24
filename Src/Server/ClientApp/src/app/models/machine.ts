import { MachineCommand } from './machine-command';
import { MachineInitCommand } from './machine-init-command';

export class Machine
{
  id: number;
  description: string;
  comPort: string;
  sizeX: number;
  sizeY: number;
  sizeZ: number;
  baudRate: number;
  axis: number;

  sizeA: number;
  sizeB: number;
  sizeC: number;
  bufferSize: number;
  commandToUpper: boolean;
  probeSizeX: number;
  probeSizeY: number;
  probeSizeZ: number;
  probeDistUp: number;
  probeDist: number;
  probeFeed: number;
  SDSupport: boolean;
  spindle: boolean;
  coolant: boolean;
  laser: boolean;
  rotate: boolean;
	commandSyntax: number;

  commands: MachineCommand[];
  initCommands: MachineInitCommand[];
}
