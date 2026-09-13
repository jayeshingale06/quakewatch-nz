import { cleanup } from "@testing-library/react";
import { afterEach } from "vitest";

// Adds the extra matchers like toBeInTheDocument().
import "@testing-library/jest-dom/vitest";

// Empty the fake page after EVERY test.
//
// React Testing Library does this by itself when Vitest globals are
// switched on. We use explicit imports instead, so the automatic
// hook never fires and we have to do it here.
//
// Without this, each render piles up in the same document and tests
// can see each other's output. That is how a test suite starts
// passing for the wrong reasons.
afterEach(() => {
  cleanup();
});
