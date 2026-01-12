# Build stage
FROM node:18.20.2-alpine AS build
WORKDIR /usr/src/app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Runtime stage
FROM nginx:stable-alpine

RUN cat <<EOF > /etc/nginx/conf.d/default.conf
server {
  listen 80;
  server_name localhost;
  root /usr/share/nginx/html;
  index index.html;

  # Proxy API requests to backend, stripping /api prefix
  location /api {
    rewrite ^/api(.*)$ \$1 break;
    proxy_pass http://stargate-api:8080;
    proxy_set_header Host \$host;
    proxy_set_header X-Real-IP \$remote_addr;
    proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto \$scheme;
    proxy_http_version 1.1;
  }

  # Serve Angular app (SPA routing)
  location / {
    try_files \$uri \$uri/ /index.html;
  }

  # Error pages
  error_page 500 502 503 504 /50x.html;
  location = /50x.html {
    root /usr/share/nginx/html;
  }
}
EOF

COPY --from=build /usr/src/app/dist/stargate-ui/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]