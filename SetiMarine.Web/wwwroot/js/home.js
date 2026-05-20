/* HOME screen */
(function () {
  const mount = document.getElementById('home-mount');
  if (!mount) return;

  mount.innerHTML = `
    <div class="home">
      <div class="home-video-fallback"></div>
      <!-- 🎬 VÍDEO DE FUNDO: em produção, coloque seu vídeo em wwwroot/videos/hero.mp4
           e descomente o bloco abaixo. O fallback gradient navy já cobre o caso sem vídeo. -->
      <!--
      <video class="home-video" autoplay muted loop playsinline>
        <source src="/videos/hero.mp4" type="video/mp4" />
      </video>
      -->
      
      
      <div class="home-video-overlay"></div>

      <nav class="home-nav">
        <span class="wordmark">SETI <span>M A R I N E</span></span>
        <div class="home-nav-right">
          <a class="home-nav-link" href="#" data-go="login">Entrar</a>
          <a class="btn btn--ghost-light btn--sm" href="#" data-go="cadastro">Criar conta</a>
        </div>
      </nav>

      <section class="home-hero">
        <div class="eyebrow eyebrow--pill"><span class="eyebrow-dot"></span>Sistema SaaS Náutico · Nível Mundial</div>
        <h1 class="home-hero-title">
          Gestão da sua marina<br/>
          <em>em tempo real</em>
        </h1>
        <p class="home-hero-sub">
          Painel Kanban, mapa visual de vagas, WhatsApp integrado e app<br/>
          para seus clientes — tudo em uma plataforma.
        </p>
        <div class="home-hero-ctas">
          <a class="btn btn--cream" href="#" data-go="cadastro">Começar gratuitamente</a>
          <a class="btn btn--ghost-light" href="#">Ver demonstração</a>
        </div>
        <div class="home-scroll">
          <span>Scroll</span>
          <div class="home-scroll-line"></div>
        </div>
      </section>

      <section class="home-strip">
        <div class="home-strip-inner">
          <h2>Uma plataforma <em>silenciosa</em>, feita para quem administra marinas de verdade.</h2>
          <p>Painel visual em tempo real, mapa de vagas, WhatsApp integrado e app do cliente — sem ruído, sem fricção. Apenas o que a sua operação precisa para funcionar com elegância.</p>
        </div>
      </section>

      <section class="home-features">
        <div class="home-features-head">
          <span class="eyebrow">O que entregamos</span>
          <h2>Controle total da sua operação</h2>
          <p>De vagas à cobrança, do WhatsApp ao app do cliente — uma plataforma única, silenciosa, precisa.</p>
        </div>
        <div class="home-features-grid">
          <div class="feature">
            <div class="feature-num">01</div>
            <h3>Mapa em tempo real</h3>
            <p>Visualize cada vaga, cada embarcação, cada movimento. Clique para gerenciar, sem planilhas.</p>
          </div>
          <div class="feature">
            <div class="feature-num">02</div>
            <h3>WhatsApp integrado</h3>
            <p>Cobrança, agendamento e comunicação direto no canal que o cliente já usa.</p>
          </div>
          <div class="feature">
            <div class="feature-num">03</div>
            <h3>App do cliente (PWA)</h3>
            <p>Seu cliente solicita retirada, acompanha a fila e vê o histórico — sem app store.</p>
          </div>
          <div class="feature">
            <div class="feature-num">04</div>
            <h3>Kanban de operação</h3>
            <p>Demandas de manobra, manutenção e retirada organizadas por equipe e turno.</p>
          </div>
          <div class="feature">
            <div class="feature-num">05</div>
            <h3>Multilíngue · PT · EN · ES</h3>
            <p>Atenda clientes internacionais com a mesma naturalidade que atende os locais.</p>
          </div>
          <div class="feature">
            <div class="feature-num">06</div>
            <h3>Relatórios financeiros</h3>
            <p>Receita por vaga, por cliente, por embarcação — sem exportar para o Excel.</p>
          </div>
        </div>
      </section>

      <section class="home-tagline">
        <div class="home-tagline-left">
          <span class="wordmark">SETI <span>M A R I N E</span></span>
        </div>
        <div class="home-tagline-right">
          <div class="home-tagline-line">Find your dreams.<br/><em>Find your escape.</em><br/>Find your route.</div>
          <div class="home-tagline-sub">Gestão náutica feita com o mesmo cuidado de uma marina bem administrada.</div>
        </div>
      </section>

      <section class="home-plans">
        <div class="home-plans-head">
          <span class="eyebrow">Planos & Preços</span>
          <h2>Escolha o plano ideal</h2>
          <p>Flexível para marinas de todos os tamanhos. Sem taxa de adesão.</p>
        </div>
        <div class="plans-grid">
          <div class="plan">
            <div class="plan-name">Essencial</div>
            <div class="plan-price"><span class="plan-price-val">R$ 299</span><span class="plan-price-mo">/mês</span></div>
            <ul class="plan-list">
              <li>Até 50 embarcações</li>
              <li>Até 3 usuários</li>
              <li>Mapa visual de vagas</li>
              <li>Suporte por e-mail</li>
            </ul>
            <a class="btn btn--ghost" href="#" data-go="cadastro">Assinar</a>
          </div>
          <div class="plan plan--featured">
            <span class="plan-badge">Popular</span>
            <div class="plan-name">Profissional</div>
            <div class="plan-price"><span class="plan-price-val">R$ 599</span><span class="plan-price-mo">/mês</span></div>
            <ul class="plan-list">
              <li>Até 200 embarcações</li>
              <li>Usuários ilimitados</li>
              <li>WhatsApp integrado</li>
              <li>App do cliente (PWA)</li>
              <li>Relatórios avançados</li>
            </ul>
            <a class="btn btn--cream" href="#" data-go="cadastro">Assinar</a>
          </div>
          <div class="plan">
            <div class="plan-name">Enterprise</div>
            <div class="plan-price"><span class="plan-price-val" style="font-style:italic;font-size:2rem;">Sob consulta</span></div>
            <ul class="plan-list">
              <li>Embarcações ilimitadas</li>
              <li>Multi-marina</li>
              <li>Integrações customizadas</li>
              <li>Gerente de conta dedicado</li>
            </ul>
            <a class="btn btn--ghost" href="#">Falar com vendas</a>
          </div>
        </div>
      </section>

      <footer class="home-footer">
        <span class="wordmark">SETI <span>M A R I N E</span></span>
        <div>
          <a href="#">Termos</a>
          <a href="#">Privacidade</a>
          <a href="#">Contato</a>
        </div>
        <span>© 2026 SETICOM · Avaré, SP</span>
      </footer>
    </div>
  `;
})();
