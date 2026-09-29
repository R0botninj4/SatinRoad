import { Api } from "../api/Api";

// Shared API client. "include" makes the browser send the login cookie to the
// API, which runs on a different port than the client.
// Not placed in src/api/ because `bun run gen:api` wipes that folder.
export const api = new Api({
    baseApiParams: { credentials: "include", headers: {} },
});

// Turns whatever the API client throws into a message a person can act on.
export function getErrorMessage(error: unknown): string {
    if (error instanceof TypeError) {
        return "Can't reach the server. Check your connection and try again.";
    }

    const response = error as { status?: number; error?: any } | null;
    const body = response?.error;

    if (typeof body?.message === "string") return body.message;

    if (body?.errors && typeof body.errors === "object") {
        const first = Object.values(body.errors).flat()[0];
        if (typeof first === "string") return first;
    }

    if (response?.status === 429) return "Too many attempts. Wait a moment and try again.";
    return "Something went wrong. Try again.";
}