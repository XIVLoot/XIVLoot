import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AbAuthComponent } from './about.component';

describe('AbAuthComponent', () => {
  let component: AbAuthComponent;
  let fixture: ComponentFixture<AbAuthComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AbAuthComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AbAuthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
