import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreSelectionModalComponent } from './store-selection-modal.component';

describe('StoreSelectionModalComponent', () => {
  let component: StoreSelectionModalComponent;
  let fixture: ComponentFixture<StoreSelectionModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [StoreSelectionModalComponent]
    });
    fixture = TestBed.createComponent(StoreSelectionModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
