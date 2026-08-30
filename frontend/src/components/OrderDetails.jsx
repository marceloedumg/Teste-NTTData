import { Info, SpinnerGap, Trash, X } from "@phosphor-icons/react";
import { formatCurrency, formatDate, shortId } from "../formatters.js";
import { StatusBadge } from "./StatusBadge.jsx";

export function OrderDetails({ order, isLoading, onClose, onRequestCancel }) {
  return (
    <aside className="order-details" aria-label="Detalhes do pedido">
      <button
        aria-label="Fechar detalhes"
        className="details-close icon-button"
        type="button"
        onClick={onClose}
      >
        <X size={25} />
      </button>

      {isLoading || !order ? (
        <div className="details-loading" role="status">
          <SpinnerGap className="spin" size={30} />
          <span>Carregando pedido...</span>
        </div>
      ) : (
        <div className="details-content">
          <header className="details-header">
            <p className="eyebrow">PEDIDO</p>
            <h2 title={order.id}>{shortId(order.id)}</h2>
            <div className="details-header__meta">
              <StatusBadge status={order.status} />
              <span>Criado em {formatDate(order.createdAt)}</span>
            </div>
          </header>

          <section className="details-section">
            <p className="details-label">Cliente</p>
            <strong>{shortId(order.customerId, 24)}</strong>
            <span className="details-muted" title={order.customerId}>{order.customerId}</span>
          </section>

          <section className="details-section details-items">
            <h3>Itens do pedido</h3>
            <div className="details-items__header" aria-hidden="true">
              <span>Item</span>
              <span>Qtd.</span>
              <span>Preço unit.</span>
              <span>Total</span>
            </div>
            {order.items.map((item) => (
              <div className="details-item" key={item.id}>
                <span>
                  <strong>{item.productName}</strong>
                  <small>{shortId(item.id, 14)}</small>
                </span>
                <span>{item.quantity}</span>
                <span>{formatCurrency(item.unitPrice)}</span>
                <strong>{formatCurrency(item.totalAmount)}</strong>
              </div>
            ))}
          </section>

          <section className="details-total">
            <span>Total do pedido</span>
            <strong>{formatCurrency(order.totalAmount)}</strong>
          </section>

          <footer className={`details-action ${order.status !== "Pending" ? "details-action--muted" : ""}`}>
            <p>
              <Info size={20} weight="fill" />
              {order.status === "Pending"
                ? "Pedidos pendentes podem ser cancelados antes da confirmação."
                : "Somente pedidos pendentes podem ser cancelados."}
            </p>
            {order.status === "Pending" && (
              <button className="cancel-button" type="button" onClick={onRequestCancel}>
                <Trash size={19} weight="bold" />
                Cancelar pedido
              </button>
            )}
          </footer>
        </div>
      )}
    </aside>
  );
}
