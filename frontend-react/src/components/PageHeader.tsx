interface PageHeaderProps {
  count: number;
  isLoading: boolean;
  onReload: () => void;
}

export function PageHeader({ count, isLoading, onReload }: PageHeaderProps) {
  return (
    <header className="page-header">
      <div>
        <h1>QuakeWatch NZ</h1>
        <p className="subtitle">Recent earthquakes in New Zealand from GeoNet</p>
      </div>

      <div className="header-right">
        <p className="quake-count">{count} earthquakes</p>

        <button
          type="button"
          className="reload-button"
          onClick={onReload}
          disabled={isLoading}
        >
          {isLoading ? "Loading..." : "Reload"}
        </button>
      </div>
    </header>
  );
}
