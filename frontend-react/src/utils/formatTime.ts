// Formats an ISO timestamp in the reader's own timezone.
export function formatTime(isoString: string): string {
  const date = new Date(isoString);

  return date.toLocaleString("en-NZ", {
    day: "numeric",
    month: "long",
    year: "numeric",
    hour: "numeric",
    minute: "2-digit",
  });
}
