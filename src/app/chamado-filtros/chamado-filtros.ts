import { Component, input, output } from '@angular/core';
import { FiltroStatus } from '../models/filtro-status';

@Component({
  imports: [],
  selector: 'app-chamado-filtros',
  styleUrl: './chamado-filtros.css',
  templateUrl: './chamado-filtros.html',
})
export class ChamadoFiltros {
  readonly filtroAtual = input.required<FiltroStatus>();
  readonly filtroAlterado = output<FiltroStatus>();

  onSelecionarFiltro(filtro: FiltroStatus) {
    // return this.filtroAlterado(this.filtroAlterado.emit($event));
  }
}
