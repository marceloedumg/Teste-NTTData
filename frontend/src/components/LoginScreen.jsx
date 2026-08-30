import { ArrowRight, Eye, EyeSlash, LockKey, WarningCircle } from "@phosphor-icons/react";
import { useState } from "react";

export function LoginScreen({ onLogin, isSubmitting, error }) {
  const [email, setEmail] = useState("dev@martech.com");
  const [password, setPassword] = useState("Senha@123");
  const [showPassword, setShowPassword] = useState(false);

  function handleSubmit(event) {
    event.preventDefault();
    onLogin({ email: email.trim(), password });
  }

  return (
    <main className="login-shell">
      <section className="login-brand" aria-label="Apresentação">
        <img
          className="login-brand__logo"
          src="/assets/ntt-data-logo.png"
          alt="NTT DATA"
        />
        <div className="login-brand__content">
          <p className="eyebrow eyebrow--light">ORDER MANAGEMENT</p>
          <h1>Pedidos claros.<br />Decisões seguras.</h1>
          <p>
            Uma interface direta para criar, acompanhar e cancelar pedidos com
            as regras de negócio da API sempre visíveis.
          </p>
        </div>
        <span className="login-brand__line" aria-hidden="true" />
      </section>

      <section className="login-form-panel">
        <div className="login-form-wrap">
          <p className="eyebrow">ACESSO À PLATAFORMA</p>
          <h2>Bem-vindo</h2>
          <p className="login-form-wrap__intro">
            Entre com as credenciais de avaliação para gerenciar os pedidos.
          </p>

          <form className="login-form" onSubmit={handleSubmit}>
            <label>
              <span>E-mail</span>
              <input
                autoComplete="username"
                type="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                required
              />
            </label>

            <label>
              <span>Senha</span>
              <div className="password-field">
                <input
                  autoComplete="current-password"
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  required
                />
                <button
                  aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"}
                  className="icon-button"
                  type="button"
                  onClick={() => setShowPassword((current) => !current)}
                >
                  {showPassword ? <EyeSlash size={20} /> : <Eye size={20} />}
                </button>
              </div>
            </label>

            {error && (
              <div className="inline-message inline-message--error" role="alert">
                <WarningCircle size={20} weight="fill" />
                <span>{error}</span>
              </div>
            )}

            <button className="primary-button primary-button--wide" disabled={isSubmitting}>
              <LockKey size={19} weight="bold" />
              {isSubmitting ? "Entrando..." : "Entrar"}
              {!isSubmitting && <ArrowRight size={19} weight="bold" />}
            </button>
          </form>

          <div className="credentials-note">
            <span>Ambiente de avaliação</span>
            <code>dev@martech.com · Senha@123</code>
          </div>
        </div>
      </section>
    </main>
  );
}
