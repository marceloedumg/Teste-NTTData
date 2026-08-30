const dateFormatter = new Intl.DateTimeFormat("pt-BR", {
  day: "2-digit",
  month: "2-digit",
  year: "numeric",
  hour: "2-digit",
  minute: "2-digit",
});

const currencyFormatter = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

export function formatDate(value) {
  return dateFormatter.format(new Date(value));
}

export function formatCurrency(value) {
  return currencyFormatter.format(Number(value));
}

export function shortId(value, length = 13) {
  return value ? value.slice(0, length) : "—";
}

export function statusLabel(status) {
  const labels = {
    Pending: "Pendente",
    Confirmed: "Confirmado",
    Cancelled: "Cancelado",
    Canceled: "Cancelado",
  };

  return labels[status] ?? status;
}

export function statusTone(status) {
  if (status === "Pending") return "pending";
  if (status === "Confirmed") return "confirmed";
  return "cancelled";
}
