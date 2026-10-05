import frontImage from "../assets/panorama/front.webp";
import rightImage from "../assets/panorama/right.webp";
import backImage from "../assets/panorama/back.webp";
import leftImage from "../assets/panorama/left.webp";
import "./PanoramaBackground.css";

// The images are imported here instead of in the CSS. Images used in CSS are
// embedded into the stylesheet, and the browser shows nothing until the whole
// stylesheet has loaded. Imported here, they load separately.
const faces = [
    { className: "panorama-front", image: frontImage },
    { className: "panorama-right", image: rightImage },
    { className: "panorama-back", image: backImage },
    { className: "panorama-left", image: leftImage },
];

// Rotating Minecraft-style panorama background: the camera sits inside a cube
// with one image on each side, and the cube slowly turns.
export function PanoramaBackground() {
    return (
        <div className="panorama" aria-hidden="true">
            <div className="panorama-cube">
                {faces.map((face) => (
                    <div
                        key={face.className}
                        className={`panorama-face ${face.className}`}
                        style={{ backgroundImage: `url(${face.image})` }}
                    />
                ))}
            </div>
        </div>
    );
}