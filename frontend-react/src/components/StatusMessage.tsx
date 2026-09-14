interface StatusMessageProps {
  isLoading: boolean;
  errorMessage: string | null;
}

export function StatusMessage({ isLoading, errorMessage }: StatusMessageProps) {
  if (errorMessage !== null) {
    return <p className="status status-error">{errorMessage}</p>;
  }

  if (isLoading) {
    return <p className="status">Loading earthquakes...</p>;
  }

  return null;
}
