import React, { useState } from 'react';
import Catalog from './pages/Catalog';
import Cart from './pages/Cart';
import Orders from './pages/Orders';
import { useCart } from './hooks/useCart';
import { useOrders } from './hooks/useOrders';
import './App.css';

export default function App() {
  const [currentPage, setCurrentPage] = useState('catalog'); // catalog | cart | orders
  const cartHook = useCart();
  const ordersHook = useOrders();

  return (
    <div className="app">
      {/* Шапка сайта */}
      <header className="header">
        <div className="header-inner">
          <div className="header-logo">
            <span className="logo-icon">🏪</span>
            <span className="logo-text">Market</span>
          </div>

          <nav className="header-nav">
            <button
              className={`nav-link ${currentPage === 'catalog' ? 'nav-link--active' : ''}`}
              onClick={() => setCurrentPage('catalog')}
            >
              🛍 Товары
            </button>
            <button
              className={`nav-link ${currentPage === 'cart' ? 'nav-link--active' : ''}`}
              onClick={() => setCurrentPage('cart')}
            >
              🛒 Корзина
              {cartHook.totalItems > 0 && (
                <span className="nav-badge">{cartHook.totalItems}</span>
              )}
            </button>
            <button
              className={`nav-link ${currentPage === 'orders' ? 'nav-link--active' : ''}`}
              onClick={() => setCurrentPage('orders')}
            >
              📋 Заказы
              {ordersHook.orders.length > 0 && (
                <span className="nav-badge">{ordersHook.orders.length}</span>
              )}
            </button>
          </nav>
        </div>
      </header>

      {/* Контент страниц */}
      <main className="main">
        {currentPage === 'catalog' && (
          <Catalog
            cart={cartHook.cart}
            onAddToCart={cartHook.addToCart}
            onIncreaseQty={cartHook.increaseQty}
            onDecreaseQty={cartHook.decreaseQty}
          />
        )}
        {currentPage === 'cart' && (
          <Cart
            cart={cartHook.cart}
            onIncrease={cartHook.increaseQty}
            onDecrease={cartHook.decreaseQty}
            onRemove={cartHook.removeFromCart}
            onClearCart={cartHook.clearCart}
            onOrderCreated={() => setCurrentPage('orders')}
            createOrder={ordersHook.createOrder}
          />
        )}
        {currentPage === 'orders' && (
          <Orders
            orders={ordersHook.orders}
            onPay={ordersHook.payOrder}
            onCancel={ordersHook.cancelOrder}
          />
        )}
      </main>
    </div>
  );
}
