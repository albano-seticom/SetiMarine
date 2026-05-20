/* MAPA (fullscreen) */
(function () {
  const mount = document.getElementById('mapa-mount');
  if (!mount) return;

  // Layout mais denso estilo foto aérea
  function genPier(label, y, count, pattern) {
    const statuses = pattern;
    const dock = `<div class="pier" style="left:80px;top:${y+24}px;width:${count*70 + 40}px;height:14px;"></div>`;
    const lbl = `<div class="zone-lbl" style="left:80px;top:${y}px;">${label}</div>`;
    const up = Array.from({length: count}).map((_,i) => {
      const st = statuses[i % statuses.length];
      return `<div class="slip ${st}" style="left:${140 + i*70}px;top:${y-26}px;width:54px;height:44px;" data-code="${label.replace('PIER ','')}${i+1}" data-status="${st}">
        <span class="slip-code">${label.replace('PIER ','')}${i+1}</span>
      </div>`;
    }).join('');
    const down = Array.from({length: count}).map((_,i) => {
      const st = statuses[(i+2) % statuses.length];
      return `<div class="slip ${st}" style="left:${140 + i*70}px;top:${y+44}px;width:54px;height:44px;" data-code="${label.replace('PIER ','')}${count+i+1}" data-status="${st}">
        <span class="slip-code">${label.replace('PIER ','')}${count+i+1}</span>
      </div>`;
    }).join('');
    return lbl + dock + up + down;
  }

  const piers =
    genPier('PIER A', 110, 10, ['ocupada','ocupada','livre','ocupada','mov','ocupada','livre','ocupada']) +
    genPier('PIER B', 230, 10, ['livre','ocupada','ocupada','manutencao','ocupada','livre','ocupada','ocupada']) +
    genPier('PIER C', 350, 10, ['ocupada','ocupada','ocupada','ocupada','mov','livre','ocupada','ocupada']) +
    genPier('PIER D', 470, 10, ['livre','ocupada','manutencao','ocupada','ocupada','ocupada','livre','ocupada']);

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
