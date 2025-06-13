import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,    // чтобы можно было использовать describe/it без импорта
    environment: 'jsdom',
    setupFiles: './setup.tests.ts',
    include: ['src/**/*.{test,spec}.{js,ts,tsx}'],
  },
})
