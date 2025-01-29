import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StackedAreaChartComponent } from './stacked-area-chart.component';

describe('StackedAreaChartComponent', () => {
  let component: StackedAreaChartComponent;
  let fixture: ComponentFixture<StackedAreaChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StackedAreaChartComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StackedAreaChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
