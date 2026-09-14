# Build stage: Node compiles React and TypeScript to static files.
FROM node:24-alpine AS build

WORKDIR /app

# npm ci installs the exact versions in package-lock.json.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# Vite bakes environment variables into the bundle, so the API address has to
# be known at build time.
ARG VITE_API_BASE=http://localhost:5055
ENV VITE_API_BASE=$VITE_API_BASE

RUN npm run build

# Serve stage: nginx and static files only.
FROM nginx:alpine AS final

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
