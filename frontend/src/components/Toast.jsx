import { CheckCircle, X } from "@phosphor-icons/react";

export function Toast({ message, onClose }) {
  if (!message) return null;

  return (
    <div className="toast" role="status">
      <CheckCircle size={21} weight="fill" />
      <span>{message}</span>
      <button aria-label="Fechar mensagem" className="icon-button" type="button" onClick={onClose}>
        <X size={18} />
      </button>
    </div>
  );
}
