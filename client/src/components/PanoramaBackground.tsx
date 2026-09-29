import "./PanoramaBackground.css";

// Rotating Minecraft-style panorama background: the camera sits inside a cube
// with one image on each side, and the cube slowly turns.
export function PanoramaBackground() {
    return (
        <div className="panorama" aria-hidden="true">
            <div className="panorama-cube">
                <div className="panorama-face panorama-front" />
                <div className="panorama-face panorama-right" />
                <div className="panorama-face panorama-back" />
                <div className="panorama-face panorama-left" />
            </div>
        </div>
    );
}