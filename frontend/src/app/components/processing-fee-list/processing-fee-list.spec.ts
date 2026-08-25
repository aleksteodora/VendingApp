import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessingFeeList } from './processing-fee-list';

describe('ProcessingFeeList', () => {
  let component: ProcessingFeeList;
  let fixture: ComponentFixture<ProcessingFeeList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProcessingFeeList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProcessingFeeList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
