import type { Quake } from "../types";

// Temporary. Next lesson this comes from http://localhost:5055/api/quakes.

export const sampleQuakes: Quake[] = [
  {
    publicID: "2026p123460",
    magnitude: 7.1,
    depth: 20,
    locality: "40 km east of Wellington",
    mmi: 9,
    quality: "best",
    time: "2026-09-12T10:30:00Z",
    severity: "major",
  },
  {
    publicID: "2026p123459",
    magnitude: 6.2,
    depth: 15,
    locality: "30 km south-west of Kaikōura",
    mmi: 7,
    quality: "best",
    time: "2026-09-10T09:45:22Z",
    severity: "strong",
  },
  {
    publicID: "2026p123457",
    magnitude: 5.8,
    depth: 35,
    locality: "15 km north-west of Christchurch",
    mmi: 5,
    quality: "best",
    time: "2026-09-11T18:02:47Z",
    severity: "moderate",
  },
  {
    publicID: "2026p123458",
    magnitude: 3.1,
    depth: 5,
    locality: "10 km north of Dunedin",
    mmi: 2,
    quality: "automatic",
    time: "2026-09-11T16:30:00Z",
    severity: "light",
  },
];
