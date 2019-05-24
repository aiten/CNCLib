import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineEntryLoadComponent } from './machine-entry-load.component';

describe('MachineEntryLoadComponent', () => {
  let component: MachineEntryLoadComponent;
  let fixture: ComponentFixture<MachineEntryLoadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineEntryLoadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineEntryLoadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
