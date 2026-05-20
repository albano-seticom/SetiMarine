/* LOGIN screen */
(function () {
  const mount = document.getElementById('login-mount');
  if (!mount) return;

  mount.innerHTML = `
    <div class="auth">
      <aside class="auth-visual">
        <span class="wordmark">SETI <span>M A R I N E</span></span>
        <div class="auth-visual-line">
          <em>Find your route.</em><br/>
          Your marina, in real time.
        </div>
        <div class="auth-visual-foot">Göcek · Avaré · Everywhere</div>
      </aside>

      <div class="auth-form">
        <div class="auth-form-inner">
          <div class="eyebrow">Acesso</div>
          <h1>Bem-vindo <em>de volta.</em></h1>
          <p class="auth-sub">Entre com suas credenciais para acessar o painel da marina.</p>

          <form>
            <div class="field">
              <div class="field-head"><label>E-mail</label></div>
              <input type="email" placeholder="voce@suamarina.com.br" />
            </div>

            <div class="field">
              <div class="field-head">
                <label>Senha</label>
                <a href="#" class="forgot">Esqueci</a>
              </div>
              <input type="password" placeholder="••••••••" />
            </div>

            <button type="button" class="btn" style="width:100%;margin-top:.5rem;" data-go="dashboard">Entrar</button>
          </form>

          <div class="auth-footer">
            Ainda não tem conta? <a href="#" data-go="cadastro">Criar conta grátis</a>
          </div>
        </div>
      </div>
    </div>
  `;
})();
