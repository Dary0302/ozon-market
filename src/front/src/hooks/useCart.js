import { useState, useEffect } from 'react';

export function useCart() {
  const [cart, setCart] = useState(() => {
    try {
      const saved = localStorage.getItem('market_cart');
      return saved ? JSON.parse(saved) : {};
    } catch {
      return {};
    }
  });

  useEffect(() => {
    localStorage.setItem('market_cart', JSON.stringify(cart));
  }, [cart]);

  const addToCart = (productId) => {
    setCart(prev => ({
      ...prev,
      [productId]: (prev[productId] || 0) + 1,
    }));
  };

  const decreaseQty = (productId) => {
    setCart(prev => {
      const current = prev[productId] || 0;
      if (current <= 1) {
        const { [productId]: _, ...rest } = prev;
        return rest;
      }
      return { ...prev, [productId]: current - 1 };
    });
  };

  const increaseQty = (productId) => {
    setCart(prev => ({
      ...prev,
      [productId]: (prev[productId] || 0) + 1,
    }));
  };

  const removeFromCart = (productId) => {
    setCart(prev => {
      const { [productId]: _, ...rest } = prev;
      return rest;
    });
  };

  const clearCart = () => setCart({});

  const totalItems = Object.values(cart).reduce((sum, qty) => sum + qty, 0);

  return {
    cart,
    addToCart,
    decreaseQty,
    increaseQty,
    removeFromCart,
    clearCart,
    totalItems,
  };
}
