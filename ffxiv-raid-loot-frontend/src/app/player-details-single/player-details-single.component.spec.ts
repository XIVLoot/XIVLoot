import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayerDetailsSingleComponent } from './player-details-single.component';

describe('PlayerDetailsSingleComponent', () => {
  let component: PlayerDetailsSingleComponent;
  let fixture: ComponentFixture<PlayerDetailsSingleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PlayerDetailsSingleComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PlayerDetailsSingleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
