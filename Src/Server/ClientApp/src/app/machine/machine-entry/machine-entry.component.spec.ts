import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineEntryComponent } from './machine-entry.component';

describe('MachineEntryComponent',
  () => {
    let component: MachineEntryComponent;
    let fixture: ComponentFixture<MachineEntryComponent>;

    beforeEach(async(() => {
      TestBed.configureTestingModule({
          declarations: [MachineEntryComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(MachineEntryComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it('should create',
      () => {
        expect(component).toBeTruthy();
      });
  });
