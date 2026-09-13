import type { Quake } from "../types";

interface StatsRowProps {
  quakes: Quake[];
}

export function StatsRow({ quakes }: StatsRowProps) {
  const hasQuakes = quakes.length > 0;

  const strongest = hasQuakes
    ? Math.max(...quakes.map((quake) => quake.magnitude)).toFixed(1)
    : "-";

  const maxShaking = hasQuakes
    ? "MMI " + Math.max(...quakes.map((quake) => quake.mmi))
    : "-";

  return (
    <section className="stats">
      <div className="stat">
        <p className="stat-label">Total</p>
        <p className="stat-value">{quakes.length}</p>
      </div>

      <div className="stat">
        <p className="stat-label">Strongest</p>
        <p className="stat-value">{strongest}</p>
      </div>

      <div className="stat">
        <p className="stat-label">Max shaking</p>
        <p className="stat-value">{maxShaking}</p>
      </div>
    </section>
  );
}
