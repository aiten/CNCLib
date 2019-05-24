import { Component, OnInit, Input } from '@angular/core';
import { EepromConfig } from '../../models/eeprom-config';

@Component({
  selector: 'ha-eeprom-config-result',
  templateUrl: './eeprom-config-result.component.html',
  styleUrls: ['./eeprom-config-result.component.css']
})
export class EepromConfigResultComponent implements OnInit
{
  @Input() entry: EepromConfig;

  ngOnInit()
  {
  }
}
