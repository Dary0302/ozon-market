import { useState, useEffect, useCallback } from 'react';
import { fetchProducts, fetchProductPrice, fetchPhotoLink } from '../api/productsApi';
import { fetchStock } from '../api/storageApi';

const TYPE_EMOJI = ['📦', '📱', '👟', '🍎', '🏠', '🎧', '⚡'];

function emojiForType(type) {
  return TYPE_EMOJI[type % TYPE_EMOJI.length] || '📦';
}

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
          const [price, photoUrl] = await Promise.all([
            fetchProductPrice(p.id).catch(() => null),
            fetchPhotoLink(p.photoId).catch(() => null),
          ]);

          return {
            id: p.id,
            name: p.name,
            description: p.description,
            type: p.type,
            emoji: emojiForType(p.type ?? 0),
            photoUrl,
            price: price ?? 0,
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
