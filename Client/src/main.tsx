import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import "./index.css"
import App from './App.tsx'
import {DevTools} from "jotai-devtools";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <DevTools />
    <App />
  </StrictMode>,
)
