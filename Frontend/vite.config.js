import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  return {
    plugins: [react()],
    build: {
      outDir: "build",
      sourcemap: mode !== "production",
      minify: mode === "production",
      chunkSizeWarningLimit: 1600,
    },
  };
});
