import { useState } from "react";

import { PageHeader } from "./components/PageHeader";
import { StatsRow } from "./components/StatsRow";
import { FilterBar } from "./components/FilterBar";
import { StatusMessage } from "./components/StatusMessage";
import { QuakeList } from "./components/QuakeList";

import { useQuakes } from "./hooks/useQuakes";
import type { FilterValue } from "./types";

// 0 includes earthquakes nobody felt, which is most of them. GeoNet
// records thousands a year, so a page filtered to MMI 3 and above shows
// only a handful and looks broken. The severity filters are how a reader
// narrows it down.
const MINIMUM_MMI = 0;

function App() {
  // All the fetching, loading and error handling: one line.
  const { quakes, isLoading, errorMessage, reload } = useQuakes(MINIMUM_MMI);

  // This state belongs to the screen, not to the data, so it stays here.
  const [activeFilter, setActiveFilter] = useState<FilterValue>("all");

  const visibleQuakes =
    activeFilter === "all"
      ? quakes
      : quakes.filter((quake) => quake.severity === activeFilter);

  return (
    <>
      <PageHeader
        count={quakes.length}
        isLoading={isLoading}
        onReload={reload}
      />

      <StatsRow quakes={quakes} />

      <FilterBar activeFilter={activeFilter} onFilterChange={setActiveFilter} />

      <StatusMessage isLoading={isLoading} errorMessage={errorMessage} />

      {!isLoading && errorMessage === null && (
        <QuakeList quakes={visibleQuakes} />
      )}
    </>
  );
}

export default App;
