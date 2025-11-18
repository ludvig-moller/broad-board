/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_BACKEND_WEBSOCKET_URL: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
