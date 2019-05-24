import { Component, OnInit, Input } from '@angular/core';
import { Machine } from '../../models/machine';

@Component({
  selector: 'ha-machine-entry',
  templateUrl: './machine-entry.component.html',
  styleUrls: ['./machine-entry.component.css']
})
export class MachineEntryComponent implements OnInit
{
  @Input() entry: Machine;
  _isMore: boolean = false;

  ngOnInit()
  {
  }
}
