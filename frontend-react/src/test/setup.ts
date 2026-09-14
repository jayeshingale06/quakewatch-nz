import { cleanup } from "@testing-library/react";
import { afterEach } from "vitest";

import "@testing-library/jest-dom/vitest";

// Testing Library only cleans up by itself when Vitest globals are on. This
// project uses explicit imports, so the automatic hook never fires and renders
// would otherwise pile up in the same document between tests.
afterEach(() => {
  cleanup();
});
