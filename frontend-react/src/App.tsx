import { useState } from "react";

import { PageHeader } from "./components/PageHeader";
import { StatsRow } from "./components/StatsRow";
import { FilterBar } from "./components/FilterBar";
import { StatusMessage } from "./components/StatusMessage";
import { QuakeList } from "./components/QuakeList";

import { useQuakes } from "./hooks/useQuakes";
import type { FilterValue } from "./types";

const MINIMUM_MMI = 3;

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
