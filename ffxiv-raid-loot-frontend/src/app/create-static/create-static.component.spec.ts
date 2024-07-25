import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateStaticComponent } from './create-static.component';

describe('CreateStaticComponent', () => {
  let component: CreateStaticComponent;
  let fixture: ComponentFixture<CreateStaticComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateStaticComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreateStaticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
