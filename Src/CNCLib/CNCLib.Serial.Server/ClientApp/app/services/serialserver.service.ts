import { SerialCommand } from "../models/serial.command";
import { SerialPortDefinition } from '../models/serial.port.definition';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info'
import { Observable } from 'rxjs/Observable';

export abstract class SerialServerService
{
    public abstract getInfo(): Promise<CNCLibServerInfo>;

    public abstract getPorts(): Promise<SerialPortDefinition[]>;
    public abstract getPort(id: number): Promise<SerialPortDefinition>;

    public abstract refresh(): Promise<SerialPortDefinition[]>;

    public abstract connect(serialportid: number, baudrate: number, resetonConnect: boolean): Promise<void>;
    public abstract disconnect(serialportid: number): Promise<void>;

    public abstract abort(serialportid: number): Promise<void>;
    public abstract resume(serialportid: number): Promise<void>;

    public abstract getHistory(serialportid: number): Promise<SerialCommand[]>;
    public abstract clearHistory(serialportid: number): Promise<void>;

    public abstract queueCommands(serialportid: number, command: string[], timeout: number): Promise<SerialCommand[]>;
}
