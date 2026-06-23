import React, { useState, useEffect, useMemo } from 'react';
import { fetchPvzList, fetchDeliveryDate } from '../api/storageApi';
import { fetchProducts, fetchProductPrice, fetchPhotoLink } from '../api/productsApi';

function useCartProducts(cart) {
  const [productsById, setProductsById] = useState({});
  const [loading, setLoading] = useState(true);

  const ids = useMemo(() => Object.keys(cart), [cart]);

  useEffect(() => {
    let cancelled = false;
    if (ids.length === 0) {
      setProductsById({});
      setLoading(false);
      return;
    }
    setLoading(true);

    fetchProducts({ page: 1, pageSize: 500 })
      .then(async (all) => {
        const needed = all.filter(p => ids.includes(p.id));
        const enriched = await Promise.all(
          needed.map(async (p) => {
            const [price, photoUrl] = await Promise.all([
              fetchProductPrice(p.id).catch(() => 0),
              fetchPhotoLink(p.photoId).catch(() => null),
            ]);
            return { ...p, price: price ?? 0, photoUrl };
          })
        );
        if (!cancelled) {
          const map = {};
          enriched.forEach(p => { map[p.id] = p; });
          setProductsById(map);
        }
      })
      .catch(err => console.error('Не удалось загрузить товары корзины:', err))
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => { cancelled = true; };
  }, [ids]);

  return { productsById, loading };
}

