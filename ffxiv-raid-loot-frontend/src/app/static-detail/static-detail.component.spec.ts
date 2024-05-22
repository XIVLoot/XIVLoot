import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaticDetailComponent } from './static-detail.component';

describe('StaticDetailComponent', () => {
  let component: StaticDetailComponent;
  let fixture: ComponentFixture<StaticDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaticDetailComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StaticDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
