/* DASHBOARD screen */
(function () {
  const mount = document.getElementById('dashboard-mount');
  if (!mount) return;

  // Ícones SVG por tipo de embarcação (vista lateral, viewBox 0 0 24 24)
  const boatPaths = {
    veleiro: `
      <path d="M3 18c0-3 3-4 9-4s9 1 9 4" stroke-linecap="round"/>
      <path d="M5 18l1 3h12l1-3" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M12 14V3" stroke-linecap="round"/>
      <path d="M12 3L5 14" stroke-linecap="round"/>
      <path d="M12 6l6 8" stroke-linecap="round"/>`,

    lancha: `
      <path d="M3 18c0-3 3-4 9-4s9 1 9 4" stroke-linecap="round"/>
      <path d="M5 18l1 3h12l1-3" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M9.5 18l1.5-6h4l1.5 6" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M9.5 12h7" stroke-linecap="round"/>`,

    catamara: `
      <path d="M2 19c0-1.8 1-2.4 5-2.4s5 .6 5 2.4" stroke-linecap="round"/>
      <path d="M3.5 19l.5 2.5h6.5l.5-2.5" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M12 19c0-1.8 1-2.4 5-2.4s5 .6 5 2.4" stroke-linecap="round"/>
      <path d="M13.5 19l.5 2.5h6.5l.5-2.5" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M7.5 19h9" stroke-linecap="round"/>
      <path d="M12 19V9" stroke-linecap="round"/>
      <path d="M12 10L7 19" stroke-linecap="round"/>`,

    jetski: `
      <path d="M5 18c0-2 2-2.5 7-2.5s7 .5 7 2.5" stroke-linecap="round"/>
      <path d="M7 18l1 3h9l1-3" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M8 18l2-5h5l1 5" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M11 13V9" stroke-linecap="round"/>
      <path d="M8.5 9h6" stroke-linecap="round" stroke-width="1.8"/>`
  };

  const TIPO_POOL = ['veleiro', 'lancha', 'lancha', 'catamara', 'lancha', 'veleiro', 'jetski', 'lancha'];

  function boatSVG(tipo, size) {
    const paths = boatPaths[tipo] || boatPaths.lancha;
    return `<svg width="${size}" height="${size}" viewBox="0 0 24 24" fill="none" stroke="rgba(250,245,232,.9)" stroke-width="1.35" class="slip-boat">${paths}</svg>`;
  }

  function slipContent(status, idx, pi) {
    if (status === 'livre') return '';
    const tipo1 = TIPO_POOL[(pi * 7 + idx) % TIPO_POOL.length];
    const isDouble = (pi * 3 + idx) % 11 === 0 && status === 'ocupada';
    if (isDouble) {
      const tipo2 = TIPO_POOL[(pi * 7 + idx + 4) % TIPO_POOL.length];
      return `<div class="slip-boats-row">${boatSVG(tipo1, 16)}${boatSVG(tipo2, 16)}</div>`;
    }
    return `<div class="slip-boats-row">${boatSVG(tipo1, 22)}</div>`;
  }

  // Slips gerados programaticamente — piers + status
  const piers = [
    { label: 'PIER A', y: 120, slips: [
      ['A1','ocupada'],['A2','ocupada'],['A3','livre'],['A4','ocupada'],
      ['A5','mov'],['A6','livre'],['A7','ocupada'],['A8','livre']
    ]},
    { label: 'PIER B', y: 240, slips: [
      ['B1','livre'],['B2','ocupada'],['B3','ocupada'],['B4','manutencao'],
      ['B5','livre'],['B6','ocupada'],['B7','ocupada'],['B8','ocupada']
    ]},
    { label: 'PIER C', y: 360, slips: [
      ['C1','ocupada'],['C2','ocupada'],['C3','ocupada'],['C4','ocupada'],
      ['C5','livre'],['C6','mov'],['C7','livre'],['C8','ocupada']
    ]}
  ];

  const slipsHtml = piers.map((p, pi) => {
    const dock  = `<div class="pier" style="left:60px;top:${p.y+22}px;width:720px;height:14px;"></div>`;
    const label = `<div class="zone-lbl" style="left:60px;top:${p.y}px;">${p.label}</div>`;
    const up = p.slips.slice(0,4).map((s,i) => {
      const boats = slipContent(s[1], i, pi);
      return `<div class="slip ${s[1]}${boats ? ' slip--boats' : ''}" style="left:${120 + i*90}px;top:${p.y-30}px;width:70px;height:48px;" data-code="${s[0]}" data-status="${s[1]}">
        ${boats}<span class="slip-code">${s[0]}</span>
      </div>`;
    }).join('');
    const down = p.slips.slice(4,8).map((s,i) => {
      const boats = slipContent(s[1], 4 + i, pi);
      return `<div class="slip ${s[1]}${boats ? ' slip--boats' : ''}" style="left:${120 + i*90}px;top:${p.y+42}px;width:70px;height:48px;" data-code="${s[0]}" data-status="${s[1]}">
        ${boats}<span class="slip-code">${s[0]}</span>
      </div>`;
    }).join('');
    return label + dock + up + down;
  }).join('');

  const dryLabel = `<div class="zone-lbl" style="left:60px;top:490px;">DRY STACK</div>`;
  const dry = Array.from({length:12}).map((_,i) => {
    const statuses = ['ocupada','ocupada','livre','ocupada','mov','ocupada','livre','ocupada','ocupada','manutencao','livre','ocupada'];
    const st    = statuses[i];
    const col   = i % 6, row = Math.floor(i/6);
    const boats = slipContent(st, i, 4);
    return `<div class="slip ${st}${boats ? ' slip--boats' : ''}" style="left:${120 + col*90}px;top:${520 + row*46}px;width:70px;height:38px;" data-code="S${i+1}" data-status="${st}">
      ${boats}<span class="slip-code">S${i+1}</span>
    </div>`;
  }).join('');

  mount.innerHTML = `
    <div class="app">
      <aside class="sidebar">
        <div class="sidebar-brand">
          <div class="sidebar-brand-logo">
            <div class="sidebar-logo-square">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                <circle cx="12" cy="5" r="1.5"/>
                <path d="M12 7v14"/>
                <path d="M9 9h6"/>
                <path d="M5 18a7 7 0 0 0 14 0"/>
              </svg>
            </div>
            <span class="sidebar-brand-name">SetiMarine</span>
          </div>
          <button class="sidebar-collapse" aria-label="Collapse">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M15 6l-6 6 6 6"/></svg>
          </button>
        </div>
        <nav>
          <div class="sidebar-group">Principal</div>
          <a class="sidebar-item active" href="#" data-go="dashboard">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/></svg>
            Dashboard
          </a>
          <a class="sidebar-item" href="#" data-go="mapa">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 6l6-3 6 3 6-3v15l-6 3-6-3-6 3z"/><path d="M9 3v15M15 6v15"/></svg>
            Mapa da Marina
          </a>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M4 12h12M12 6l6 6-6 6"/></svg>
            Movimentações
          </a>

          <div class="sidebar-group">Configurações</div>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 6h18M3 12h18M3 18h18"/></svg>
            Corredores
          </a>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="5" width="18" height="14" rx="1"/><path d="M3 10h18"/></svg>
            Vagas
          </a>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 16c0-3 3-4 9-4s9 1 9 4"/><path d="M5 16l1 4h12l1-4"/><path d="M12 12V6"/><path d="M9 6h6"/></svg>
            Embarcações
          </a>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 4-6 8-6s8 2 8 6"/></svg>
            Clientes
          </a>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M5 4v16M19 4v16"/><path d="M5 8h14M5 16h14"/></svg>
            Serviços
          </a>

          <div class="sidebar-group">Gestão</div>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="9" cy="8" r="3"/><circle cx="17" cy="10" r="2.5"/><path d="M3 20c0-3 2.5-5 6-5s6 2 6 5"/><path d="M14 18c0-2 1.5-3 3.5-3s3.5 1 3.5 3"/></svg>
            Usuários
          </a>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M4 20V8M10 20V4M16 20v-8M22 20V12"/></svg>
            Relatórios
          </a>
          <a class="sidebar-item" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M4 21V7l8-4 8 4v14"/><path d="M9 21v-6h6v6"/></svg>
            Minha Empresa
          </a>

          <div class="sidebar-group sidebar-group--accent">Super Admin</div>
          <a class="sidebar-item sidebar-item--accent" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 3l2.5 5.5L20 9l-4 4 1 6-5-3-5 3 1-6-4-4 5.5-.5z"/></svg>
            Painel SETICOM
          </a>
        </nav>
        <div class="sidebar-foot">
          <a class="sidebar-item sidebar-item--muted" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="5" width="18" height="12" rx="1"/><path d="M8 21h8"/></svg>
            Planos <span class="sidebar-badge">Em breve</span>
          </a>
          <a class="sidebar-item sidebar-item--muted" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M2 12h2M20 12h2M5 5l1.5 1.5M17.5 17.5L19 19M5 19l1.5-1.5M17.5 6.5L19 5"/></svg>
            Tema Claro
          </a>
          <div class="sidebar-user">
            <div class="sidebar-user-avatar">P</div>
            <div class="sidebar-user-info">
              <strong>paulo</strong>
              <span>Super Admin</span>
            </div>
            <svg class="sidebar-user-chev" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M9 6l6 6-6 6"/></svg>
          </div>
          <a class="sidebar-item sidebar-item--danger" href="#">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M9 5H5v14h4M14 8l4 4-4 4M18 12H9"/></svg>
            Sair
          </a>
        </div>
      </aside>

      <main class="content">
        <div class="page-head">
          <div>
            <h1>Visão <em>geral</em></h1>
            <div class="page-head-date">Sexta-feira · 18 de abril de 2026</div>
          </div>
          <div class="page-head-actions">
            <a class="btn btn--ghost btn--sm" href="#">Exportar</a>
            <a class="btn btn--sm" href="#">+ Nova vaga</a>
          </div>
        </div>

        <div class="kpis">
          <div class="kpi kpi--free">
            <div class="kpi-label">Vagas livres</div>
            <div class="kpi-val">24</div>
            <div class="kpi-foot"><span class="kpi-dot"></span>17% do total</div>
          </div>
          <div class="kpi kpi--occ">
            <div class="kpi-label">Ocupadas</div>
            <div class="kpi-val">108</div>
            <div class="kpi-foot"><span class="kpi-dot"></span>78% de taxa</div>
          </div>
          <div class="kpi kpi--mov">
            <div class="kpi-label">Em movimentação</div>
            <div class="kpi-val">4</div>
            <div class="kpi-foot"><span class="kpi-dot"></span>entradas ativas</div>
          </div>
          <div class="kpi kpi--maint">
            <div class="kpi-label">Manutenção</div>
            <div class="kpi-val">2</div>
            <div class="kpi-foot"><span class="kpi-dot"></span>fora de operação</div>
          </div>
        </div>

        <div class="map-card">
          <div class="map-card-head">
            <div class="map-card-title">
              <span class="live-dot"></span>
              Mapa da marina · Tempo real
            </div>
            <div class="legend">
              <span class="legend-i"><span class="legend-sq free"></span>Livre</span>
              <span class="legend-i"><span class="legend-sq occ"></span>Ocupada</span>
              <span class="legend-i"><span class="legend-sq mov"></span>Movimentação</span>
              <span class="legend-i"><span class="legend-sq maint"></span>Manutenção</span>
            </div>
          </div>
          <div class="map-body" id="map-body">
            ${slipsHtml}
            ${dryLabel}
            ${dry}

            <div class="detail-panel" id="detail-panel" hidden>
              <div class="detail-head">
                <span class="detail-code" id="d-code">A1</span>
                <button class="detail-close" id="d-close">✕</button>
              </div>
              <span class="detail-badge ocupada" id="d-badge">Ocupada</span>
              <div class="detail-row"><span class="detail-key">Tipo</span><span class="detail-val">Molhada</span></div>
              <div class="detail-row"><span class="detail-key">Corredor</span><span class="detail-val">Pier A</span></div>
              <div class="detail-row"><span class="detail-key">Tamanho</span><span class="detail-val">42 pés</span></div>
              <div class="detail-row"><span class="detail-key">Cliente</span><span class="detail-val">João Almeida</span></div>
              <div class="detail-emblbl">Embarcações</div>
              <div id="d-embs"><span class="detail-emb">Boa Viagem</span></div>
              <a href="#" class="detail-cta">Editar vaga</a>
            </div>
          </div>
        </div>
      </main>
    </div>
  `;

  // interaction
  const panel = mount.querySelector('#detail-panel');
  const dcode = mount.querySelector('#d-code');
  const dbadge = mount.querySelector('#d-badge');
  const labels = { livre: 'Livre', ocupada: 'Ocupada', mov: 'Movimentação', manutencao: 'Manutenção' };

  mount.querySelectorAll('.slip').forEach(s => {
    s.addEventListener('click', () => {
      const code = s.dataset.code;
      const status = s.dataset.status;
      dcode.textContent = code;
      dbadge.textContent = labels[status];
      dbadge.className = 'detail-badge ' + status;
      panel.hidden = false;
    });
  });
  mount.querySelector('#d-close').addEventListener('click', () => panel.hidden = true);
})();