export default function Cart({ cart, onIncrease, onDecrease, onRemove, onClearCart, onOrderCreated, createOrder }) {
  const { productsById, loading: productsLoading } = useCartProducts(cart);

  const [selectedPvzId, setSelectedPvzId] = useState('');
  const [deliveryDate, setDeliveryDate] = useState(null);
  const [deliveryLoading, setDeliveryLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [pvzList, setPvzList] = useState([]);
  const [pvzLoading, setPvzLoading] = useState(true);
  const [pvzError, setPvzError] = useState('');

  useEffect(() => {
    let cancelled = false;
    setPvzLoading(true);
    fetchPvzList()
      .then(data => {
        if (!cancelled) {
          setPvzList(data);
          setPvzError('');
        }
      })
      .catch(err => {
        if (!cancelled) {
          console.error('Не удалось загрузить ПВЗ:', err);
          setPvzError('Не удалось загрузить список ПВЗ с сервера.');
        }
      })
      .finally(() => {
        if (!cancelled) setPvzLoading(false);
      });
    return () => { cancelled = true; };
  }, []);

  // Товары из корзины, обогащённые данными с бэкенда
  const cartItems = Object.entries(cart)
    .map(([id, qty]) => ({ product: productsById[id], qty }))
    .filter(item => item.product);

  const totalItems = cartItems.reduce((sum, item) => sum + item.qty, 0);
  const totalPrice = cartItems.reduce((sum, item) => sum + item.product.price * item.qty, 0);

  // Когда выбран ПВЗ — запрашиваем у StorageService реальную дату доставки
  const handlePvzChange = async (pvzId) => {
    setSelectedPvzId(pvzId);
    setDeliveryDate(null);
    if (!pvzId || cartItems.length === 0) return;

    setDeliveryLoading(true);
    try {
      const items = cartItems.map(item => ({
        productId: item.product.id,
        quantity: item.qty,
      }));
      const date = await fetchDeliveryDate(pvzId, items);
      setDeliveryDate(date);
    } catch (err) {
      console.error('Не удалось рассчитать дату доставки:', err);
      setDeliveryDate(null);
    } finally {
      setDeliveryLoading(false);
    }
  };

  const handleCreateOrder = async () => {
    setError('');

    if (!selectedPvzId) {
      setError('Выберите пункт выдачи заказа');
      return;
    }

    const shortages = cartItems.filter(item => item.qty > item.product.stock);
    if (shortages.length > 0) {
      const list = shortages.map(s =>
        `«${s.product.name}»: нужно ${s.qty} шт., в наличии ${s.product.stock ?? 0} шт.`
      ).join('\n');
      setError(`Недостаточно товара на складе:\n${list}`);
      return;
    }

    const pvz = pvzList.find(p => p.id === selectedPvzId);
    const pvzObj = {
      id: pvz?.id ?? selectedPvzId,
      name: pvz?.address ?? `ПВЗ ${selectedPvzId}`,
    };

    const orderItems = cartItems.map(item => ({
      id: item.product.id,
      name: item.product.name,
      emoji: item.product.emoji,
      price: item.product.price,
      qty: item.qty,
    }));

    try {
      await createOrder(orderItems, pvzObj, totalPrice, null);
      onClearCart();
      setSuccess('Заказ успешно создан!');
      setTimeout(() => {
        onOrderCreated();
      }, 1200);
    } catch (err) {
      setError(`Ошибка создания заказа: ${err.message}`);
    }
  };

  if (productsLoading) {
    return (
      <div className="empty-state">
        <div className="empty-state__icon">⏳</div>
        <div className="empty-state__title">Загружаем корзину...</div>
      </div>
    );
  }

  if (cartItems.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-state__icon">🛒</div>
        <div className="empty-state__title">Корзина пуста</div>
        <div className="empty-state__text">Добавьте товары из каталога</div>
      </div>
    );
  }

  return (
    <div>
      <div className="section-title">🛒 Корзина</div>

      {error && (
        <div className="alert alert--error" style={{ whiteSpace: 'pre-line' }}>
          ❌ {error}
        </div>
      )}
      {success && (
        <div className="alert alert--success">
          ✅ {success} Переходим к заказам...
        </div>
      )}

      <div className="cart-layout">
        {/* Список товаров */}
        <div className="cart-items">
          {cartItems.map(({ product, qty }) => (
            <div key={product.id} className="cart-item">
              {/* Картинка */}
              <div className="cart-item-img">
                {product.photoUrl ? (
                  <img
                    src={product.photoUrl}
                    alt={product.name}
                    style={{ width: '100%', height: '100%', objectFit: 'cover', borderRadius: '10px' }}
                    onError={e => { e.target.style.display = 'none'; }}
                  />
                ) : (
                  <span style={{ fontSize: '36px' }}>{product.emoji}</span>
                )}
              </div>

              {/* Инфо */}
              <div className="cart-item-info">
                <div className="cart-item-name">{product.name}</div>
                <div style={{ fontSize: '14px', color: 'var(--gray-500)' }}>
                  {product.price.toLocaleString('ru-RU')} ₽ / шт.
                </div>
              </div>

              {/* Количество */}
              <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                <button className="qty-btn qty-btn--outline" onClick={() => onDecrease(product.id)} style={{ width: '32px', height: '32px' }}>−</button>
                <span style={{ fontWeight: 600, fontSize: '15px', minWidth: '24px', textAlign: 'center' }}>{qty}</span>
                <button className="qty-btn" onClick={() => onIncrease(product.id)} style={{ width: '32px', height: '32px' }}>+</button>
              </div>

              {/* Сумма и удалить */}
              <div style={{ textAlign: 'right', minWidth: '90px' }}>
                <div style={{ fontWeight: 700, fontSize: '15px', marginBottom: '6px' }}>
                  {(product.price * qty).toLocaleString('ru-RU')} ₽
                </div>
                <button
                  className="btn btn--danger btn--sm"
                  onClick={() => onRemove(product.id)}
                >
                  🗑 Убрать
                </button>
              </div>
            </div>
          ))}
        </div>

        {/* Оформление */}
        <div className="cart-summary">
          <div style={{ fontWeight: 600, fontSize: '16px', marginBottom: '16px' }}>Оформление</div>

          <div className="summary-row">
            <span>Товаров</span>
            <span>{totalItems} шт.</span>
          </div>
          <div className="summary-row">
            <span>Доставка</span>
            <span style={{ color: 'var(--green)' }}>Бесплатно</span>
          </div>
          <div className="divider" />
          <div className="summary-row" style={{ fontWeight: 700, fontSize: '16px' }}>
            <span>Итого</span>
            <span>{totalPrice.toLocaleString('ru-RU')} ₽</span>
          </div>

          <div className="divider" />

          {/* Выбор ПВЗ */}
          <div style={{ marginBottom: '8px', fontWeight: 500, fontSize: '13px' }}>
            📍 Пункт выдачи
          </div>

          {pvzError && (
            <div className="alert alert--error" style={{ marginBottom: '8px', fontSize: '12px' }}>
              ⚠️ {pvzError}
            </div>
          )}

          <select
            className="select"
            value={selectedPvzId}
            onChange={e => handlePvzChange(e.target.value)}
            style={{ marginBottom: '12px' }}
            disabled={pvzLoading}
          >
            <option value="">
              {pvzLoading ? '⏳ Загрузка ПВЗ...' : '— Выберите ПВЗ —'}
            </option>
            {pvzList.map(pvz => (
              <option key={pvz.id} value={pvz.id}>{pvz.address}</option>
            ))}
          </select>

          {/* Дата доставки */}
          {deliveryLoading && (
            <div className="alert alert--info" style={{ marginBottom: '12px' }}>
              ⏳ Рассчитываем дату доставки...
            </div>
          )}
          {deliveryDate && !deliveryLoading && (
            <div className="alert alert--info" style={{ marginBottom: '12px' }}>
              🚚 Дата доставки: <strong>{new Date(deliveryDate).toLocaleDateString('ru-RU')}</strong>
            </div>
          )}

          <button
            className="btn btn--primary btn--full btn--lg"
            onClick={handleCreateOrder}
            disabled={!selectedPvzId || deliveryLoading || pvzLoading || !!success}
          >
            ✅ Создать заказ
          </button>
        </div>
      </div>

      <style>{`
        .cart-layout {
          display: grid;
          grid-template-columns: 1fr 300px;
          gap: 24px;
          align-items: start;
        }
        .cart-items {
          display: flex;
          flex-direction: column;
          gap: 12px;
        }
        .cart-item {
          background: white;
          border: 1px solid var(--gray-200);
          border-radius: 12px;
          padding: 16px;
          display: grid;
          grid-template-columns: 72px 1fr auto auto;
          gap: 16px;
          align-items: center;
          transition: border-color 0.15s;
        }
        .cart-item:hover {
          border-color: var(--blue-mid);
        }
        .cart-item-img {
          width: 72px;
          height: 72px;
          background: var(--blue-light);
          border-radius: 10px;
          display: flex;
          align-items: center;
          justify-content: center;
          flex-shrink: 0;
          overflow: hidden;
        }
        .cart-item-info {
          min-width: 0;
        }
        .cart-item-name {
          font-size: 14px;
          font-weight: 500;
          margin-bottom: 4px;
          line-height: 1.4;
        }
        .cart-summary {
          background: white;
          border: 1px solid var(--gray-200);
          border-radius: 12px;
          padding: 20px;
          position: sticky;
          top: 80px;
        }
        .summary-row {
          display: flex;
          justify-content: space-between;
          font-size: 14px;
          color: var(--gray-700);
          margin-bottom: 10px;
        }
      `}</style>
    </div>
  );
}
