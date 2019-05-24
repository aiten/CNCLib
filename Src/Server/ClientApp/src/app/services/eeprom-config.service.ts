import { EepromConfigInput } from '../models/eeprom-config-input';
import { EepromConfig } from '../models/eeprom-config';

export abstract class EepromConfigService
{
  public abstract calculateConfig(config: EepromConfigInput): Promise<EepromConfig>;
}
