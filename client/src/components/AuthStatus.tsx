import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthProvider";
import "./AuthStatus.css";

// Small "Log in" / "Log out" strip shown at the top of a page.
export function AuthStatus() {
    const { user, loading, logout } = useAuth();

    if (loading) return null;

    return (
        <nav className="auth-status" aria-label="Account">
            {user ? (
                <>
                    <span>{user.username}</span>
                    <button type="button" className="auth-status-link" onClick={logout}>
                        Log out
                    </button>
                </>
            ) : (
                <Link to="/login" className="auth-status-link">
                    Log in
                </Link>
            )}
        </nav>
    );
}