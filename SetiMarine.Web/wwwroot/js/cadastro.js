/* CADASTRO screen (3 etapas) */
(function () {
  const mount = document.getElementById('cadastro-mount');
  if (!mount) return;

  let etapa = 1;
  let data = {
    razao: '', fantasia: '', cnpj: '', telefone: '', email: '',
    plano: 'pro',
    nome: '', emailAdmin: '', senha: '', confirmar: ''
  };

  const planos = [
    { id: 'ess', nome: 'Essencial',    preco: 'R$ 299/mês' },
    { id: 'pro', nome: 'Profissional', preco: 'R$ 599/mês', featured: true },
    { id: 'ent', nome: 'Enterprise',   preco: 'Sob consulta' }
  ];

  function render() {
    mount.innerHTML = `
      <div class="auth">
        <aside class="auth-visual">
          <span class="wordmark">SETI <span>M A R I N E</span></span>
          <div class="auth-visual-line">
            <em>Find your escape.</em><br/>
            Comece sua marina em minutos.
          </div>
          <div class="auth-visual-foot">Sem taxa de adesão · Suporte PT-BR</div>
        </aside>

        <div class="auth-form">
          <div class="auth-form-inner" style="max-width:460px;">
            <div class="eyebrow">Criar conta</div>
            <h1>${etapa < 4 ? 'Sua nova <em>marina.</em>' : 'Pronto.'}</h1>
            ${etapa < 4 ? `<p class="auth-sub">Três etapas rápidas. Nenhum cartão necessário.</p>` : ''}

            ${etapa < 4 ? `
              <div class="steps">
                <div class="step ${etapa===1?'active':etapa>1?'done':''}">
                  <div class="step-num">${etapa>1?'✓':'1'}</div> Empresa
                </div>
                <div class="step-line"></div>
                <div class="step ${etapa===2?'active':etapa>2?'done':''}">
                  <div class="step-num">${etapa>2?'✓':'2'}</div> Admin
                </div>
                <div class="step-line"></div>
                <div class="step ${etapa===3?'active':''}">
                  <div class="step-num">3</div> Confirmação
                </div>
              </div>
            ` : ''}

            <div id="cad-body"></div>
          </div>
        </div>
      </div>
    `;

    const body = mount.querySelector('#cad-body');
    if (etapa === 1) body.innerHTML = etapa1();
    if (etapa === 2) body.innerHTML = etapa2();
    if (etapa === 3) body.innerHTML = etapa3();
    if (etapa === 4) body.innerHTML = sucesso();

    bind();
  }

  function etapa1() {
    return `
      <div class="field">
        <div class="field-head"><label>Razão Social *</label></div>
        <input id="f-razao" value="${data.razao}" placeholder="Marina Exemplo Ltda" />
      </div>
      <div class="field">
        <div class="field-head"><label>Nome Fantasia</label></div>
        <input id="f-fantasia" value="${data.fantasia}" placeholder="Marina Exemplo" />
      </div>
      <div class="row2">
        <div class="field">
          <div class="field-head"><label>CNPJ *</label></div>
          <input id="f-cnpj" value="${data.cnpj}" placeholder="00.000.000/0001-00" />
        </div>
        <div class="field">
          <div class="field-head"><label>Telefone</label></div>
          <input id="f-tel" value="${data.telefone}" placeholder="(00) 00000-0000" />
        </div>
      </div>
      <div class="field">
        <div class="field-head"><label>E-mail da empresa *</label></div>
        <input id="f-email" type="email" value="${data.email}" placeholder="contato@suamarina.com.br" />
      </div>

      <div class="field">
        <div class="field-head"><label>Plano *</label></div>
        ${planos.map(p => `
          <div class="plano-opt ${data.plano===p.id?'selected':''}" data-plano="${p.id}">
            <div class="plano-nome">${p.nome}</div>
            <div class="plano-preco">${p.preco}</div>
          </div>
        `).join('')}
      </div>

      <div class="cad-actions">
        <button class="btn btn--ghost" data-go="home">Cancelar</button>
        <button class="btn" id="next1">Próximo →</button>
      </div>
    `;
  }

  function etapa2() {
    return `
      <div class="field">
        <div class="field-head"><label>Nome completo *</label></div>
        <input id="f-nome" value="${data.nome}" placeholder="João da Silva" />
      </div>
      <div class="field">
        <div class="field-head"><label>E-mail de acesso *</label></div>
        <input id="f-emailadmin" type="email" value="${data.emailAdmin}" placeholder="joao@suamarina.com.br" />
      </div>
      <div class="row2">
        <div class="field">
          <div class="field-head"><label>Senha *</label></div>
          <input id="f-senha" type="password" value="${data.senha}" placeholder="Mínimo 6 caracteres" />
        </div>
        <div class="field">
          <div class="field-head"><label>Confirmar *</label></div>
          <input id="f-confirmar" type="password" value="${data.confirmar}" placeholder="Repita" />
        </div>
      </div>

      <div class="cad-actions">
        <button class="btn btn--ghost" id="back2">← Voltar</button>
        <button class="btn" id="next2">Próximo →</button>
      </div>
    `;
  }

  function etapa3() {
    const p = planos.find(p => p.id === data.plano);
    return `
      <div style="margin-bottom:2rem;">
        <div class="confirm-row"><span class="confirm-label">Razão Social</span><span class="confirm-val">${data.razao || '—'}</span></div>
        <div class="confirm-row"><span class="confirm-label">Nome Fantasia</span><span class="confirm-val">${data.fantasia || '—'}</span></div>
        <div class="confirm-row"><span class="confirm-label">CNPJ</span><span class="confirm-val">${data.cnpj || '—'}</span></div>
        <div class="confirm-row"><span class="confirm-label">E-mail empresa</span><span class="confirm-val">${data.email || '—'}</span></div>
        <div class="confirm-row"><span class="confirm-label">Plano</span><span class="confirm-val">${p?.nome || '—'}</span></div>
        <div class="confirm-row"><span class="confirm-label">Administrador</span><span class="confirm-val">${data.nome || '—'}</span></div>
        <div class="confirm-row"><span class="confirm-label">E-mail de acesso</span><span class="confirm-val">${data.emailAdmin || '—'}</span></div>
      </div>
      <div class="cad-actions">
        <button class="btn btn--ghost" id="back3">← Voltar</button>
        <button class="btn" id="finalizar">Criar conta</button>
      </div>
    `;
  }

  function sucesso() {
    return `
      <div class="success">
        <div class="success-mark">✓</div>
        <h2 style="font-family:var(--f-display);font-weight:300;font-size:1.8rem;color:var(--navy-800);margin-bottom:.5rem;">Conta criada</h2>
        <p style="color:var(--ink-500);margin-bottom:2rem;">Sua marina está pronta. Bem-vindo ao SetiMarine.</p>
        <button class="btn" data-go="dashboard" style="width:100%;">Ir para o painel →</button>
      </div>
    `;
  }

  function bind() {
    mount.querySelectorAll('.plano-opt').forEach(el => {
      el.addEventListener('click', () => { data.plano = el.dataset.plano; render(); });
    });

    const save1 = () => {
      data.razao    = mount.querySelector('#f-razao')?.value || '';
      data.fantasia = mount.querySelector('#f-fantasia')?.value || '';
      data.cnpj     = mount.querySelector('#f-cnpj')?.value || '';
      data.telefone = mount.querySelector('#f-tel')?.value || '';
      data.email    = mount.querySelector('#f-email')?.value || '';
    };
    const save2 = () => {
      data.nome       = mount.querySelector('#f-nome')?.value || '';
      data.emailAdmin = mount.querySelector('#f-emailadmin')?.value || '';
      data.senha      = mount.querySelector('#f-senha')?.value || '';
      data.confirmar  = mount.querySelector('#f-confirmar')?.value || '';
    };

    mount.querySelector('#next1')?.addEventListener('click', () => { save1(); etapa = 2; render(); });
    mount.querySelector('#next2')?.addEventListener('click', () => { save2(); etapa = 3; render(); });
    mount.querySelector('#back2')?.addEventListener('click', () => { save2(); etapa = 1; render(); });
    mount.querySelector('#back3')?.addEventListener('click', () => { etapa = 2; render(); });
    mount.querySelector('#finalizar')?.addEventListener('click', () => { etapa = 4; render(); });
  }

  render();
})();
