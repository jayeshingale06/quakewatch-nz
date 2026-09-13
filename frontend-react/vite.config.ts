import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// GitHub Pages serves a project at /<repo-name>/ rather than at /.
// So every link to a CSS or JS file has to start with that prefix,
// or the browser asks for /assets/... and gets a 404.
//
// The deploy workflow sets GITHUB_PAGES=true. Local development does
// not, so `npm run dev` still serves from / as normal.
const base = process.env.GITHUB_PAGES === "true" ? "/quakewatch-nz/" : "/";

export default defineConfig({
  base,
  plugins: [react()],
});
