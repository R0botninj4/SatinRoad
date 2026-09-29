import { createBrowserRouter, Navigate, RouterProvider, type RouteObject } from "react-router-dom";
import { MainPage } from "./pages/MainPage";
import "./index.css";
import { ProductListingsPage } from "./pages/ProductListingsPage";
import { PanoramaBackground } from "./components/PanoramaBackground";

const routes: RouteObject[] = [
    { path: "/", element: <MainPage /> },
    { path: "/products/:id", element: <ProductListingsPage /> }, 
    { path: "*", element: <Navigate to="/" /> },
];

const router = createBrowserRouter(routes);

export function App() {
    return (
        <>
            <PanoramaBackground />
            <RouterProvider router={router} />
        </>
    );
}
export default App;