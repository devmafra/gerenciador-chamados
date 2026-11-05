import { inject, Service, signal } from '@angular/core';
import { Chamado } from '../models/chamado';
import { NovoChamado } from '../models/novo-chamado';
import { HttpClient } from '@angular/common/http';
import { defer, EMPTY, finalize, Observable, tap } from 'rxjs';

@Service()
export class Chamados {
  private readonly chamados = signal<Chamado[]>([]);
  private readonly carregando = signal<boolean>(false);
  private readonly salvando = signal<boolean>(false);
  private readonly sincronizacaoPendente = signal<boolean>(false);
  private readonly sincronizando = signal<boolean>(false);
  private readonly criando = signal<boolean>(false);
  private readonly erro = signal<string | null>(null);
  private readonly erroSalvamento = signal<string | null>(null);

  readonly lista = this.chamados.asReadonly();
  readonly carregandoStatus = this.carregando.asReadonly();
  readonly salvandoStatus = this.salvando.asReadonly();
  readonly mensagemErro = this.erro.asReadonly();
  readonly mensagemErroSalvamento = this.erroSalvamento.asReadonly();
  readonly sincronizacaoPendenteStatus = this.sincronizacaoPendente.asReadonly();
  readonly sincronizandoStatus = this.sincronizando.asReadonly();
  readonly criandoStatus = this.criando.asReadonly();

  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5051/chamados';

  public editarStatus(id: number): void {
    const chamado = this.chamados().find((c) => c.id === id);
    if (!chamado) return;

    let novoStatus: 2 | 3;

    switch (chamado.status) {
      case 'Aberto':
        novoStatus = 2;
        break;
      case 'EmAndamento':
        novoStatus = 3;
        break;
      default:
        return;
    }

    if (this.salvando() || this.sincronizacaoPendente()) {
      return;
    }
    this.erroSalvamento.set(null);

    this.salvando.set(true);
    this.atualizarStatusNaApi(id, novoStatus)
      .pipe(finalize(() => this.salvando.set(false)))
      .subscribe({
        next: (result) => {
          this.chamados.update((lista) =>
            lista.map((chamado) => (chamado.id === id ? result : chamado)),
          );
        },
        error: (err) => {
          if (err.status === 409) {
            this.sincronizacaoPendente.set(true);
            this.erroSalvamento.set('Os dados do servidor foram alterados!');
            this.sincronizarAposConflito();
            return;
          }

          console.error('Não foi possível atualizar o chamado:', err);
          this.erroSalvamento.set('Não foi possível atualizar o chamado!');
        },
      });
  }

  private atualizarStatusNaApi(id: number, novoStatus: number): Observable<Chamado> {
    const patchUrl = `${this.apiUrl}/${id}/status`;
    return this.http.patch<Chamado>(patchUrl, { status: novoStatus });
  }

  private buscarDaApi(): Observable<Chamado[]> {
    return this.http.get<Chamado[]>(this.apiUrl);
  }

  public carregarDaApi(): void {
    this.carregando.set(true);
    this.erro.set(null);
    this.buscarDaApi()
      .pipe(finalize(() => this.carregando.set(false)))
      .subscribe({
        next: (result) => {
          this.chamados.set(result);
        },
        error: (err) => {
          console.error('Erro ao carregar chamados:', err);
          this.erro.set('Não foi possível carregar os chamados!');
        },
      });
  }
  private sincronizarAposConflito(): void {
    if (this.sincronizando()) return;
    this.sincronizando.set(true);
    this.buscarDaApi()
      .pipe(finalize(() => this.sincronizando.set(false)))
      .subscribe({
        next: (result) => {
          this.chamados.set(result);
          this.sincronizacaoPendente.set(false);
          this.erroSalvamento.set(null);
        },
        error: (err) => {
          console.error('Erro ao sincronizar os dados:', err);
          this.erroSalvamento.set('Erro ao sincronizar os dados!');
        },
      });
  }

  public tentarSincronizarNovamente(): void {
    if (!this.sincronizacaoPendente() || this.sincronizando()) {
      return;
    }
    this.erroSalvamento.set(null);
    this.sincronizarAposConflito();
  }

  private criarNaApi(novoChamado: NovoChamado): Observable<Chamado> {
    return this.http.post<Chamado>(this.apiUrl, novoChamado);
  }

  public criarChamado(novoChamado: NovoChamado): Observable<Chamado> {
    return defer(() => {
      if (this.criando()) return EMPTY;
      if (!novoChamado.titulo.trim()) return EMPTY;

      this.criando.set(true);

      return this.criarNaApi(novoChamado).pipe(
        tap((result) => this.chamados.update((lista) => [...lista, result])),
        finalize(() => this.criando.set(false)),
      );
    });
  }
}
