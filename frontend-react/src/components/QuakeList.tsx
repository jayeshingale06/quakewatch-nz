import { QuakeCard } from "./QuakeCard";
import type { Quake } from "../types";

interface QuakeListProps {
  quakes: Quake[];
}

export function QuakeList({ quakes }: QuakeListProps) {
  if (quakes.length === 0) {
    return <p className="empty">No earthquakes in this band.</p>;
  }

  return (
    <section>
      {quakes.map((quake) => (
        <QuakeCard key={quake.publicID} quake={quake} />
      ))}
    </section>
  );
}
