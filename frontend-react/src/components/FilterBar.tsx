import type { FilterValue } from "../types";

interface FilterBarProps {
  activeFilter: FilterValue;
  onFilterChange: (filter: FilterValue) => void;
}

// Filters as data, so adding one is a single line.
const FILTERS: { value: FilterValue; label: string }[] = [
  { value: "all", label: "All" },
  { value: "light", label: "Light" },
  { value: "moderate", label: "Moderate" },
  { value: "strong", label: "Strong" },
  { value: "major", label: "Major" },
];

export function FilterBar({ activeFilter, onFilterChange }: FilterBarProps) {
  return (
    <div className="filters">
      {FILTERS.map((filter) => (
        <button
          key={filter.value}
          type="button"
          className={
            filter.value === activeFilter
              ? "filter-button is-active"
              : "filter-button"
          }
          onClick={() => onFilterChange(filter.value)}
        >
          {filter.label}
        </button>
      ))}
    </div>
  );
}
