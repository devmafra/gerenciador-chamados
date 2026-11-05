export type StatusChamado = 'Aberto' | 'EmAndamento' | 'Concluido';

export interface Chamado {
  id: number;
  titulo: string;
  status: StatusChamado;
}
