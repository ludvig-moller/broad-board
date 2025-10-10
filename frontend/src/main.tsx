import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider, createBrowserRouter } from "react-router-dom"

import "./styles/main.scss"
import Layout from './components/Layout'
import LandingPage from './pages/LandingPage'
import Board from './pages/Board'

const router = createBrowserRouter([
  {
    element: <Layout />,
    children: [
      {path: "/", element: <LandingPage />},
      {path: "/:boardId", element: <Board />},
    ],
  },
]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>
);
