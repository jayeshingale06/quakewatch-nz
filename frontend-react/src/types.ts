// The shape of one earthquake, as our C# API sends it.
// This must match Models/Quake.cs on the server.

// Severity can only ever be one of these four words.
export type Severity = "light" | "moderate" | "strong" | "major";

// A filter is any severity, or the word "all".
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
