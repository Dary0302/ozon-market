# ─── Этап 1: сборка React-приложения ────────────────────────────────────────
FROM node:20-alpine AS build

WORKDIR /app

COPY package.json package-lock.json* ./

RUN if [ -f package-lock.json ]; then npm ci; else npm install; fi

COPY public/ ./public/
COPY src/ ./src/

ARG REACT_APP_ORDER_API
ARG REACT_APP_STORAGE_API
ARG REACT_APP_PRODUCT_API
ENV REACT_APP_ORDER_API=$REACT_APP_ORDER_API
ENV REACT_APP_STORAGE_API=$REACT_APP_STORAGE_API
ENV REACT_APP_PRODUCT_API=$REACT_APP_PRODUCT_API

RUN npm run build

# ─── Этап 2: nginx раздаёт статику ──────────────────────────────────────────
FROM nginx:1.27-alpine AS final

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/build /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
