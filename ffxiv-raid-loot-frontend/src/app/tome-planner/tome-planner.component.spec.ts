import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TomePlannerComponent } from './tome-planner.component';

describe('TomePlannerComponent', () => {
  let component: TomePlannerComponent;
  let fixture: ComponentFixture<TomePlannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TomePlannerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TomePlannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
