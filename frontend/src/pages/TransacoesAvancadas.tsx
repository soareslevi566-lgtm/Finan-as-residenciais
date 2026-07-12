import { useCallback, useEffect, useState } from 'react';
import type { CategoriaTransacao, FiltrosTransacao, Pessoa, TipoTransacao, Transacao } from '../types';
import { mensagemErro, pessoasApi, transacoesApi } from '../services/api';

const brl = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });
const categorias: { valor: CategoriaTransacao; nome: string }[] = [
  { valor: 'Moradia', nome: 'Moradia' }, { valor: 'Alimentacao', nome: 'Alimentação' },
  { valor: 'Transporte', nome: 'Transporte' }, { valor: 'Saude', nome: 'Saúde' },
  { valor: 'Educacao', nome: 'Educação' }, { valor: 'Lazer', nome: 'Lazer' },
  { valor: 'Salario', nome: 'Salário' }, { valor: 'Outros', nome: 'Outros' }
];
const hoje = () => new Date().toISOString().slice(0, 10);

export default function TransacoesAvancadas() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [itens, setItens] = useState<Transacao[]>([]);
  const [descricao, setDescricao] = useState(''); const [valor, setValor] = useState('');
  const [tipo, setTipo] = useState<TipoTransacao>('Despesa'); const [categoria, setCategoria] = useState<CategoriaTransacao>('Moradia');
  const [data, setData] = useState(hoje()); const [pessoaId, setPessoaId] = useState('');
  const [filtros, setFiltros] = useState<Record<string, string>>({}); const [ocupado, setOcupado] = useState(true);
  const [aviso, setAviso] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);
  const selecionada = pessoas.find(p => p.id === Number(pessoaId)); const menor = !!selecionada && selecionada.idade < 18;

  const carregar = useCallback(async (f: Record<string, string> = filtros) => {
    setOcupado(true);
    try {
      const params: FiltrosTransacao = { pessoaId: f.pessoaId ? Number(f.pessoaId) : undefined, tipo: (f.tipo || undefined) as TipoTransacao | undefined, categoria: (f.categoria || undefined) as CategoriaTransacao | undefined, inicio: f.inicio || undefined, fim: f.fim || undefined, busca: f.busca?.trim() || undefined };
      const [p, t] = await Promise.all([pessoasApi.listar(), transacoesApi.listar(params)]); setPessoas(p.data); setItens(t.data);
    } catch (e) { setAviso({ tipo: 'error', texto: mensagemErro(e) }); } finally { setOcupado(false); }
  }, [filtros]);

  useEffect(() => { carregar({}); }, []);
  useEffect(() => { if (menor) setTipo('Despesa'); }, [menor]);

  async function cadastrar(e: React.FormEvent) {
    e.preventDefault();
    if (!descricao.trim() || Number(valor) <= 0 || !pessoaId || !data) { setAviso({ tipo: 'error', texto: 'Preencha todos os campos com valores válidos.' }); return; }
    setOcupado(true);
    try {
      await transacoesApi.criar({ descricao, valor: Number(valor), tipo, pessoaId: Number(pessoaId), categoria, data: `${data}T12:00:00Z` });
      setDescricao(''); setValor(''); setData(hoje()); setAviso({ tipo: 'ok', texto: 'Transação registrada com sucesso.' }); await carregar(filtros);
    } catch (e) { setAviso({ tipo: 'error', texto: mensagemErro(e) }); setOcupado(false); }
  }
  function limpar() { setFiltros({}); carregar({}); }
  async function excluir(transacao: Transacao) {
    if (!confirm(`Excluir o lançamento “${transacao.descricao}”? Esta ação não poderá ser desfeita.`)) return;
    try {
      await transacoesApi.excluir(transacao.id);
      setAviso({ tipo: 'ok', texto: 'Lançamento excluído com sucesso.' });
      await carregar(filtros);
    } catch (e) { setAviso({ tipo: 'error', texto: mensagemErro(e) }); }
  }

  return <main><header className="page-head"><p>CONTROLE DA CASA</p><h1>Transações</h1><span>Registre, organize e encontre cada movimento</span></header>
    <div className="grid transactions-grid"><section className="card"><h2>Nova transação</h2><form onSubmit={cadastrar}>
      <label>Descrição<input value={descricao} onChange={e => setDescricao(e.target.value)} maxLength={250} placeholder="Ex.: Conta de energia" /></label>
      <div className="form-pair"><label>Valor<input type="number" min="0.01" step="0.01" value={valor} onChange={e => setValor(e.target.value)} /></label><label>Data<input type="date" max={hoje()} value={data} onChange={e => setData(e.target.value)} /></label></div>
      <div className="form-pair"><label>Tipo<select value={tipo} onChange={e => setTipo(e.target.value as TipoTransacao)}><option>Despesa</option><option disabled={menor}>Receita</option></select></label><label>Categoria<select value={categoria} onChange={e => setCategoria(e.target.value as CategoriaTransacao)}>{categorias.map(c => <option key={c.valor} value={c.valor}>{c.nome}</option>)}</select></label></div>
      <label>Pessoa<select value={pessoaId} onChange={e => setPessoaId(e.target.value)}><option value="">Selecione</option>{pessoas.map(p => <option value={p.id} key={p.id}>{p.nome} · {p.idade} anos</option>)}</select></label>
      {menor && <p className="rule">Menores de 18 anos só podem registrar despesas.</p>}<button disabled={ocupado || !pessoas.length}>Registrar transação</button>
    </form>{aviso && <div className={`notice ${aviso.tipo}`} role="status">{aviso.texto}</div>}</section>
    <section className="card"><h2>Lançamentos · {itens.length}</h2><div className="filters" aria-label="Filtros de transações">
      <input aria-label="Buscar descrição" placeholder="Buscar descrição" value={filtros.busca || ''} onChange={e => setFiltros({ ...filtros, busca: e.target.value })} />
      <select aria-label="Filtrar pessoa" value={filtros.pessoaId || ''} onChange={e => setFiltros({ ...filtros, pessoaId: e.target.value })}><option value="">Todas as pessoas</option>{pessoas.map(p => <option value={p.id} key={p.id}>{p.nome}</option>)}</select>
      <select aria-label="Filtrar tipo" value={filtros.tipo || ''} onChange={e => setFiltros({ ...filtros, tipo: e.target.value })}><option value="">Todos os tipos</option><option>Despesa</option><option>Receita</option></select>
      <select aria-label="Filtrar categoria" value={filtros.categoria || ''} onChange={e => setFiltros({ ...filtros, categoria: e.target.value })}><option value="">Todas as categorias</option>{categorias.map(c => <option key={c.valor} value={c.valor}>{c.nome}</option>)}</select>
      <input aria-label="Data inicial" type="date" value={filtros.inicio || ''} onChange={e => setFiltros({ ...filtros, inicio: e.target.value })} /><input aria-label="Data final" type="date" value={filtros.fim || ''} onChange={e => setFiltros({ ...filtros, fim: e.target.value })} />
      <button type="button" onClick={() => carregar(filtros)}>Aplicar</button><button type="button" className="secondary" onClick={limpar}>Limpar</button></div>
      {ocupado && !itens.length ? <div className="empty">Carregando…</div> : itens.length === 0 ? <div className="empty">Nenhuma transação encontrada.</div> : <div className="list">{itens.map(t => <div className="transaction detailed" key={t.id}><div><strong>{t.descricao}</strong><small>{t.pessoaNome} · {categorias.find(c => c.valor === t.categoria)?.nome} · {new Date(t.data).toLocaleDateString('pt-BR')}</small></div><div className="transaction-actions"><span className={t.tipo === 'Receita' ? 'income' : 'expense'}>{t.tipo === 'Receita' ? '+' : '−'} {brl.format(t.valor)}</span><button type="button" className="icon-delete" aria-label={`Excluir ${t.descricao}`} onClick={() => excluir(t)}>Excluir</button></div></div>)}</div>}
    </section></div></main>;
}
