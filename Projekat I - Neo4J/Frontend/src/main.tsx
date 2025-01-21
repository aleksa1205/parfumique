import "./index.css";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import MainLayer from "./layers/MainLayer";
import HomePage from "./pages/HomePage";
import Login from "./pages/Login";
import Fragrances from "./pages/Fragrances";
import FragranceDetails from "./pages/FragranceDetails";
import AboutUs from "./pages/AboutUs";
import Register from "./pages/Register";
import NotFound from "./pages/NotFound";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import AuthProvider from "./context/AuthProvider";
import { CurrUserProvider } from "./context/CurrUserProvider";
import AdminRequiredLayer from "./layers/AdminRequiredLayer";
import AdminDashboard from "./pages/Admin/AdminDashboard";
import AdminFragrance from "./pages/Admin/AdminFragrance";
import { AdminNotes } from "./pages/Admin/AdminNote";
import { AdminManufacturer } from "./pages/Admin/AdminManufacturer";
import AdminParfumer from "./pages/Admin/AdminPafrumer";
import AdminUser from "./pages/Admin/AdminUser";

const queryClient = new QueryClient();
const router = createBrowserRouter([
  {
    element: <MainLayer />,
    children: [
      {
        path: "/*",
        element: <NotFound />,
      },
      {
        path: "/",
        element: <HomePage />
      },
      {
        path: "/register",
        element: <Register />,
      },
      {
        path: "/login",
        element: <Login />,
      },
      {
        path: "/fragrances",
        element: <Fragrances />,
      },
      {
        path: "/fragrances/:id",
        element: <FragranceDetails />,
      },
      {
        path: "/about-us",
        element: <AboutUs />,
      },
      {
        element: <AdminRequiredLayer />,
        children: [
          {
            path: "/admin-dashboard",
            element: <AdminDashboard />,
          },
          {
            path: "/admin-dashboard/fragrance",
            element: <AdminFragrance />
          },
          {
            path: "/admin-dashboard/note",
            element: <AdminNotes />
          },
          {
            path: "/admin-dashboard/manufacturer",
            element: <AdminManufacturer />
          },
          {
            path: "/admin-dashboard/parfumer",
            element: <AdminParfumer />
          },
          {
            path: "/admin-dashboard/user",
            element: <AdminUser />
          },
        ]
      }
    ]
  },
]);

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <CurrUserProvider>
          <RouterProvider router={router} />
        </CurrUserProvider>
      </AuthProvider>
    </QueryClientProvider>
  </StrictMode>
);
