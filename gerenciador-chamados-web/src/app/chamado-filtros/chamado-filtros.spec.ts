import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChamadoFiltros } from './chamado-filtros';

describe('ChamadoFiltros', () => {
  let component: ChamadoFiltros;
  let fixture: ComponentFixture<ChamadoFiltros>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChamadoFiltros],
    }).compileComponents();

    fixture = TestBed.createComponent(ChamadoFiltros);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
