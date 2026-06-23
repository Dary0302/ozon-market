import { API_BASE } from '../config/api';

// GET /api/pvz/all — список пунктов выдачи заказов
export async function fetchPvzList() {
  const response = await fetch(`${API_BASE.STORAGE}/api/pvz/all`);
  if (!response.ok) {
    throw new Error(`Не удалось загрузить список ПВЗ: ${response.status}`);
  }
  return response.json(); // [{ id, address, pointId }]
}

// GET /api/stored-products/stock — суммарные остатки по всем товарам на всех складах
export async function fetchStock() {
  const response = await fetch(`${API_BASE.STORAGE}/api/stored-products/stock`);
  if (!response.ok) {
    throw new Error(`Не удалось загрузить остатки: ${response.status}`);
  }
  const data = await response.json();
  const stockMap = {};
  data.forEach(item => {
    stockMap[item.productId] = item.quantity;
  });
  return stockMap;
}

// POST /api/stored-products/{pvzId}/date — расчётная дата доставки для списка товаров
export async function fetchDeliveryDate(pvzId, items) {
  const response = await fetch(`${API_BASE.STORAGE}/api/stored-products/${pvzId}/date`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(items),
  });
  if (!response.ok) {
    throw new Error(`Не удалось рассчитать дату доставки: ${response.status}`);
  }
  return response.json();
}
