FROM node:24-alpine AS build
WORKDIR /src/frontend

COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci

COPY frontend ./
COPY assets ../assets

ARG VITE_API_BASE_URL
ARG VITE_SUPABASE_URL
ARG SUPABASE_PUBLIC_VALUE
ARG VITE_WHATSAPP_NUMBER=593996359219

RUN VITE_API_BASE_URL="$VITE_API_BASE_URL" \
    VITE_SUPABASE_URL="$VITE_SUPABASE_URL" \
    VITE_SUPABASE_PUBLISHABLE_KEY="$SUPABASE_PUBLIC_VALUE" \
    VITE_WHATSAPP_NUMBER="$VITE_WHATSAPP_NUMBER" \
    npm run build

FROM nginx:1.29-alpine AS final
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /src/frontend/dist /usr/share/nginx/html
EXPOSE 8080
