import { useState, type FormEvent } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthProvider";
import { getErrorMessage } from "../auth/apiClient";
import "./LoginPage.css";

type Mode = "login" | "register";

// Same limits the API enforces (RegisterRequestDto).
const USERNAME_PATTERN = /^[A-Za-z0-9_]{3,30}$/;
const MIN_PASSWORD_LENGTH = 12;

function validateRegistration(username: string, password: string, repeat: string) {
    if (!USERNAME_PATTERN.test(username)) {
        return "Username must be 3-30 characters: letters, numbers or underscores.";
    }
    if (password.length < MIN_PASSWORD_LENGTH) {
        return `Password must be at least ${MIN_PASSWORD_LENGTH} characters.`;
    }
    if (password !== repeat) {
        return "The two passwords don't match.";
    }
    return "";
}

export function LoginPage() {
    const { user, loading, login, register } = useAuth();
    const navigate = useNavigate();

    const [mode, setMode] = useState<Mode>("login");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [repeat, setRepeat] = useState("");
    const [error, setError] = useState("");
    const [pending, setPending] = useState(false);

    const isLogin = mode === "login";

    // Already logged in: nothing to do here.
    if (!loading && user) return <Navigate to="/" replace />;

    function switchMode() {
        setMode(isLogin ? "register" : "login");
        setError("");
        setRepeat("");
    }

    async function handleSubmit(event: FormEvent) {
        event.preventDefault();

        const name = username.trim();
        const problem = isLogin
            ? name && password ? "" : "Enter your username and password."
            : validateRegistration(name, password, repeat);
        if (problem) {
            setError(problem);
            return;
        }

        setError("");
        setPending(true);
        try {
            if (isLogin) await login(name, password);
            else await register(name, password);
            navigate("/");
        } catch (caught) {
            setError(getErrorMessage(caught));
        } finally {
            setPending(false);
        }
    }

    return (
        <section className="login-page">
            <Link to="/" className="login-back">Go back to main page</Link>

            <form className="login-sign" onSubmit={handleSubmit} noValidate>
                <h1 className="login-title">{isLogin ? "Log in" : "Create account"}</h1>

                <label className="login-field">
                    <span>Username</span>
                    <input
                        className="login-input"
                        type="text"
                        name="username"
                        autoComplete="username"
                        autoCapitalize="none"
                        spellCheck={false}
                        maxLength={30}
                        value={username}
                        onChange={(event) => setUsername(event.target.value)}
                    />
                </label>

                <label className="login-field">
                    <span>Password</span>
                    <input
                        className="login-input"
                        type="password"
                        name="password"
                        autoComplete={isLogin ? "current-password" : "new-password"}
                        maxLength={128}
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                    />
                    {!isLogin && <small>At least {MIN_PASSWORD_LENGTH} characters.</small>}
                </label>

                {!isLogin && (
                    <label className="login-field">
                        <span>Repeat password</span>
                        <input
                            className="login-input"
                            type="password"
                            name="repeat-password"
                            autoComplete="new-password"
                            maxLength={128}
                            value={repeat}
                            onChange={(event) => setRepeat(event.target.value)}
                        />
                    </label>
                )}

                <p className="login-error" role="alert">{error}</p>

                <button className="login-button" type="submit" disabled={pending}>
                    {pending ? "Please wait..." : isLogin ? "Log in" : "Create account"}
                </button>

                <button className="login-switch" type="button" onClick={switchMode}>
                    {isLogin ? "New here? Create an account" : "Have an account? Log in"}
                </button>
            </form>
        </section>
    );
}