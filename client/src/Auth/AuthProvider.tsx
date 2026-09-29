import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import type { UserResponseDto } from "../api/Api";
import { api } from "./apiClient";

type AuthContextValue = {
    user: UserResponseDto | null;
    // True until we know whether someone is already logged in.
    loading: boolean;
    login: (username: string, password: string) => Promise<void>;
    register: (username: string, password: string) => Promise<void>;
    logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<UserResponseDto | null>(null);
    const [loading, setLoading] = useState(true);

    // The login cookie is HttpOnly, so ask the API who we are on page load.
    useEffect(() => {
        api.api
            .authMe()
            .then(setUser)
            .catch(() => setUser(null))
            .finally(() => setLoading(false));
    }, []);

    const login = useCallback(async (username: string, password: string) => {
        setUser(await api.api.authLogin({ username, password }));
    }, []);

    const register = useCallback(
        async (username: string, password: string) => {
            await api.api.authRegister({ username, password });
            // Registering doesn't log you in, so do that right after.
            await login(username, password);
        },
        [login],
    );

    const logout = useCallback(async () => {
        try {
            await api.api.authLogout();
        } finally {
            setUser(null);
        }
    }, []);

    const value = useMemo(
        () => ({ user, loading, login, register, logout }),
        [user, loading, login, register, logout],
    );

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) throw new Error("useAuth must be used inside <AuthProvider>.");
    return context;
}