import { createBrowserRouter, Navigate, RouterProvider, type RouteObject } from "react-router-dom";
import { MainPage } from "./pages/MainPage";
import "./index.css";

const routes: RouteObject[] = [
    { path: "/", element: <MainPage /> },
    { path: "*", element: <Navigate to="/" replace /> },
];

const router = createBrowserRouter(routes);

export function App() {
    return <RouterProvider router={router} />;
}

export default App;