// Production server: serves the built site from dist/ with caching and compression.
// Build first with `bun run build`, then start with `bun run start`.
import { basename, extname, join } from "path";

const DIST = join(import.meta.dir, "dist");
const PORT = Number(process.env.PORT ?? 3000);

// Built files have a content hash in their name (e.g. index-gzttvca9.js).
// A new version gets a new name, so the browser can keep them for a year.
const CACHE_FOREVER = "public, max-age=31536000, immutable";
// index.html keeps its name, so the browser must check it every time.
// Otherwise users would keep loading an old version after a new deployment.
const ALWAYS_CHECK = "no-cache";

// Text files shrink a lot with gzip; images and fonts are already compressed.
const COMPRESSIBLE = new Set([".html", ".js", ".css", ".svg", ".json", ".map"]);
const gzipped = new Map<string, Uint8Array<ArrayBuffer>>();

async function serveFile(name: string, request: Request): Promise<Response | null> {
    const file = Bun.file(join(DIST, name));
    if (!(await file.exists())) return null;

    const headers = new Headers({
        "Content-Type": file.type,
        "Cache-Control": name === "index.html" ? ALWAYS_CHECK : CACHE_FOREVER,
        "Vary": "Accept-Encoding",
    });

    const acceptsGzip = request.headers.get("accept-encoding")?.includes("gzip");
    if (acceptsGzip && COMPRESSIBLE.has(extname(name))) {
        // Compress each file once and reuse the result
        let body = gzipped.get(name);
        if (!body) {
            body = Bun.gzipSync(new Uint8Array(await file.arrayBuffer())) as Uint8Array<ArrayBuffer>;
            gzipped.set(name, body);
        }
        headers.set("Content-Encoding", "gzip");
        return new Response(body, { headers });
    }

    return new Response(file, { headers });
}

Bun.serve({
    port: PORT,
    async fetch(request) {
        // All built files sit directly in dist/, so only the file name matters.
        // basename() also stops paths like /../../secret from leaving dist/.
        const name = basename(new URL(request.url).pathname);

        const fileResponse = name ? await serveFile(name, request) : null;

        // Unknown paths (e.g. /products/6) are React Router pages: send index.html
        return fileResponse ?? (await serveFile("index.html", request))!;
    },
});

console.log(`Serving dist/ on http://localhost:${PORT}`);