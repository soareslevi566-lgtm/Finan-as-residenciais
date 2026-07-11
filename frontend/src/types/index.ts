export type Pessoa={id:number;nome:string;idade:number};
export type TipoTransacao='Despesa'|'Receita';
export type CategoriaTransacao='Moradia'|'Alimentacao'|'Transporte'|'Saude'|'Educacao'|'Lazer'|'Salario'|'Outros';
export type Transacao={id:number;descricao:string;valor:number;tipo:TipoTransacao;pessoaId:number;pessoaNome:string;categoria:CategoriaTransacao;data:string};
export type FiltrosTransacao={pessoaId?:number;tipo?:TipoTransacao;categoria?:CategoriaTransacao;inicio?:string;fim?:string;busca?:string};
export type TotalPessoa={pessoaId:number;nome:string;idade:number;totalReceitas:number;totalDespesas:number;saldo:number};
export type Totais={pessoas:TotalPessoa[];totalGeralReceitas:number;totalGeralDespesas:number;saldoGeral:number};
