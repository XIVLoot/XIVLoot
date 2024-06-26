import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GearAcqHistorySingleComponent } from './gear-acq-history-single.component';

describe('GearAcqHistorySingleComponent', () => {
  let component: GearAcqHistorySingleComponent;
  let fixture: ComponentFixture<GearAcqHistorySingleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GearAcqHistorySingleComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GearAcqHistorySingleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
