import {
  Bell,
  CaretDown,
  CaretLeft,
  CaretRight,
  MagnifyingGlass,
  Plus,
  Question,
  SignOut,
  WarningCircle,
} from "@phosphor-icons/react";
import { useCallback, useEffect, useMemo, useState } from "react";
import { ApiError, ordersApi } from "../api.js";
import { ConfirmDialog } from "./ConfirmDialog.jsx";
import { CreateOrderModal } from "./CreateOrderModal.jsx";
import { OrderDetails } from "./OrderDetails.jsx";
import { OrdersTable } from "./OrdersTable.jsx";
import { Toast } from "./Toast.jsx";

const PAGE_SIZE = 5;

export function OrdersScreen({ token, email, onLogout }) {
  const [orders, setOrders] = useState([]);
  const [page, setPage] = useState(1);
  const [pagination, setPagination] = useState({ totalCount: 0, totalPages: 0 });
  const [isLoading, setIsLoading] = useState(true);
  const [listError, setListError] = useState("");
  const [selectedId, setSelectedId] = useState(null);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [isLoadingDetails, setIsLoadingDetails] = useState(false);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [createError, setCreateError] = useState("");
  const [isConfirmOpen, setIsConfirmOpen] = useState(false);
  const [isCancelling, setIsCancelling] = useState(false);
  const [toast, setToast] = useState("");

  const handleApiError = useCallback((error, fallbackMessage) => {
    if (error instanceof ApiError && error.status === 401) {
      onLogout();
      return "Sua sessão expirou. Entre novamente.";
    }

    return error?.message || fallbackMessage;
  }, [onLogout]);

  const loadOrders = useCallback(async () => {
    setIsLoading(true);
    setListError("");
    try {
      const result = await ordersApi.list(token, page, PAGE_SIZE);
      setOrders(result.items);
      setPagination({
        totalCount: result.totalCount,
        totalPages: result.totalPages,
      });

      // A primeira seleção mantém o painel vivo como no mock, sem roubar a escolha posterior do usuário.
      setSelectedId((current) => current ?? result.items[0]?.id ?? null);
    } catch (error) {
      setListError(handleApiError(error, "Não foi possível carregar os pedidos."));
    } finally {
      setIsLoading(false);
    }
  }, [handleApiError, page, token]);

  useEffect(() => {
    loadOrders();
  }, [loadOrders]);

  useEffect(() => {
    if (!selectedId) {
      setSelectedOrder(null);
      return undefined;
    }

    let isCurrent = true;
    setIsLoadingDetails(true);

    ordersApi.getById(token, selectedId)
      .then((result) => {
        if (isCurrent) setSelectedOrder(result);
      })
      .catch((error) => {
        if (isCurrent) {
          setListError(handleApiError(error, "Não foi possível abrir o pedido."));
          setSelectedId(null);
        }
      })
      .finally(() => {
        if (isCurrent) setIsLoadingDetails(false);
      });

    return () => {
      isCurrent = false;
    };
  }, [handleApiError, selectedId, token]);

  useEffect(() => {
    if (!toast) return undefined;
    const timeout = window.setTimeout(() => setToast(""), 3500);
    return () => window.clearTimeout(timeout);
  }, [toast]);

  const visibleOrders = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();
    return orders.filter((order) => {
      const matchesStatus = statusFilter === "All" || order.status === statusFilter;
      const matchesSearch = !normalizedSearch ||
        order.id.toLowerCase().includes(normalizedSearch) ||
        order.customerId.toLowerCase().includes(normalizedSearch);
      return matchesStatus && matchesSearch;
    });
  }, [orders, search, statusFilter]);

  const pageNumbers = useMemo(() => {
    const lastPage = Math.max(1, pagination.totalPages);
    const start = Math.max(1, Math.min(page - 2, lastPage - 4));
    const end = Math.min(lastPage, start + 4);
    return Array.from({ length: end - start + 1 }, (_, index) => start + index);
  }, [page, pagination.totalPages]);

  async function createOrder(payload) {
    setIsCreating(true);
    setCreateError("");
    try {
      const created = await ordersApi.create(token, payload);
      setIsCreateOpen(false);
      setSelectedId(created.id);
      setSelectedOrder(created);
      setToast("Pedido criado com sucesso.");

      if (page !== 1) {
        setPage(1);
      } else {
        await loadOrders();
      }
    } catch (error) {
      setCreateError(handleApiError(error, "Não foi possível criar o pedido."));
    } finally {
      setIsCreating(false);
    }
  }

  async function cancelOrder() {
    if (!selectedId) return;

    setIsCancelling(true);
    try {
      await ordersApi.cancel(token, selectedId);
      setIsConfirmOpen(false);
      const refreshed = await ordersApi.getById(token, selectedId);
      setSelectedOrder(refreshed);
      setToast("Pedido cancelado com sucesso.");
      await loadOrders();
    } catch (error) {
      setIsConfirmOpen(false);
      setListError(handleApiError(error, "Não foi possível cancelar o pedido."));
    } finally {
      setIsCancelling(false);
    }
  }

  return (
    <div className="app-shell">
      <header className="app-header">
        <div className="app-header__brand">
          <img src="/assets/ntt-data-logo.png" alt="NTT DATA" />
        </div>
        <nav aria-label="Navegação principal">
          <a aria-current="page" href="#orders">Pedidos</a>
        </nav>
        <div className="app-header__actions">
          <button aria-label="Notificações" className="header-icon-button" type="button">
            <Bell size={21} />
          </button>
          <button aria-label="Ajuda" className="header-icon-button" type="button">
            <Question size={21} />
          </button>
          <span className="header-divider" aria-hidden="true" />
          <span className="user-avatar" aria-hidden="true">DM</span>
          <span className="user-email">{email}</span>
          <button aria-label="Sair" className="header-icon-button" title="Sair" type="button" onClick={onLogout}>
            <SignOut size={20} />
          </button>
        </div>
      </header>

      <main className={`orders-layout ${selectedId ? "orders-layout--details" : ""}`} id="orders">
        <section className="orders-workspace">
          <header className="workspace-heading">
            <div>
              <h1>Pedidos</h1>
              <p>Acompanhe, filtre e gerencie os pedidos com agilidade e segurança.</p>
            </div>
            <button className="primary-button" type="button" onClick={() => {
              setCreateError("");
              setIsCreateOpen(true);
            }}>
              <Plus size={20} weight="bold" />
              Novo pedido
            </button>
          </header>

          <div className="orders-toolbar">
            <label className="search-field">
              <MagnifyingGlass aria-hidden="true" size={22} />
              <span className="sr-only">Buscar pedidos</span>
              <input
                placeholder="Buscar por pedido ou cliente nesta página"
                value={search}
                onChange={(event) => setSearch(event.target.value)}
              />
            </label>
            <label className="status-filter">
              <span className="sr-only">Filtrar por status</span>
              <select value={statusFilter} onChange={(event) => setStatusFilter(event.target.value)}>
                <option value="All">Status: Todos</option>
                <option value="Pending">Status: Pendentes</option>
                <option value="Confirmed">Status: Confirmados</option>
                <option value="Cancelled">Status: Cancelados</option>
              </select>
              <CaretDown aria-hidden="true" size={18} />
            </label>
          </div>

          {listError && (
            <div className="inline-message inline-message--error list-error" role="alert">
              <WarningCircle size={20} weight="fill" />
              <span>{listError}</span>
              <button className="text-button" type="button" onClick={loadOrders}>Tentar novamente</button>
            </div>
          )}

          <OrdersTable
            isLoading={isLoading}
            items={visibleOrders}
            selectedId={selectedId}
            onCopied={setToast}
            onSelect={setSelectedId}
          />

          <footer className="pagination">
            <span>
              {pagination.totalCount === 0
                ? "Nenhum pedido"
                : `${(page - 1) * PAGE_SIZE + 1}–${Math.min(page * PAGE_SIZE, pagination.totalCount)} de ${pagination.totalCount} pedidos`}
            </span>
            <div className="pagination__buttons">
              <button
                aria-label="Página anterior"
                disabled={page === 1 || isLoading}
                type="button"
                onClick={() => setPage((current) => current - 1)}
              >
                <CaretLeft size={18} />
              </button>
              {pageNumbers.map((number) => (
                <button
                  aria-current={number === page ? "page" : undefined}
                  className={number === page ? "is-current" : ""}
                  key={number}
                  type="button"
                  onClick={() => setPage(number)}
                >
                  {number}
                </button>
              ))}
              <button
                aria-label="Próxima página"
                disabled={page >= pagination.totalPages || isLoading}
                type="button"
                onClick={() => setPage((current) => current + 1)}
              >
                <CaretRight size={18} />
              </button>
            </div>
          </footer>
        </section>

        {selectedId && (
          <OrderDetails
            isLoading={isLoadingDetails}
            order={selectedOrder}
            onClose={() => setSelectedId(null)}
            onRequestCancel={() => setIsConfirmOpen(true)}
          />
        )}
      </main>

      <CreateOrderModal
        error={createError}
        isOpen={isCreateOpen}
        isSubmitting={isCreating}
        onClose={() => setIsCreateOpen(false)}
        onSubmit={createOrder}
      />
      <ConfirmDialog
        isOpen={isConfirmOpen}
        isSubmitting={isCancelling}
        onCancel={() => setIsConfirmOpen(false)}
        onConfirm={cancelOrder}
      />
      <Toast message={toast} onClose={() => setToast("")} />
    </div>
  );
}
