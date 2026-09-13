import type { Quake } from "../types";
import { formatTime } from "../utils/formatTime";

interface QuakeCardProps {
  quake: Quake;
}

export function QuakeCard({ quake }: QuakeCardProps) {
  const isReviewed = quake.quality === "best";

  return (
    <article className={`quake quake-${quake.severity}`}>
      <h2>{quake.publicID}</h2>

      <p>Magnitude: {quake.magnitude}</p>
      <p>Depth: {quake.depth} km</p>
      <p>Locality: {quake.locality}</p>
      <p>Shaking (MMI): {quake.mmi}</p>

      <p>
        Quality:{" "}
        {isReviewed ? quake.quality : <strong>{quake.quality}</strong>}
      </p>

      <time dateTime={quake.time}>{formatTime(quake.time)}</time>
    </article>
  );
}
