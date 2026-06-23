import React, { useState, useMemo } from 'react';
import { useProducts } from '../hooks/useProducts';

export default function Catalog({ cart, onAddToCart, onIncreaseQty, onDecreaseQty }) {
  const { products, loading, error, reload } = useProducts();

  const [search, setSearch] = useState('');
  const [priceFrom, setPriceFrom] = useState('');
  const [priceTo, setPriceTo] = useState('');

  const filteredProducts = useMemo(() => {
    return products.filter(p => {
      const matchSearch = p.name?.toLowerCase().includes(search.toLowerCase());
      const matchFrom = !priceFrom || p.price >= Number(priceFrom);
      const matchTo = !priceTo || p.price <= Number(priceTo);
      return matchSearch && matchFrom && matchTo;
    });
  }, [products, search, priceFrom, priceTo]);

  const hasActiveFilters = search || priceFrom || priceTo;

  if (loading) {
    return (
      <div className="empty-state">
        <div className="empty-state__icon">⏳</div>
        <div className="empty-state__title">Загружаем каталог...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="empty-state">
        <div className="empty-state__icon">⚠️</div>
        <div className="empty-state__title">Не удалось загрузить товары</div>
        <div className="empty-state__text">{error}</div>
        <button className="btn btn--primary" style={{ marginTop: '12px' }} onClick={() => reload()}>
          Повторить
        </button>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', gap: '24px', alignItems: 'flex-start' }}>
      {/* Боковая панель с фильтрами */}
      <aside className="filters-sidebar">
        <div className="filters-section">
          <div className="filters-label">Цена, ₽</div>
          <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
            <input
              className="input"
              placeholder="От"
              type="number"
              value={priceFrom}
              onChange={e => setPriceFrom(e.target.value)}
              style={{ textAlign: 'center' }}
            />
            <span style={{ color: 'var(--gray-400)', flexShrink: 0 }}>—</span>
            <input
              className="input"
              placeholder="До"
              type="number"
              value={priceTo}
              onChange={e => setPriceTo(e.target.value)}
              style={{ textAlign: 'center' }}
            />
          </div>
        </div>

        {hasActiveFilters && (
          <button
            className="btn btn--ghost btn--sm btn--full"
            style={{ marginTop: '12px' }}
            onClick={() => {
              setSearch('');
              setPriceFrom('');
              setPriceTo('');
            }}
          >
            Сбросить фильтры
          </button>
        )}
      </aside>

      {/* Правая часть — поиск + сетка товаров */}
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ marginBottom: '20px' }}>
          <div style={{ position: 'relative' }}>
            <span style={{
              position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)',
              fontSize: '16px', pointerEvents: 'none',
            }}>🔍</span>
            <input
              className="input"
              placeholder="Поиск товаров..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              style={{ paddingLeft: '38px' }}
            />
          </div>
        </div>

        {filteredProducts.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state__icon">🔍</div>
            <div className="empty-state__title">Ничего не найдено</div>
            <div className="empty-state__text">Попробуйте изменить фильтры</div>
          </div>
        ) : (
          <div className="products-grid">
            {filteredProducts.map(product => {
              const qtyInCart = cart[product.id] || 0;
              const isOutOfStock = product.stock === 0;
              const isLowStock = product.stock > 0 && product.stock <= 5;

              return (
                <div key={product.id} className="card product-card">
                  {/* Картинка — фото с бэкенда, либо эмодзи-заглушка */}
                  <div className="product-img-wrap">
                    {product.photoUrl ? (
                      <img
                        src={product.photoUrl}
                        alt={product.name}
                        style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                        onError={e => { e.target.style.display = 'none'; }}
                      />
                    ) : (
                      <div className="product-emoji">{product.emoji}</div>
                    )}
                  </div>

                  <div className="product-info">
                    <div className="product-name">{product.name}</div>

                    {/* Цена */}
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px' }}>
                      <span className="price">{product.price.toLocaleString('ru-RU')} ₽</span>
                    </div>

                    {/* Наличие */}
                    <div style={{
                      fontSize: '12px',
                      marginBottom: '12px',
                      flex: 1,
                      color: isOutOfStock ? 'var(--gray-400)' : isLowStock ? '#b06000' : 'var(--green)',
                    }}>
                      {isOutOfStock
                        ? '❌ Нет в наличии'
                        : isLowStock
                          ? `⚠️ Осталось ${product.stock} шт.`
                          : `✅ В наличии: ${product.stock} шт.`}
                    </div>

                    {/* Кнопка / счётчик количества */}
                    <div className="product-action">
                      {isOutOfStock ? (
                        <button className="btn btn--ghost btn--full" disabled>
                          Нет в наличии
                        </button>
                      ) : qtyInCart === 0 ? (
                        <button
                          className="btn btn--primary btn--full"
                          onClick={() => onAddToCart(product.id)}
                        >
                          В корзину
                        </button>
                      ) : (
                        <div className="product-qty-row">
                          <button className="qty-btn" onClick={() => onDecreaseQty(product.id)}>−</button>
                          <span className="product-qty-val">{qtyInCart} шт.</span>
                          <button className="qty-btn" onClick={() => onIncreaseQty(product.id)}>+</button>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>

      <style>{`
        .filters-sidebar {
          width: 220px;
          flex-shrink: 0;
          background: white;
          border-radius: 12px;
          border: 1px solid var(--gray-200);
          padding: 16px;
          position: sticky;
          top: 80px;
        }
        .filters-label {
          font-size: 11px;
          font-weight: 700;
          color: var(--gray-500);
          margin-bottom: 10px;
          text-transform: uppercase;
          letter-spacing: 0.8px;
        }
        .filters-section {
          margin-bottom: 4px;
        }

        .products-grid {
          display: grid;
          grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
          gap: 16px;
        }
        .product-card {
          display: flex;
          flex-direction: column;
        }
        .product-img-wrap {
          background: var(--blue-light);
          height: 160px;
          display: flex;
          align-items: center;
          justify-content: center;
          position: relative;
          flex-shrink: 0;
          overflow: hidden;
        }
        .product-emoji {
          font-size: 64px;
        }
        .product-info {
          padding: 14px;
          flex: 1;
          display: flex;
          flex-direction: column;
          justify-content: flex-start;
        }
        .product-name {
          font-size: 14px;
          font-weight: 500;
          line-height: 1.4;
          margin-bottom: 8px;
          color: var(--gray-900);
          min-height: 40px;
        }
        .product-action {
          margin-top: auto;
          height: 38px;
          display: flex;
          align-items: stretch;
        }
        .product-action .btn {
          height: 38px;
          padding-top: 0;
          padding-bottom: 0;
        }
        .product-qty-row {
          display: flex;
          align-items: center;
          justify-content: space-between;
          width: 100%;
          height: 38px;
        }
        .product-qty-row .qty-btn {
          width: 38px;
          height: 38px;
          flex-shrink: 0;
        }
        .product-qty-val {
          font-weight: 600;
          font-size: 14px;
          text-align: center;
          flex: 1;
        }
      `}</style>
    </div>
  );
}
