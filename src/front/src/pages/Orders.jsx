import React, { useState } from 'react';

const STATUS_CONFIG = {
  Created:                { label: 'Заказ создан',       class: 'status-badge--created',    icon: '📝' },
  Paid:                   { label: 'Оплачен',            class: 'status-badge--paid',       icon: '💳' },
  InAssembly:             { label: 'Собирается',         class: 'status-badge--assembling', icon: '📦' },
  TransferredForDelivery: { label: 'Передан в доставку', class: 'status-badge--delivery',   icon: '🚚' },
  Delivered:              { label: 'Доставлен',          class: 'status-badge--delivered',  icon: '✅' },
  Canceled:              { label: 'Отменён',            class: 'status-badge--canceled',  icon: '❌' },
};

function getStatusConfig(status) {
  return STATUS_CONFIG[status] || { label: status || 'Неизвестно', class: 'status-badge--created', icon: 'ℹ️' };
}

const CANCELLABLE_STATUSES = new Set(['Created', 'Paid', 'InAssembly', 'TransferredForDelivery']);

// Одна карточка заказа
function OrderCard({ order, onPay, onCancel }) {
  const [isOpen, setIsOpen] = useState(false);
  const [confirmCancel, setConfirmCancel] = useState(false);
  const config = getStatusConfig(order.status);
  const canPay = order.status === 'Created';
  const canCancel = CANCELLABLE_STATUSES.has(order.status);

  return (
    <div className="order-card">
      {/* Шапка карточки — всегда видна */}
      <div className="order-header" onClick={() => setIsOpen(!isOpen)}>
        <div className="order-header-main">
          <div>
            <div className="order-id">{order.id}</div>
            <div style={{ fontSize: '12px', color: 'var(--gray-500)', marginTop: '2px' }}>
              {order.date} · 📍 {order.pvz}
            </div>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            <span style={{ fontWeight: 700, color: 'var(--blue)', fontSize: '15px' }}>
              {order.total.toLocaleString('ru-RU')} ₽
            </span>
            <span className={`status-badge ${config.class}`}>
              {config.icon} {config.label}
            </span>
            <span className="order-chevron" style={{ transform: isOpen ? 'rotate(180deg)' : 'rotate(0deg)' }}>
              ▼
            </span>
          </div>
        </div>

        {/* Превью товаров в свёрнутом виде */}
        {!isOpen && (
          <div style={{ fontSize: '13px', color: 'var(--gray-500)', marginTop: '6px' }}>
            {order.items.slice(0, 3).map(item => `${item.emoji} ${item.name} ×${item.qty}`).join(' · ')}
            {order.items.length > 3 && ` и ещё ${order.items.length - 3}...`}
          </div>
        )}
      </div>

      {/* Раскрытое тело карточки */}
      {isOpen && (
        <div className="order-body">
          {/* Состав заказа */}
          <div style={{ fontWeight: 600, fontSize: '14px', marginBottom: '12px' }}>
            📋 Состав заказа
          </div>
          <div className="order-items">
            {order.items.map(item => (
              <div key={item.id} className="order-item-row">
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                  <div className="order-item-img">
                    {item.photoUrl ? (
                      <img
                        src={item.photoUrl}
                        alt={item.name}
                        style={{ width: '100%', height: '100%', objectFit: 'cover', borderRadius: '8px' }}
                        onError={e => { e.target.style.display = 'none'; }}
                      />
                    ) : (
                      <span style={{ fontSize: '24px' }}>{item.emoji}</span>
                    )}
                  </div>
                  <div>
                    <div style={{ fontWeight: 500, fontSize: '14px' }}>{item.name}</div>
                    <div style={{ fontSize: '12px', color: 'var(--gray-500)' }}>
                      {item.price.toLocaleString('ru-RU')} ₽ / шт.
                    </div>
                  </div>
                </div>
                <div style={{ textAlign: 'right', flexShrink: 0 }}>
                  <div style={{ fontSize: '12px', color: 'var(--gray-500)', marginBottom: '2px' }}>
                    {item.qty} шт.
                  </div>
                  <div style={{ fontWeight: 600, color: 'var(--blue)' }}>
                    {(item.price * item.qty).toLocaleString('ru-RU')} ₽
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Итог */}
          <div className="divider" />
          <div style={{ display: 'flex', justifyContent: 'space-between', fontWeight: 700, fontSize: '15px', marginBottom: '16px' }}>
            <span>Итого</span>
            <span>{order.total.toLocaleString('ru-RU')} ₽</span>
          </div>

          {/* Диалог подтверждения отмены */}
          {confirmCancel ? (
            <div className="alert alert--error" style={{ flexDirection: 'column', gap: '12px' }}>
              <div>❓ Вы уверены, что хотите отменить заказ? Товары вернутся на склад.</div>
              <div style={{ display: 'flex', gap: '8px' }}>
                <button className="btn btn--danger btn--sm" onClick={() => { onCancel(order.id); setConfirmCancel(false); }}>
                  Да, отменить
                </button>
                <button className="btn btn--ghost btn--sm" onClick={() => setConfirmCancel(false)}>
                  Нет, оставить
                </button>
              </div>
            </div>
          ) : (
            <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
              {canPay && (
                <button className="btn btn--primary" onClick={() => onPay(order.id)}>
                  💳 Оплатить заказ
                </button>
              )}
              {canCancel && (
                <button className="btn btn--danger" onClick={() => setConfirmCancel(true)}>
                  ✕ Отменить заказ
                </button>
              )}
              <button className="btn btn--ghost" style={{ marginLeft: 'auto' }} onClick={() => setIsOpen(false)}>
                Свернуть ▲
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

// Главный компонент страницы заказов
export default function Orders({ orders, onPay, onCancel }) {
  if (orders.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-state__icon">📋</div>
        <div className="empty-state__title">Заказов пока нет</div>
        <div className="empty-state__text">Оформите первый заказ в каталоге</div>
      </div>
    );
  }

  return (
    <div>
      <div className="section-title">📋 Мои заказы</div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
        {orders.map(order => (
          <OrderCard
            key={order.id}
            order={order}
            onPay={onPay}
            onCancel={onCancel}
          />
        ))}
      </div>

      <style>{`
        .order-card {
          background: white;
          border: 1px solid var(--gray-200);
          border-radius: 12px;
          overflow: hidden;
          transition: border-color 0.15s, box-shadow 0.15s;
        }
        .order-card:hover {
          border-color: var(--blue-mid);
          box-shadow: 0 2px 8px rgba(26,115,232,0.1);
        }
        .order-header {
          padding: 16px 20px;
          cursor: pointer;
          user-select: none;
        }
        .order-header:hover {
          background: var(--gray-50);
        }
        .order-header-main {
          display: flex;
          justify-content: space-between;
          align-items: center;
          gap: 12px;
        }
        .order-id {
          font-size: 14px;
          font-weight: 700;
          color: var(--gray-900);
          font-family: monospace;
        }
        .order-chevron {
          font-size: 12px;
          color: var(--gray-400);
          transition: transform 0.2s ease;
          display: inline-block;
        }
        .order-body {
          border-top: 1px solid var(--gray-200);
          padding: 16px 20px;
        }
        .order-items {
          display: flex;
          flex-direction: column;
          gap: 8px;
          margin-bottom: 8px;
        }
        .order-item-row {
          display: flex;
          justify-content: space-between;
          align-items: center;
          padding: 10px 12px;
          background: var(--gray-50);
          border-radius: 8px;
          gap: 12px;
        }
        .order-item-img {
          width: 44px;
          height: 44px;
          background: var(--blue-light);
          border-radius: 8px;
          display: flex;
          align-items: center;
          justify-content: center;
          flex-shrink: 0;
          overflow: hidden;
        }
      `}</style>
    </div>
  );
}