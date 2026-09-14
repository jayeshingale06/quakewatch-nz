import { useState } from "react";

import { PageHeader } from "./components/PageHeader";
import { StatsRow } from "./components/StatsRow";
import { FilterBar } from "./components/FilterBar";
import { StatusMessage } from "./components/StatusMessage";
import { QuakeList } from "./components/QuakeList";

import { useQuakes } from "./hooks/useQuakes";
import type { FilterValue } from "./types";

// 0 includes events nobody felt, which is most of them. Filtering to MMI 3 and
// above leaves a handful and makes the page look broken, so the severity
// filters do the narrowing instead.
const MINIMUM_MMI = 0;

function App() {
  const { quakes, isLoading, errorMessage, reload } = useQuakes(MINIMUM_MMI);

  // Belongs to the screen rather than the data, so it stays here.
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
