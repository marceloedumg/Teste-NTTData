const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? "").replace(/\/$/, "");

export class ApiError extends Error {
  constructor(message, status, details = null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.details = details;
  }
}

async function parseResponse(response) {
  if (response.status === 204) {
    return null;
  }

  const contentType = response.headers.get("content-type") ?? "";
  return contentType.includes("application/json")
    ? response.json()
    : response.text();
}

async function request(path, { token, ...options } = {}) {
  const headers = new Headers(options.headers);

  if (options.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  });
  const payload = await parseResponse(response);

  if (!response.ok) {
    // Problem Details é preservado para que a interface mostre o motivo real vindo da API.
    const message =
      payload?.detail ??
      payload?.title ??
      (typeof payload === "string" && payload) ??
      "Não foi possível concluir a solicitação.";

    throw new ApiError(message, response.status, payload);
  }

  return payload;
}

export const ordersApi = {
  login(email, password) {
    return request("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
  },

  list(token, page, pageSize = 5) {
    return request(`/api/orders?page=${page}&pageSize=${pageSize}`, { token });
  },

  getById(token, id) {
    return request(`/api/orders/${id}`, { token });
  },

  create(token, order) {
    return request("/api/orders", {
      token,
      method: "POST",
      body: JSON.stringify(order),
    });
  },

  cancel(token, id) {
    return request(`/api/orders/${id}/cancel`, {
      token,
      method: "PATCH",
    });
  },
};
