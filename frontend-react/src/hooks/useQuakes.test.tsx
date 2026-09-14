import { renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { useQuakes } from "./useQuakes";
import { fetchQuakes } from "../api/quakeApi";
import type { Quake } from "../types";

// The api module is mocked, so no request leaves the machine.
vi.mock("../api/quakeApi");

const mockedFetchQuakes = vi.mocked(fetchQuakes);

const sampleQuake: Quake = {
  publicID: "test-1",
  magnitude: 6.2,
  depth: 15,
  locality: "Somewhere in New Zealand",
  mmi: 7,
  quality: "best",
  time: "2026-09-10T09:45:22Z",
  severity: "strong",
};

describe("useQuakes", () => {
  beforeEach(() => {
    vi.resetAllMocks();
    // The hook logs failures on purpose. Keep test output clean.
    vi.spyOn(console, "error").mockImplementation(() => {});
  });

  it("starts out loading", () => {
    mockedFetchQuakes.mockResolvedValue([]);

    const { result } = renderHook(() => useQuakes(3));

    expect(result.current.isLoading).toBe(true);
    expect(result.current.quakes).toEqual([]);
  });

  it("puts the fetched quakes into state and stops loading", async () => {
    mockedFetchQuakes.mockResolvedValue([sampleQuake]);

    const { result } = renderHook(() => useQuakes(3));

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(result.current.quakes).toHaveLength(1);
    expect(result.current.quakes[0].publicID).toBe("test-1");
    expect(result.current.errorMessage).toBeNull();
  });

  it("sets an error message and clears the data when the API fails", async () => {
    mockedFetchQuakes.mockRejectedValue(new Error("network is down"));

    const { result } = renderHook(() => useQuakes(3));

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(result.current.quakes).toEqual([]);
    expect(result.current.errorMessage).not.toBeNull();
  });

  it("passes the minimum MMI through to the API", async () => {
    mockedFetchQuakes.mockResolvedValue([]);

    const { result } = renderHook(() => useQuakes(6));

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(mockedFetchQuakes).toHaveBeenCalledWith(6);
  });

  it("fetches again when reload is called", async () => {
    mockedFetchQuakes.mockResolvedValue([sampleQuake]);

    const { result } = renderHook(() => useQuakes(3));

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    const callsBefore = mockedFetchQuakes.mock.calls.length;

    result.current.reload();

    await waitFor(() =>
      expect(mockedFetchQuakes.mock.calls.length).toBeGreaterThan(callsBefore)
    );
  });
});
