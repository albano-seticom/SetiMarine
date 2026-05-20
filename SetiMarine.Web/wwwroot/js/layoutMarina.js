// layoutMarina.js — SetiMarine v2
// Sistema de mapa visual com:
//   - Pan (Alt+drag ou drag livre no canvas)
//   - Zoom com scroll/botoes
//   - Vagas como containers com SVG de barco proporcional
//   - Drag-and-drop de embarcacoes DENTRO de cada vaga
//   - Multiplas embarcacoes por vaga, dimensionadas para caber no espaco disponivel

window.layoutMarina = (() => {
  // ── estado ─────────────────────────────────────────────────────────────────
  let _ref = null;      // DotNetRef para callbacks Blazor
  let _canvas = null;   // wrapper div (overflow:hidden)
  let _inner = null;    // div interno que tem transform

  let _px = 0, _py = 0, _sc = 1;   // translate X/Y e scale
  let _panning = false;
  let _psx = 0, _psy = 0;          // start do pan

  // Drag de embarcacao dentro da vaga
  let _drag = null; // { el, vagaEl, startX, startY, origLeft, origTop, embId, vagaId }

  const BOAT_ASPECT = {
    Lancha:  0.40,   // largura relativa ao comprimento (40%)
    Veleiro: 0.30,
    JetSki:  0.38,
    Escuna:  0.28,
    Outro:   0.38,
  };

  const SVG_MAP = {
    Lancha:  '/images/boats/top/lancha-top.svg',
    Veleiro: '/images/boats/top/veleiro-top.svg',
    JetSki:  '/images/boats/top/jetski-top.svg',
    Escuna:  '/images/boats/top/veleiro-top.svg',
    Outro:   '/images/boats/top/lancha-top.svg',
  };

  // cores por status (CSS var ou fallback)
  const STATUS_COLORS = {
    livre:          { bg: 'rgba(26,179,166,0.12)',  border: 'rgba(26,179,166,0.5)',  boat: '#1ab3a6' },
    ocupada:        { bg: 'rgba(22,53,103,0.18)',   border: 'rgba(29,71,133,0.55)',  boat: '#4d8fd4' },
    emmovimentacao: { bg: 'rgba(201,168,76,0.15)',  border: 'rgba(201,168,76,0.55)', boat: '#c9a84c' },
    manutencao:     { bg: 'rgba(232,69,60,0.12)',   border: 'rgba(232,69,60,0.5)',   boat: '#e8453c' },
  };

  // ── init ───────────────────────────────────────────────────────────────────
  function init(dotnetRef, canvasId) {
    _ref    = dotnetRef;
    _canvas = document.getElementById(canvasId);
    if (!_canvas) return;

    _inner  = _canvas.querySelector('.canvas-inner');
    if (!_inner) return;

    // Garantir posicionamento relativo no canvas
    _canvas.style.position = 'relative';
    _canvas.style.overflow = 'hidden';
    _inner.style.position  = 'absolute';
    _inner.style.transformOrigin = '0 0';

    applyTransform();
    renderBoatsInAllVagas();

    // Eventos de pan no canvas
    _canvas.addEventListener('mousedown',  onCanvasMouseDown);
    _canvas.addEventListener('mousemove',  onCanvasMouseMove);
    _canvas.addEventListener('mouseup',    onCanvasMouseUp);
    _canvas.addEventListener('mouseleave', onCanvasMouseUp);
    _canvas.addEventListener('wheel',      onWheel, { passive: false });

    // Drag das embarcacoes
    document.addEventListener('mousemove', onBoatDragMove);
    document.addEventListener('mouseup',   onBoatDragUp);
  }

  // ── render de barcos em todas as vagas ────────────────────────────────────
  function renderBoatsInAllVagas() {
    const vagaEls = _inner.querySelectorAll('.vaga-item[data-vaga-id]');
    vagaEls.forEach(el => renderBoatsInVaga(el));
  }

  function renderBoatsInVaga(vagaEl) {
    // Remove container anterior se existir
    let boatContainer = vagaEl.querySelector('.boat-container');
    if (boatContainer) boatContainer.remove();

    const embarcacoesJson = vagaEl.dataset.embarcacoes;
    if (!embarcacoesJson) return;

    let embarcacoes;
    try { embarcacoes = JSON.parse(embarcacoesJson); }
    catch { return; }
    if (!embarcacoes || embarcacoes.length === 0) return;

    const vagaW = parseFloat(vagaEl.style.width)  || 80;
    const vagaH = parseFloat(vagaEl.style.height) || 120;

    const statusCls = getStatusClass(vagaEl);
    const colors    = STATUS_COLORS[statusCls] || STATUS_COLORS.livre;
    const isVertical = vagaH >= vagaW;

    boatContainer = document.createElement('div');
    boatContainer.className = 'boat-container';
    boatContainer.style.cssText = `
      position: absolute; inset: 4px; pointer-events: none;
      display: flex;
      flex-direction: ${isVertical ? 'row' : 'column'};
      align-items: center; justify-content: center;
      gap: 3px;
    `;

    embarcacoes.forEach(emb => {
      const boatEl = buildBoatElement(emb, vagaW, vagaH, embarcacoes.length, isVertical, colors);
      boatEl.dataset.embId  = emb.id;
      boatEl.dataset.vagaId = vagaEl.dataset.vagaId;

      // Tornar draggable se modo edicao
      if (vagaEl.dataset.editMode === 'true') {
        boatEl.style.pointerEvents = 'all';
        boatEl.style.cursor = 'grab';
        boatEl.addEventListener('mousedown', onBoatDragStart);
      }

      boatContainer.appendChild(boatEl);
    });

    vagaEl.appendChild(boatContainer);
  }

  function buildBoatElement(emb, vagaW, vagaH, totalBoats, isVertical, colors) {
    const wrapper = document.createElement('div');
    wrapper.className = 'boat-slot';
    wrapper.dataset.embId = emb.id;

    // Calcular dimensoes da embarcacao proporcional ao tamanho da vaga
    // A embarcacao ocupa ate 85% do espaco disponivel na vaga
    const availW = isVertical
      ? Math.floor((vagaW - 12) / totalBoats) - 3
      : vagaW - 10;
    const availH = isVertical
      ? vagaH - 10
      : Math.floor((vagaH - 12) / totalBoats) - 3;

    const aspect = BOAT_ASPECT[emb.tipo] || 0.38;
    let bW, bH;

    if (isVertical) {
      // vaga vertical: barco apontando para frente (alto e estreito)
      bH = Math.min(availH, 200);
      bW = Math.min(availW, Math.round(bH * aspect));
      if (bW > availW) { bW = availW; bH = Math.round(bW / aspect); }
    } else {
      // vaga horizontal: barco deitado
      bW = Math.min(availW, 200);
      bH = Math.min(availH, Math.round(bW * aspect));
      if (bH > availH) { bH = availH; bW = Math.round(bH / aspect); }
    }

    bW = Math.max(bW, 10);
    bH = Math.max(bH, 16);

    wrapper.style.cssText = `
      position: relative;
      width: ${bW}px; height: ${bH}px;
      flex-shrink: 0;
      display: flex; align-items: center; justify-content: center;
      ${!isVertical ? 'transform: rotate(90deg);' : ''}
    `;

    // SVG da embarcacao via <img> (usa os arquivos existentes em /images/boats/top/)
    const img = document.createElement('img');
    img.src   = SVG_MAP[emb.tipo] || SVG_MAP.Outro;
    img.alt   = emb.nome || emb.tipo;
    img.style.cssText = `
      width: 100%; height: 100%;
      object-fit: contain;
      filter: brightness(0) invert(1);
      opacity: 0.75;
      color: ${colors.boat};
      transition: transform 0.15s, opacity 0.15s;
      pointer-events: none;
      user-select: none;
    `;
    // Colorir o SVG com a cor do status via filter
    img.style.filter = buildColorFilter(colors.boat);

    // Tooltip com nome
    wrapper.title = emb.nome || emb.tipo;

    wrapper.appendChild(img);

    // Hover
    wrapper.addEventListener('mouseenter', () => {
      img.style.opacity = '1';
      img.style.transform = 'scale(1.06)';
    });
    wrapper.addEventListener('mouseleave', () => {
      img.style.opacity = '0.75';
      img.style.transform = 'scale(1)';
    });

    return wrapper;
  }

  // Converte hex/rgb em aproximacao de CSS filter para colorir SVG
  function buildColorFilter(color) {
    // Cores predefinidas → filtros aproximados
    const presets = {
      '#1ab3a6': 'brightness(0) saturate(100%) invert(58%) sepia(60%) saturate(400%) hue-rotate(140deg) brightness(95%)',
      '#4d8fd4': 'brightness(0) saturate(100%) invert(52%) sepia(40%) saturate(600%) hue-rotate(185deg) brightness(100%)',
      '#c9a84c': 'brightness(0) saturate(100%) invert(70%) sepia(50%) saturate(500%) hue-rotate(10deg) brightness(95%)',
      '#e8453c': 'brightness(0) saturate(100%) invert(35%) sepia(80%) saturate(600%) hue-rotate(330deg) brightness(100%)',
    };
    return presets[color] || 'brightness(0) invert(0.8)';
  }

  function getStatusClass(vagaEl) {
    const classes = ['livre', 'ocupada', 'emmovimentacao', 'manutencao'];
    for (const c of classes) { if (vagaEl.classList.contains(c)) return c; }
    return 'livre';
  }

  // ── Drag de embarcacao dentro da vaga ────────────────────────────────────
  function onBoatDragStart(e) {
    e.stopPropagation();
    e.preventDefault();
    const boatEl  = e.currentTarget;
    const vagaEl  = boatEl.closest('.vaga-item');
    if (!vagaEl) return;

    const rect = boatEl.getBoundingClientRect();
    _drag = {
      el:        boatEl,
      vagaEl:    vagaEl,
      startX:    e.clientX,
      startY:    e.clientY,
      origLeft:  boatEl.offsetLeft,
      origTop:   boatEl.offsetTop,
      embId:     parseInt(boatEl.dataset.embId),
      vagaId:    parseInt(boatEl.dataset.vagaId),
    };
    boatEl.style.cursor    = 'grabbing';
    boatEl.style.zIndex    = '50';
    boatEl.style.position  = 'absolute';
    boatEl.style.left      = _drag.origLeft + 'px';
    boatEl.style.top       = _drag.origTop  + 'px';
  }

  function onBoatDragMove(e) {
    if (!_drag) return;
    const dx    = (e.clientX - _drag.startX) / _sc;
    const dy    = (e.clientY - _drag.startY) / _sc;
    const vagaW = parseFloat(_drag.vagaEl.style.width)  || 80;
    const vagaH = parseFloat(_drag.vagaEl.style.height) || 120;
    const bW    = parseFloat(_drag.el.style.width)      || 30;
    const bH    = parseFloat(_drag.el.style.height)     || 50;

    const newL = Math.max(0, Math.min(_drag.origLeft + dx, vagaW - bW - 4));
    const newT = Math.max(0, Math.min(_drag.origTop  + dy, vagaH - bH - 4));

    _drag.el.style.left = newL + 'px';
    _drag.el.style.top  = newT + 'px';
  }

  function onBoatDragUp(e) {
    if (!_drag) return;
    const dx = (e.clientX - _drag.startX) / _sc;
    const dy = (e.clientY - _drag.startY) / _sc;
    _drag.el.style.cursor = 'grab';
    _drag.el.style.zIndex = '';

    // So persiste se moveu mais de 4px
    if (Math.abs(dx) > 4 || Math.abs(dy) > 4) {
      const posX = parseFloat(_drag.el.style.left);
      const posY = parseFloat(_drag.el.style.top);
      if (_ref) {
        _ref.invokeMethodAsync('OnEmbarcacaoPosicionada', _drag.embId, _drag.vagaId, Math.round(posX), Math.round(posY))
          .catch(e => console.warn('layoutMarina: OnEmbarcacaoPosicionada falhou', e));
      }
    }
    _drag = null;
  }

  // ── Pan do canvas ─────────────────────────────────────────────────────────
  function onCanvasMouseDown(e) {
    if (e.altKey || e.button === 1 || e.button === 0) {
      // Ignorar cliques em vagas (deixar Blazor tratar)
      if (e.target.closest('.vaga-item') && !e.altKey && e.button === 0) return;
      _panning = true;
      _psx = e.clientX - _px;
      _psy = e.clientY - _py;
      _canvas.style.cursor = 'grabbing';
      e.preventDefault();
    }
  }

  function onCanvasMouseMove(e) {
    if (!_panning) return;
    _px = e.clientX - _psx;
    _py = e.clientY - _psy;
    applyTransform();
  }

  function onCanvasMouseUp() {
    if (_panning) { _panning = false; _canvas.style.cursor = ''; }
  }

  function onWheel(e) {
    e.preventDefault();
    const delta  = e.deltaY < 0 ? 0.1 : -0.1;
    const newSc  = Math.min(3, Math.max(0.25, _sc + delta));
    // Zoom centrado no ponteiro
    const rect   = _canvas.getBoundingClientRect();
    const mx     = e.clientX - rect.left;
    const my     = e.clientY - rect.top;
    _px = mx - (mx - _px) * (newSc / _sc);
    _py = my - (my - _py) * (newSc / _sc);
    _sc = newSc;
    applyTransform();
  }

  function applyTransform() {
    if (!_inner) return;
    _inner.style.transform = `translate(${_px}px, ${_py}px) scale(${_sc.toFixed(3)})`;
  }

  // ── API publica ────────────────────────────────────────────────────────────
  function resetView() {
    _px = 0; _py = 0; _sc = 1;
    applyTransform();
  }

  function zoomIn()  { _sc = Math.min(3, _sc + 0.15); applyTransform(); }
  function zoomOut() { _sc = Math.max(0.25, _sc - 0.15); applyTransform(); }

  // Chamado pelo Blazor para atualizar embarcacoes de uma vaga especifica
  function updateVaga(vagaId, embarcacoesJson) {
    if (!_inner) return;
    const vagaEl = _inner.querySelector(`.vaga-item[data-vaga-id="${vagaId}"]`);
    if (!vagaEl) return;
    vagaEl.dataset.embarcacoes = embarcacoesJson;
    renderBoatsInVaga(vagaEl);
  }

  // Ativa o modo de arraste de embarcacoes em uma vaga
  function setEditMode(vagaId, enabled) {
    if (!_inner) return;
    const vagaEl = _inner.querySelector(`.vaga-item[data-vaga-id="${vagaId}"]`);
    if (!vagaEl) return;
    vagaEl.dataset.editMode = enabled ? 'true' : 'false';
    renderBoatsInVaga(vagaEl);
  }

  function destroy() {
    if (!_canvas) return;
    _canvas.removeEventListener('mousedown',  onCanvasMouseDown);
    _canvas.removeEventListener('mousemove',  onCanvasMouseMove);
    _canvas.removeEventListener('mouseup',    onCanvasMouseUp);
    _canvas.removeEventListener('mouseleave', onCanvasMouseUp);
    _canvas.removeEventListener('wheel',      onWheel);
    document.removeEventListener('mousemove', onBoatDragMove);
    document.removeEventListener('mouseup',   onBoatDragUp);
    _canvas = null; _inner = null; _ref = null;
  }

  return { init, destroy, resetView, zoomIn, zoomOut, updateVaga, setEditMode };
})();
