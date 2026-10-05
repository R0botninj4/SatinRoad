import {
    createContext,
    useCallback,
    useContext,
    useEffect,
    useMemo,
    useState,
    type ReactNode,
} from "react";
import type { UserResponseDto } from "../api/Api";
import { api } from "./apiClient";

type AuthContextValue = {
    user: UserResponseDto | null;
    loading: boolean;
    login: (username: string, password: string) => Promise<void>;
    register: (username: string, password: string) => Promise<void>;
    logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<UserResponseDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function loadCurrentUser() {
            try {
                const currentUser = await api.api.authMe();
                setUser(currentUser);
            } catch {
                setUser(null);
            } finally {
                setLoading(false);
            }
        }

        loadCurrentUser();
    }, []);

    const login = useCallback(async (username: string, password: string) => {
        const loggedInUser = await api.api.authLogin({
            username,
            password,
        });

        setUser(loggedInUser);
    }, []);

    const register = useCallback(
        async (username: string, password: string) => {
            await api.api.authRegister({ username, password });
            await login(username, password);
        },
        [login],
    );

    const logout = useCallback(async () => {
        await api.api.authLogout();
        setUser(null);
    }, []);

    const value = useMemo(
        () => ({
            user,
            loading,
            login,
            register,
            logout,
        }),
        [user, loading, login, register, logout],
    );

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error("useAuth must be used inside <AuthProvider>.");
    }

    return context;
}