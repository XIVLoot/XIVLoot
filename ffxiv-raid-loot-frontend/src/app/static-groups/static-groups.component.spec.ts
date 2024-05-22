import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaticGroupsComponent } from './static-groups.component';

describe('StaticGroupsComponent', () => {
  let component: StaticGroupsComponent;
  let fixture: ComponentFixture<StaticGroupsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaticGroupsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StaticGroupsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
