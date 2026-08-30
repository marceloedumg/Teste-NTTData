import { CheckCircle, Clock, XCircle } from "@phosphor-icons/react";
import { statusLabel, statusTone } from "../formatters.js";

const icons = {
  pending: Clock,
  confirmed: CheckCircle,
  cancelled: XCircle,
};

export function StatusBadge({ status }) {
  const tone = statusTone(status);
  const Icon = icons[tone];

  return (
    <span className={`status-badge status-badge--${tone}`}>
      <Icon aria-hidden="true" size={14} weight="fill" />
      {statusLabel(status)}
    </span>
  );
}
