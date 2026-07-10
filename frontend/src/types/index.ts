export type Pessoa={id:number;nome:string;idade:number};
export type TipoTransacao='Despesa'|'Receita';
export type Transacao={id:number;descricao:string;valor:number;tipo:TipoTransacao;pessoaId:number;pessoaNome:string};
export type TotalPessoa={pessoaId:number;nome:string;idade:number;totalReceitas:number;totalDespesas:number;saldo:number};
export type Totais={pessoas:TotalPessoa[];totalGeralReceitas:number;totalGeralDespesas:number;saldoGeral:number};
