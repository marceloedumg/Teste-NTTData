import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  build: {
    outDir: "dist/client",
  },
  optimizeDeps: {
    include: ["react", "react-dom/client"],
  },
  server: {
    host: "0.0.0.0",
    allowedHosts: ["terminal.local"],
    // O proxy mantém o mesmo contrato relativo usado pelo Nginx no Docker.
    proxy: {
      "/api": "http://localhost:5080",
      "/auth": "http://localhost:5080",
      "/health": "http://localhost:5080",
    },
    warmup: {
      clientFiles: ["./src/main.jsx"],
    },
  },
  plugins: [react()],
});
