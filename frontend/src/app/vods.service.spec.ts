import { TestBed } from '@angular/core/testing';

import { VodsService } from './vods.service';

describe('VodsService', () => {
  let service: VodsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VodsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
