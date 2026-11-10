import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChamadoForm } from './chamado-form';

describe('ChamadoForm', () => {
  let component: ChamadoForm;
  let fixture: ComponentFixture<ChamadoForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChamadoForm],
    }).compileComponents();

    fixture = TestBed.createComponent(ChamadoForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
