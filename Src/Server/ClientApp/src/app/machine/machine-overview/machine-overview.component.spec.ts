import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineOverviewComponent } from './machine-overview.component';

describe('MachineOverviewComponent', () =>
{
  let component: MachineOverviewComponent;
  let fixture: ComponentFixture<MachineOverviewComponent>;

  beforeEach(async(() =>
  {
    TestBed.configureTestingModule({
      declarations: [MachineOverviewComponent]
    })
      .compileComponents();
  }));

  beforeEach(() =>
  {
    fixture = TestBed.createComponent(MachineOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () =>
  {
    expect(component).toBeTruthy();
  });
});
