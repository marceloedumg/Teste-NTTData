import { SpinnerGap, Trash, Warning } from "@phosphor-icons/react";

export function ConfirmDialog({ isOpen, isSubmitting, onCancel, onConfirm }) {
  if (!isOpen) return null;

  return (
    <div className="modal-backdrop modal-backdrop--confirm" role="presentation">
      <section aria-labelledby="confirm-title" aria-modal="true" className="confirm-dialog" role="alertdialog">
        <Warning className="confirm-dialog__icon" size={34} weight="fill" />
        <h2 id="confirm-title">Cancelar este pedido?</h2>
        <p>
          Essa alteração não pode ser desfeita. O pedido continuará disponível
          para consulta com o status Cancelado.
        </p>
        <div className="confirm-dialog__actions">
          <button className="secondary-button" disabled={isSubmitting} type="button" onClick={onCancel}>
            Manter pedido
          </button>
          <button className="danger-button" disabled={isSubmitting} type="button" onClick={onConfirm}>
            {isSubmitting ? <SpinnerGap className="spin" size={18} /> : <Trash size={18} weight="bold" />}
            {isSubmitting ? "Cancelando..." : "Sim, cancelar"}
          </button>
        </div>
      </section>
    </div>
  );
}
