# ============================================================
# Stage 1: BUILD. Node compiles React and TypeScript into
# plain HTML, CSS and JavaScript.
# ============================================================
FROM node:24-alpine AS build

WORKDIR /app

# package-lock.json pins exact versions, so this build is
# reproducible. npm ci is the strict version of npm install.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# Vite bakes environment variables INTO the built files at
# build time, so the API address must be known here.
ARG VITE_API_BASE=http://localhost:5055
ENV VITE_API_BASE=$VITE_API_BASE

RUN npm run build

# ============================================================
# Stage 2: SERVE. No Node at all. Just nginx and static files.
# ============================================================
FROM nginx:alpine AS final

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
