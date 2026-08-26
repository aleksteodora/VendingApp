import { TestBed } from '@angular/core/testing';

import { ProcessingFee } from './processing-fee';

describe('ProcessingFee', () => {
  let service: ProcessingFee;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProcessingFee);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
