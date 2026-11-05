import { Component, input, output } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-chamado-card',
  styleUrl: './chamado-card.css',
  templateUrl: './chamado-card.html',
})
export class ChamadoCard {
  id = input.required<number>();
  status = input.required<string>();
  titulo = input.required<string>();
  salvando = input.required<boolean>();
  sincronizando = input.required<boolean>();
  sincronizacaoPendente = input.required<boolean>();

  alterarStatus = output<number>();

  onClick(): void {
    this.alterarStatus.emit(this.id());
  }
}
