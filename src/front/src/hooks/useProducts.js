import { useState, useEffect, useCallback } from 'react';
import { fetchProducts, fetchProductPrice, fetchPhotoLink } from '../api/productsApi';
import { fetchStock } from '../api/storageApi';
import { emojiForType } from '../utils/productVisuals';

export function useProducts() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const loadProducts = useCallback(async (filter = {}) => {
    setLoading(true);
    setError(null);
    try {
      const [rawProducts, stockMap] = await Promise.all([
        fetchProducts(filter),
        fetchStock().catch(() => ({})),
      ]);

      const enriched = await Promise.all(
        rawProducts.map(async (p) => {
          const [priceInfo, photoUrl] = await Promise.all([
            fetchProductPrice(p.id).catch(() => null),
            fetchPhotoLink(p.id).catch(() => null),
          ]);

          const cost = priceInfo?.cost ?? 0;
          const costWithoutDiscount = priceInfo?.costWithoutDiscount ?? cost;
          const discountPercent = priceInfo?.discount ?? (
            costWithoutDiscount > 0
              ? Math.round((1 - cost / costWithoutDiscount) * 100)
              : 0
          );

          return {
            id: p.id,
            name: p.name,
            description: p.description,
            type: p.type,
            emoji: emojiForType(p.type),
            photoUrl,
            price: cost,
            oldPrice: discountPercent > 0 ? costWithoutDiscount : null,
            discountPercent,
            stock: stockMap[p.id] ?? 0,
          };
        })
      );

      setProducts(enriched);
    } catch (err) {
      console.error('useProducts loadProducts error:', err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadProducts();
  }, [loadProducts]);

  return { products, loading, error, reload: loadProducts };
}
