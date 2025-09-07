import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider, createBrowserRouter } from "react-router-dom"
import LandingPage from './pages/LandingPage'
import Board from './pages/Board'

const router = createBrowserRouter([
  {path: "/", element: <LandingPage />},
  {path: "/:boardId", element: <Board />},
])

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>
)
