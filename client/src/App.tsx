import { createBrowserRouter, Navigate, RouterProvider, type RouteObject } from "react-router-dom";
import { MainPage } from "./pages/MainPage";
import "./index.css";
import { ProductListingsPage } from "./pages/ProductListingsPage";
import { PanoramaBackground } from "./components/PanoramaBackground";
import { LoginPage } from "./pages/LoginPage";
import { ProfilePage } from "./pages/ProfilePage";
import { AuthProvider } from "./Auth/AuthProvider";

const routes: RouteObject[] = [
    { path: "/", element: <MainPage /> },
    { path: "/login", element: <LoginPage /> },
    { path: "/profile", element: <ProfilePage /> },
    { path: "/products/:id", element: <ProductListingsPage /> },
    { path: "*", element: <Navigate to="/" /> },
];

const router = createBrowserRouter(routes);

export function App() {
    return (
        <>
            <PanoramaBackground />
            <AuthProvider>
                <RouterProvider router={router} />
            </AuthProvider>
        </>
    );
}
export default App;