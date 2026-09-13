import { describe, expect, it } from "vitest";

import { formatTime } from "./formatTime";

describe("formatTime", () => {
  it("writes the month as a word, not a number", () => {
    const result = formatTime("2026-09-12T10:30:00Z");

    expect(result).toContain("September");
    expect(result).toContain("2026");
  });

  it("includes a time with a minute", () => {
    const result = formatTime("2026-09-12T10:30:00Z");

    // en-NZ gives am/pm, and the minute is always two digits.
    expect(result).toMatch(/\d{1,2}:\d{2}/);
  });

  it("does not crash on an unparseable string", () => {
    // Bad input should not throw. "Invalid Date" is an acceptable answer.
    expect(() => formatTime("not a date")).not.toThrow();
  });
});
