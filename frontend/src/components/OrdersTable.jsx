import {
  CaretRight,
  Copy,
  Package,
} from "@phosphor-icons/react";
import { formatCurrency, formatDate, shortId } from "../formatters.js";
import { StatusBadge } from "./StatusBadge.jsx";

function LoadingRows() {
  return Array.from({ length: 5 }, (_, index) => (
    <tr className="table-loading-row" key={index}>
      {Array.from({ length: 5 }, (__, cell) => (
        <td key={cell}><span className="skeleton" /></td>
      ))}
    </tr>
  ));
}

export function OrdersTable({ items, isLoading, selectedId, onSelect, onCopied }) {
  async function copyId(event, id) {
    event.stopPropagation();
    await navigator.clipboard.writeText(id);
    onCopied("ID do pedido copiado.");
  }

  return (
    <div className="orders-table-wrap">
      <table className="orders-table">
        <thead>
          <tr>
            <th>Pedido</th>
            <th>Cliente</th>
            <th>Data</th>
            <th>Status</th>
            <th className="table-money">Total</th>
            <th><span className="sr-only">Abrir</span></th>
          </tr>
        </thead>
        <tbody>
          {isLoading ? (
            <LoadingRows />
          ) : (
            items.map((order) => (
              <tr
                className={selectedId === order.id ? "is-selected" : ""}
                key={order.id}
                tabIndex="0"
                onClick={() => onSelect(order.id)}
                onKeyDown={(event) => {
                  if (event.key === "Enter" || event.key === " ") {
                    event.preventDefault();
                    onSelect(order.id);
                  }
                }}
              >
                <td>
                  <span className="order-id-cell">
                    <strong>{shortId(order.id)}</strong>
                    <button
                      aria-label={`Copiar ID ${order.id}`}
                      className="copy-button"
                      type="button"
                      onClick={(event) => copyId(event, order.id)}
                    >
                      <Copy size={17} />
                    </button>
                  </span>
                </td>
                <td>
                  <span className="customer-id">{shortId(order.customerId, 18)}</span>
                </td>
                <td>{formatDate(order.createdAt)}</td>
                <td><StatusBadge status={order.status} /></td>
                <td className="table-money"><strong>{formatCurrency(order.totalAmount)}</strong></td>
                <td className="table-arrow"><CaretRight size={18} /></td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      {!isLoading && items.length === 0 && (
        <div className="empty-state">
          <Package size={42} weight="thin" />
          <h3>Nenhum pedido encontrado</h3>
          <p>Ajuste a busca ou o filtro para ver outros resultados.</p>
        </div>
      )}
    </div>
  );
}
