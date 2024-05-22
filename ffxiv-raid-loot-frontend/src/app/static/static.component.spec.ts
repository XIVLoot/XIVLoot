import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaticComponent } from './static.component';

describe('StaticComponent', () => {
  let component: StaticComponent;
  let fixture: ComponentFixture<StaticComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaticComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StaticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
