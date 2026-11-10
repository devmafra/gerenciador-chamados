import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChamadoCard } from './chamado-card/chamado-card';
import { Chamados } from './services/chamados';
import { ChamadoForm } from './chamado-form/chamado-form';
import { FormsModule } from '@angular/forms';
import { FiltroStatus } from './models/filtro-status';

@Component({
  imports: [RouterOutlet, ChamadoCard, FormsModule, ChamadoForm],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  protected readonly title = signal('gerenciador-chamados-web');
  private readonly chamadosService = inject(Chamados);

  protected readonly chamados = this.chamadosService.lista;

  protected readonly carregando = this.chamadosService.carregandoStatus;
  protected readonly mensagemErro = this.chamadosService.mensagemErro;

  protected readonly salvando = this.chamadosService.salvandoStatus;
  protected readonly mensagemErroSalvamento = this.chamadosService.mensagemErroSalvamento;

  protected readonly sincronizacaoPendente = this.chamadosService.sincronizacaoPendenteStatus;
  protected readonly sincronizando = this.chamadosService.sincronizandoStatus;

  protected readonly totalChamados = computed(() => this.chamados().length);

  protected readonly chamadosAbertos = computed(
    () => this.chamados().filter((c) => c.status === 'Aberto').length,
  );

  protected readonly filtroStatus = signal<FiltroStatus>('Todos');

  protected buscaTitulo = signal<string>('');

  protected readonly chamadosFiltrados = computed(() => {
    const filtro = this.filtroStatus();
    const busca = this.buscaTitulo().toLowerCase().trim();

    return this.chamados().filter(
      (c) => (filtro === 'Todos' || c.status === filtro) && c.titulo.toLowerCase().includes(busca),
    );
  });

  onAlterarFiltro(filtro: FiltroStatus): void {
    this.filtroStatus.set(filtro);
  }

  onAlterarStatus(id: number): void {
    this.chamadosService.editarStatus(id);
  }

  tentarSincronizarNovamente(): void {
    this.chamadosService.tentarSincronizarNovamente();
  }

  ngOnInit(): void {
    this.chamadosService.carregarDaApi();
  }
}
