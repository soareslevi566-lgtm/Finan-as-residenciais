import axios from 'axios'; import type {Pessoa,TipoTransacao,Totais,Transacao} from '../types';
export const api=axios.create({baseURL:import.meta.env.VITE_API_URL||'http://localhost:5000/api'});
export const pessoasApi={listar:()=>api.get<Pessoa[]>('/pessoas'),criar:(d:{nome:string;idade:number})=>api.post<Pessoa>('/pessoas',d),excluir:(id:number)=>api.delete(`/pessoas/${id}`)};
export const transacoesApi={listar:()=>api.get<Transacao[]>('/transacoes'),criar:(d:{descricao:string;valor:number;tipo:TipoTransacao;pessoaId:number})=>api.post<Transacao>('/transacoes',d)};
export const totaisApi={obter:()=>api.get<Totais>('/totais')};
export function mensagemErro(e:unknown){return axios.isAxiosError(e)?(e.response?.data?.mensagem||'Não foi possível concluir a operação.'):'Ocorreu um erro inesperado.'}
