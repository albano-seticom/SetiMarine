/* MAPA (fullscreen) */
(function () {
  const mount = document.getElementById('mapa-mount');
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
    // Algumas vagas têm 2 embarcações (jetski duplo ou lancha + jetski)
    const isDouble = (pi * 3 + idx) % 11 === 0 && status === 'ocupada';
    if (isDouble) {
      const tipo2 = TIPO_POOL[(pi * 7 + idx + 4) % TIPO_POOL.length];
      return `<div class="slip-boats-row">${boatSVG(tipo1, 14)}${boatSVG(tipo2, 14)}</div>`;
    }
    return `<div class="slip-boats-row">${boatSVG(tipo1, 19)}</div>`;
  }

  function genPier(label, y, count, pattern, pi) {
    const pfx = label.replace('PIER ', '');
    const dock = `<div class="pier" style="left:80px;top:${y+24}px;width:${count*70 + 40}px;height:14px;"></div>`;
    const lbl  = `<div class="zone-lbl" style="left:80px;top:${y}px;">${label}</div>`;

    const up = Array.from({length: count}).map((_, i) => {
      const st    = pattern[i % pattern.length];
      const code  = `${pfx}${i+1}`;
      const boats = slipContent(st, i, pi);
      return `<div class="slip ${st}${boats ? ' slip--boats' : ''}" style="left:${140 + i*70}px;top:${y-26}px;width:54px;height:44px;" data-code="${code}" data-status="${st}" data-tipo="${TIPO_POOL[(pi*7+i)%TIPO_POOL.length]}">
        ${boats}<span class="slip-code">${code}</span>
      </div>`;
    }).join('');

    const down = Array.from({length: count}).map((_, i) => {
      const st    = pattern[(i+2) % pattern.length];
      const code  = `${pfx}${count+i+1}`;
      const boats = slipContent(st, count + i, pi);
      return `<div class="slip ${st}${boats ? ' slip--boats' : ''}" style="left:${140 + i*70}px;top:${y+44}px;width:54px;height:44px;" data-code="${code}" data-status="${st}" data-tipo="${TIPO_POOL[(pi*7+count+i)%TIPO_POOL.length]}">
        ${boats}<span class="slip-code">${code}</span>
      </div>`;
    }).join('');

    return lbl + dock + up + down;
  }

  const piers =
    genPier('PIER A', 110, 10, ['ocupada','ocupada','livre','ocupada','mov','ocupada','livre','ocupada'], 0) +
    genPier('PIER B', 230, 10, ['livre','ocupada','ocupada','manutencao','ocupada','livre','ocupada','ocupada'], 1) +
    genPier('PIER C', 350, 10, ['ocupada','ocupada','ocupada','ocupada','mov','livre','ocupada','ocupada'], 2) +
    genPier('PIER D', 470, 10, ['livre','ocupada','manutencao','ocupada','ocupada','ocupada','livre','ocupada'], 3);

  mount.innerHTML = `
    <div class="app mapa-page">
      <aside class="sidebar">
        <div class="sidebar-brand"><span class="wordmark">SETI <span>M A R I N E</span></span></div>
        <nav>
          <div class="sidebar-group">Operação</div>
          <a class="sidebar-item" href="#" data-go="dashboard">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 12l9-9 9 9M5 10v10h14V10"/></svg>
            Visão geral
          </a>
          <a class="sidebar-item active" href="#" data-go="mapa">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 6l6-3 6 3 6-3v15l-6 3-6-3-6 3z"/></svg>
            Mapa da marina
          </a>
          <a class="sidebar-item" href="#"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="5" width="18" height="14" rx="1"/><path d="M3 10h18"/></svg>Vagas</a>
          <a class="sidebar-item" href="#"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 4-6 8-6s8 2 8 6"/></svg>Clientes</a>
        </nav>
        <div class="sidebar-foot"><strong>Marina Göcek</strong>138 vagas · Profissional</div>
      </aside>

      <main class="content">
        <div class="page-head">
          <div>
            <h1>Mapa <em>completo</em></h1>
            <div class="page-head-date">Arraste vagas para reposicionar · Scroll para zoom</div>
          </div>
          <div class="page-head-actions">
            <a class="btn btn--ghost btn--sm" href="#">Centralizar</a>
            <a class="btn btn--ghost btn--sm" href="#">Listar vagas</a>
            <a class="btn btn--sm" href="#">+ Nova vaga</a>
          </div>
        </div>

        <div class="map-card">
          <div class="map-card-head">
            <div class="map-card-title"><span class="live-dot"></span>Marina Göcek · 138 vagas</div>
            <div class="legend">
              <span class="legend-i"><span class="legend-sq free"></span>Livre</span>
              <span class="legend-i"><span class="legend-sq occ"></span>Ocupada</span>
              <span class="legend-i"><span class="legend-sq mov"></span>Movimentação</span>
              <span class="legend-i"><span class="legend-sq maint"></span>Manutenção</span>
            </div>
          </div>
          <div class="map-body">${piers}</div>
        </div>
      </main>
    </div>
  `;
})();
