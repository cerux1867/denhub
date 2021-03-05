import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VodComponent } from './vod.component';

describe('VodComponent', () => {
  let component: VodComponent;
  let fixture: ComponentFixture<VodComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VodComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
