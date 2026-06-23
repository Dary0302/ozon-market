# ─── Этап 1: сборка React-приложения ────────────────────────────────────────
FROM node:20-alpine AS build

WORKDIR /app

# Сначала копируем только package.json и package-lock.json.
# Docker кэширует этот слой отдельно — если исходники поменяются,
# но зависимости нет, повторная установка npm пропускается.
COPY package.json package-lock.json* ./

# npm ci использует точные версии из lock-файла (детерминированная установка).
# Если lock-файла нет — падаем на npm install.
RUN if [ -f package-lock.json ]; then npm ci; else npm install; fi

# Копируем исходный код и собираем production-сборку.
# CRA (Create React App) кладёт результат в ./build
COPY public/ ./public/
COPY src/ ./src/

# Адреса бэкенда можно переопределить при сборке образа:
#   docker build --build-arg REACT_APP_ORDER_API=http://host:5001 ...
# Если не передать — используются дефолты из src/config/api.js
ARG REACT_APP_ORDER_API
ARG REACT_APP_STORAGE_API
ARG REACT_APP_PRODUCT_API
ENV REACT_APP_ORDER_API=$REACT_APP_ORDER_API
ENV REACT_APP_STORAGE_API=$REACT_APP_STORAGE_API
ENV REACT_APP_PRODUCT_API=$REACT_APP_PRODUCT_API

RUN npm run build

# ─── Этап 2: nginx раздаёт статику ──────────────────────────────────────────
FROM nginx:1.27-alpine AS final

# Удаляем дефолтный конфиг nginx, который слушает порт 80 без наших правил
RUN rm /etc/nginx/conf.d/default.conf

# Подкладываем наш конфиг (просто раздача статики + SPA fallback)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Копируем собранные файлы из первого этапа
COPY --from=build /app/build /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
