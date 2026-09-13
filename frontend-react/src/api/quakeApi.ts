import type { Quake } from "../types";

// Every call to our C# API goes through this file.
//
// The address comes from an environment variable so the same code
// works locally, in Docker and on Azure. Vite bakes VITE_ variables
// into the built files, so it must be set at BUILD time.
const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5055";

export async function fetchQuakes(minMmi: number): Promise<Quake[]> {
  const response = await fetch(`${API_BASE}/api/quakes?minMmi=${minMmi}`);

  // fetch does NOT throw on 404 or 503, so we check ourselves.
  if (!response.ok) {
    throw new Error(`The API answered with status ${response.status}`);
  }

  return (await response.json()) as Quake[];
}
