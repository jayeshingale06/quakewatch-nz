import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// GitHub Pages serves this project from /<repo-name>/, so asset links need that
// prefix or the browser requests /assets/... and gets a 404.
// The deploy workflow sets GITHUB_PAGES=true; local development does not.
const base = process.env.GITHUB_PAGES === "true" ? "/quakewatch-nz/" : "/";

export default defineConfig({
  base,
  plugins: [react()],
});
