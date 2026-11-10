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
    fixture.componentRef.setInput('id', 1);
    fixture.componentRef.setInput('status', 'Aberto');
    fixture.componentRef.setInput('titulo', 'Chamado de teste');
    fixture.componentRef.setInput('salvando', false);
    fixture.componentRef.setInput('sincronizando', false);
    fixture.componentRef.setInput('sincronizacaoPendente', false);
    fixture.detectChanges();
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
