import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppMainLayout } from './main-layout.component';

describe('MainLayoutComponent', () => {
  let component: AppMainLayout;
  let fixture: ComponentFixture<AppMainLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppMainLayout],
    }).compileComponents();

    fixture = TestBed.createComponent(AppMainLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
