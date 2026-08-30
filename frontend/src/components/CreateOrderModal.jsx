import {
  ArrowRight,
  Plus,
  SpinnerGap,
  Trash,
  WarningCircle,
  X,
} from "@phosphor-icons/react";
import { useEffect, useMemo, useState } from "react";
import { formatCurrency } from "../formatters.js";

function newItem() {
  return {
    key: crypto.randomUUID(),
    productName: "",
    quantity: 1,
    unitPrice: "",
  };
}

export function CreateOrderModal({ isOpen, isSubmitting, error, onClose, onSubmit }) {
  const [customerId, setCustomerId] = useState("");
  const [items, setItems] = useState(() => [newItem()]);

  const total = useMemo(
    () => items.reduce(
      (sum, item) => sum + Number(item.quantity || 0) * Number(item.unitPrice || 0),
      0,
    ),
    [items],
  );

  useEffect(() => {
    if (!isOpen) return undefined;

    function closeOnEscape(event) {
      if (event.key === "Escape" && !isSubmitting) onClose();
    }

    document.body.classList.add("modal-open");
    window.addEventListener("keydown", closeOnEscape);
    return () => {
      document.body.classList.remove("modal-open");
      window.removeEventListener("keydown", closeOnEscape);
    };
  }, [isOpen, isSubmitting, onClose]);

  useEffect(() => {
    if (!isOpen) {
      setCustomerId("");
      setItems([newItem()]);
    }
  }, [isOpen]);

  if (!isOpen) return null;

  function updateItem(key, field, value) {
    setItems((current) => current.map((item) => (
      item.key === key ? { ...item, [field]: value } : item
    )));
  }

  function removeItem(key) {
    setItems((current) => current.filter((item) => item.key !== key));
  }

  function handleSubmit(event) {
    event.preventDefault();
    onSubmit({
      customerId,
      items: items.map(({ productName, quantity, unitPrice }) => ({
        productName: productName.trim(),
        quantity: Number(quantity),
        unitPrice: Number(unitPrice),
      })),
    });
  }

  return (
    <div className="modal-backdrop" role="presentation" onMouseDown={(event) => {
      if (event.target === event.currentTarget && !isSubmitting) onClose();
    }}>
      <section aria-labelledby="create-order-title" aria-modal="true" className="create-modal" role="dialog">
        <header className="modal-header">
          <div>
            <p className="eyebrow">NOVO PEDIDO</p>
            <h2 id="create-order-title">Criar pedido</h2>
            <p>Informe o cliente e adicione pelo menos um item.</p>
          </div>
          <button
            aria-label="Fechar"
            className="icon-button"
            disabled={isSubmitting}
            type="button"
            onClick={onClose}
          >
            <X size={24} />
          </button>
        </header>

        <form className="create-form" onSubmit={handleSubmit}>
          <label className="field-group">
            <span>ID do cliente</span>
            <div className="customer-field-row">
              <input
                autoFocus
                placeholder="00000000-0000-0000-0000-000000000000"
                value={customerId}
                onChange={(event) => setCustomerId(event.target.value)}
                required
              />
              <button
                className="secondary-button"
                type="button"
                onClick={() => setCustomerId(crypto.randomUUID())}
              >
                Gerar UUID
              </button>
            </div>
            <small>A API identifica o cliente por UUID; não há cadastro de clientes neste desafio.</small>
          </label>

          <div className="items-editor">
            <div className="items-editor__heading">
              <h3>Itens</h3>
              <button
                className="text-button"
                type="button"
                onClick={() => setItems((current) => [...current, newItem()])}
              >
                <Plus size={17} weight="bold" />
                Adicionar item
              </button>
            </div>

            <div className="item-row item-row--header" aria-hidden="true">
              <span>Produto</span>
              <span>Quantidade</span>
              <span>Preço unitário</span>
              <span />
            </div>

            {items.map((item, index) => (
              <div className="item-row" key={item.key}>
                <label>
                  <span className="sr-only">Produto {index + 1}</span>
                  <input
                    placeholder="Ex.: Notebook"
                    value={item.productName}
                    onChange={(event) => updateItem(item.key, "productName", event.target.value)}
                    required
                  />
                </label>
                <label>
                  <span className="sr-only">Quantidade do item {index + 1}</span>
                  <input
                    min="1"
                    type="number"
                    value={item.quantity}
                    onChange={(event) => updateItem(item.key, "quantity", event.target.value)}
                    required
                  />
                </label>
                <label>
                  <span className="sr-only">Preço unitário do item {index + 1}</span>
                  <input
                    min="0.01"
                    placeholder="0,00"
                    step="0.01"
                    type="number"
                    value={item.unitPrice}
                    onChange={(event) => updateItem(item.key, "unitPrice", event.target.value)}
                    required
                  />
                </label>
                <button
                  aria-label={`Remover item ${index + 1}`}
                  className="remove-item-button icon-button"
                  disabled={items.length === 1}
                  type="button"
                  onClick={() => removeItem(item.key)}
                >
                  <Trash size={19} />
                </button>
              </div>
            ))}
          </div>

          {error && (
            <div className="inline-message inline-message--error" role="alert">
              <WarningCircle size={20} weight="fill" />
              <span>{error}</span>
            </div>
          )}

          <footer className="modal-footer">
            <div>
              <span>Total previsto</span>
              <strong>{formatCurrency(total)}</strong>
            </div>
            <span className="modal-footer__actions">
              <button className="secondary-button" disabled={isSubmitting} type="button" onClick={onClose}>
                Voltar
              </button>
              <button className="primary-button" disabled={isSubmitting} type="submit">
                {isSubmitting ? <SpinnerGap className="spin" size={19} /> : <Plus size={19} weight="bold" />}
                {isSubmitting ? "Criando..." : "Criar pedido"}
                {!isSubmitting && <ArrowRight size={18} weight="bold" />}
              </button>
            </span>
          </footer>
        </form>
      </section>
    </div>
  );
}
