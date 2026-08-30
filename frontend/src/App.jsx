import { useState } from "react";
import { ordersApi } from "./api.js";
import { LoginScreen } from "./components/LoginScreen.jsx";
import { OrdersScreen } from "./components/OrdersScreen.jsx";

const SESSION_KEY = "order-management-session";

function readSession() {
  try {
    return JSON.parse(sessionStorage.getItem(SESSION_KEY)) ?? null;
  } catch {
    return null;
  }
}

export function App() {
  const [session, setSession] = useState(readSession);
  const [isLoggingIn, setIsLoggingIn] = useState(false);
  const [loginError, setLoginError] = useState("");

  async function login({ email, password }) {
    setIsLoggingIn(true);
    setLoginError("");
    try {
      const response = await ordersApi.login(email, password);
      const nextSession = {
        email,
        token: response.accessToken,
        expiresAt: response.expiresAt,
      };

      // O token fica na sessão do navegador para não sobreviver ao fechamento da aba de avaliação.
      sessionStorage.setItem(SESSION_KEY, JSON.stringify(nextSession));
      setSession(nextSession);
    } catch (error) {
      setLoginError(error?.message || "Não foi possível entrar.");
    } finally {
      setIsLoggingIn(false);
    }
  }

  function logout() {
    sessionStorage.removeItem(SESSION_KEY);
    setSession(null);
  }

  if (!session) {
    return (
      <LoginScreen
        error={loginError}
        isSubmitting={isLoggingIn}
        onLogin={login}
      />
    );
  }

  return (
    <OrdersScreen
      email={session.email}
      token={session.token}
      onLogout={logout}
    />
  );
}
