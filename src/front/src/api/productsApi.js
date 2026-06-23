import { API_BASE } from '../config/api';

// POST /api/products — список товаров по фильтру (ProductFilter)
export async function fetchProducts(filter = {}) {
  const body = {
    name: filter.name || null,
    types: filter.types || null,
    minPrice: filter.minPrice ?? null,
    maxPrice: filter.maxPrice ?? null,
    hasDiscount: filter.hasDiscount ?? null,
    minDiscount: filter.minDiscount ?? null,
    maxDiscount: filter.maxDiscount ?? null,
    page: filter.page ?? 1,
    pageSize: filter.pageSize ?? 100,
  };

  const response = await fetch(`${API_BASE.PRODUCT}/api/products`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw new Error(`Не удалось загрузить товары: ${response.status}`);
  }
  return response.json();
}

// GET /api/products/prices/{product-id} — текущая цена товара (число)
export async function fetchProductPrice(productId) {
  const response = await fetch(`${API_BASE.PRODUCT}/api/products/prices/${productId}`);
  if (!response.ok) return null;
  return response.json(); // число
}

// POST /api/products/prices/amount — сумма цен по списку товаров (для пересчёта корзины)
export async function fetchPricesAmount(items) {
  // items: [{ productId, quantity }]
  const response = await fetch(`${API_BASE.PRODUCT}/api/products/prices/amount`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(items),
  });
  if (!response.ok) {
    throw new Error(`Не удалось посчитать сумму: ${response.status}`);
  }
  return response.json(); // число
}

// GET /api/photos/link/{photo-id} — ссылка на скачивание фото
export async function fetchPhotoLink(photoId) {
  if (!photoId) return null;
  try {
    const response = await fetch(`${API_BASE.PRODUCT}/api/photos/link/${photoId}`);
    if (!response.ok) return null;
    const data = await response.json();
    return data.downloadPath || null;
  } catch {
    return null;
  }
}
