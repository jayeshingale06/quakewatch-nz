// Turns "2026-09-12T10:30:00Z" into "12 September 2026, 10:30 pm"
// in the reader's own timezone.

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
