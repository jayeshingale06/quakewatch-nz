// One earthquake as the API sends it. Must match Models/Quake.cs.
export type Severity = "light" | "moderate" | "strong" | "major";

export type FilterValue = Severity | "all";

export interface Quake {
  publicID: string;
  magnitude: number;
  depth: number;
  locality: string;
  mmi: number;
  quality: string;
  time: string;
  severity: Severity;
}
