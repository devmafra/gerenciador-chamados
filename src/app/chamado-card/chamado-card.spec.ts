import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChamadoCard } from './chamado-card';

describe('ChamadoCard', () => {
  let component: ChamadoCard;
  let fixture: ComponentFixture<ChamadoCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChamadoCard],
    }).compileComponents();

    fixture = TestBed.createComponent(ChamadoCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
