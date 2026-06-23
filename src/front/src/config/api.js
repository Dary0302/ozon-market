// Адреса бэкенд-сервисов берутся из переменных окружения React (REACT_APP_*),
// которые подставляются на этапе сборки (npm start / npm run build).
// Если переменная не задана — используется адрес сервера по умолчанию.
//
// CORS на бэкенде разрешён, поэтому фронтенд обращается к бэкенду
// напрямую по полному адресу (http://host:port), без nginx-прокси.

const DEFAULT_SERVER_HOST = '62.113.98.200';

export const API_BASE = {
  ORDER: process.env.REACT_APP_ORDER_API || `http://${DEFAULT_SERVER_HOST}:5001`,
  STORAGE: process.env.REACT_APP_STORAGE_API || `http://${DEFAULT_SERVER_HOST}:5002`,
  PRODUCT: process.env.REACT_APP_PRODUCT_API || `http://${DEFAULT_SERVER_HOST}:5003`,
};
