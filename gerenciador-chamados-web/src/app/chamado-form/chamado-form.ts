import { Component, inject, signal } from '@angular/core';
import { NovoChamado } from '../models/novo-chamado';
import { FormsModule } from '@angular/forms';
import { Chamados } from '../services/chamados';

@Component({
  imports: [FormsModule],
  selector: 'app-chamado-form',
  styleUrl: './chamado-form.css',
  templateUrl: './chamado-form.html',
})
export class ChamadoForm {
  private readonly chamadosService = inject(Chamados);

  protected criando = this.chamadosService.criandoStatus;
  protected readonly erroCriacao = signal<string | null>(null);

  protected tituloNovoChamado = '';

  onCriarChamado(): void {
    const tituloNovoChamado: NovoChamado = {
      titulo: this.tituloNovoChamado.trim(),
    };

    if (!tituloNovoChamado.titulo) return;

    this.erroCriacao.set(null);

    this.chamadosService.criarChamado(tituloNovoChamado).subscribe({
      next: () => (this.tituloNovoChamado = ''),
      error: (err) => {
        console.error('Erro ao criar o chamado:', err);
        this.erroCriacao.set('Não foi possível criar o chamado!');
      },
    });
  }
}
