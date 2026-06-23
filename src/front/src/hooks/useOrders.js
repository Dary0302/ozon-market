import { useState, useEffect, useRef, useCallback } from 'react';
import { API_BASE } from '../config/api';

const STATUS_MAP = {
  0: 'created',
  1: 'paid',
  2: 'assembling',
  3: 'delivery',
  4: 'delivered',
  5: 'cancelled',
  6: 'returned',
  7: 'ready_for_pickup',
  'Created': 'created',
  'Paid': 'paid',
  'Assembling': 'assembling',
  'Delivery': 'delivery',
  'Delivered': 'delivered',
  'Cancelled': 'cancelled',
  'Returned': 'returned',
  'ReadyForPickup': 'ready_for_pickup',
  'Processing': 'assembling',
  'InDelivery': 'delivery',
};

const ACTIVE_STATUSES = new Set(['created', 'paid', 'assembling', 'delivery', 'ready_for_pickup']);

function mapBackendOrder(backendOrder) {
  const status = STATUS_MAP[backendOrder.status] ?? backendOrder.status?.toLowerCase() ?? 'created';
  return {
    id: backendOrder.id,
    date: backendOrder.createdOn
      ? new Date(backendOrder.createdOn).toLocaleString('ru-RU')
      : new Date().toLocaleString('ru-RU'),
    status,
    pvzId: backendOrder.pvzId,
    pvz: backendOrder.pvzId,
    total: backendOrder.amount,
    deliveryDate: backendOrder.deliveryDate,
    items: (backendOrder.products || []).map(p => ({
      id: p.productId,
      name: `Товар ${p.productId.slice(0, 8)}...`,
      emoji: '📦',
      price: p.price,
      qty: p.quantity,
    })),
  };
}

export function useOrders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const pollingRef = useRef(null);

  // Загрузка всех заказов с бэкенда
  const fetchAllOrders = useCallback(async () => {
    try {
      const response = await fetch(
        `${API_BASE.ORDER}/api/orders/details?pageNumber=1&pageSize=100`
      );
      if (!response.ok) {
        throw new Error(`Ошибка загрузки заказов: ${response.status}`);
      }
      const data = await response.json();
      const items = data.items ?? data ?? [];
      setOrders(items.map(mapBackendOrder));
    } catch (err) {
      console.error('fetchAllOrders error:', err);
      setError(err.message);
    }
  }, []);

  // Загрузка конкретного заказа по id (для поллинга статуса)
  const fetchOrder = useCallback(async (orderId) => {
    try {
      const response = await fetch(`${API_BASE.ORDER}/api/orders/${orderId}/details`);
      if (!response.ok) return null;
      const data = await response.json();
      return mapBackendOrder(data);
    } catch (err) {
      console.error('fetchOrder error:', err);
      return null;
    }
  }, []);

  // Обновить один заказ в локальном стейте
  const updateOrderInState = useCallback((updatedOrder) => {
    setOrders(prev =>
      prev.map(o => (o.id === updatedOrder.id ? updatedOrder : o))
    );
  }, []);

  // При монтировании — загружаем все заказы
  useEffect(() => {
    fetchAllOrders();
  }, [fetchAllOrders]);

  // Поллинг: обновляем статусы активных заказов каждые 3 секунды
  useEffect(() => {
    const activeOrders = orders.filter(o => ACTIVE_STATUSES.has(o.status));

    if (activeOrders.length === 0) {
      if (pollingRef.current) {
        clearInterval(pollingRef.current);
        pollingRef.current = null;
      }
      return;
    }

    if (!pollingRef.current) {
      pollingRef.current = setInterval(async () => {
        await fetchAllOrders();
      }, 3000);
    }

    return () => {
      if (pollingRef.current) {
        clearInterval(pollingRef.current);
        pollingRef.current = null;
      }
    };
  }, [orders, fetchAllOrders]);

  // Создать заказ
  const createOrder = async (items, pvz, total, deliveryDays) => {
    setLoading(true);
    setError(null);
    try {
      // item.id здесь — реальный UUID товара, полученный от ProductService,
      // поэтому никакого преобразования не требуется.
      const body = {
        pvzId: pvz.id,
        clientAmount: total,
        products: items.map(item => ({
          productId: item.id,
          quantity: item.qty,
        })),
      };

      const response = await fetch(`${API_BASE.ORDER}/api/orders`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const errText = await response.text();
        throw new Error(`Не удалось создать заказ: ${response.status} ${errText}`);
      }

      const newOrderId = await response.json();

      const optimisticOrder = {
        id: newOrderId,
        date: new Date().toLocaleString('ru-RU'),
        status: 'created',
        pvzId: pvz.id,
        pvz: pvz.name,
        total,
        deliveryDays,
        items: items.map(item => ({
          id: item.id,
          name: item.name,
          emoji: item.emoji,
          price: item.price,
          qty: item.qty,
        })),
      };

      setOrders(prev => [optimisticOrder, ...prev]);

      setTimeout(fetchAllOrders, 1000);

      return optimisticOrder;
    } catch (err) {
      setError(err.message);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  // Оплатить заказ
  const payOrder = async (orderId) => {
    setError(null);
    try {
      const response = await fetch(`${API_BASE.ORDER}/api/orders/${orderId}/pay`, {
        method: 'PATCH',
      });
      if (!response.ok) {
        const errText = await response.text();
        throw new Error(`Не удалось оплатить заказ: ${response.status} ${errText}`);
      }
      setOrders(prev =>
        prev.map(o => (o.id === orderId ? { ...o, status: 'paid' } : o))
      );
      setTimeout(fetchAllOrders, 2000);
    } catch (err) {
      setError(err.message);
      throw err;
    }
  };

  // Отменить заказ
  const cancelOrder = async (orderId) => {
    setError(null);
    try {
      const response = await fetch(`${API_BASE.ORDER}/api/orders/${orderId}/cancel`, {
        method: 'PATCH',
      });
      if (!response.ok) {
        const errText = await response.text();
        throw new Error(`Не удалось отменить заказ: ${response.status} ${errText}`);
      }
      setOrders(prev =>
        prev.map(o => (o.id === orderId ? { ...o, status: 'cancelled' } : o))
      );
      setTimeout(fetchAllOrders, 1000);
    } catch (err) {
      setError(err.message);
      throw err;
    }
  };

  return {
    orders,
    loading,
    error,
    createOrder,
    payOrder,
    cancelOrder,
    refetch: fetchAllOrders,
  };
}