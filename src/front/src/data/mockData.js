// Статусы заказа — для отображения подписей в UI заказов.
// (Сами заказы, товары, цены и остатки теперь приходят с бэкенда:
//  см. src/api/productsApi.js, src/api/storageApi.js, src/hooks/useOrders.js)

export const ORDER_STATUSES = {
  created: 'Заказ создан',
  paid: 'Оплачен',
  assembling: 'Собирается',
  delivery: 'Передан в доставку',
  delivered: 'Доставлен',
  cancelled: 'Отменён',
};
