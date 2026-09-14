import type { Quake } from "../types";

// The address comes from an environment variable. Vite bakes VITE_ values into
// the bundle, so it has to be set at build time.
const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5055";

export async function fetchQuakes(minMmi: number): Promise<Quake[]> {
  const response = await fetch(`${API_BASE}/api/quakes?minMmi=${minMmi}`);

  // fetch does not throw on 404 or 503, so check the status here.
  if (!response.ok) {
    throw new Error(`The API answered with status ${response.status}`);
  }

  return (await response.json()) as Quake[];
}
