import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { QuakeCard } from "./QuakeCard";
import type { Quake } from "../types";

const reviewedQuake: Quake = {
  publicID: "2026p123459",
  magnitude: 6.2,
  depth: 15,
  locality: "30 km south-west of Seddon",
  mmi: 7,
  quality: "best",
  time: "2026-09-10T09:45:22Z",
  severity: "strong",
};

describe("QuakeCard", () => {
  it("shows the identifier, magnitude, depth and locality", () => {
    render(<QuakeCard quake={reviewedQuake} />);

    expect(
      screen.getByRole("heading", { name: "2026p123459" })
    ).toBeInTheDocument();

    expect(screen.getByText(/Magnitude: 6.2/)).toBeInTheDocument();
    expect(screen.getByText(/Depth: 15 km/)).toBeInTheDocument();
    expect(screen.getByText(/30 km south-west of Seddon/)).toBeInTheDocument();
    expect(screen.getByText(/Shaking \(MMI\): 7/)).toBeInTheDocument();
  });

  it("puts the severity into the class name so the CSS can colour it", () => {
    const { container } = render(<QuakeCard quake={reviewedQuake} />);

    const article = container.querySelector("article");

    expect(article?.className).toContain("quake");
    expect(article?.className).toContain("quake-strong");
  });

  it("does not emphasise a quality of best", () => {
    const { container } = render(<QuakeCard quake={reviewedQuake} />);

    expect(container.querySelector("strong")).toBeNull();
    expect(screen.getByText(/Quality: best/)).toBeInTheDocument();
  });

  it("emphasises a quality that has not been reviewed by a human", () => {
    render(
      <QuakeCard quake={{ ...reviewedQuake, quality: "automatic" }} />
    );

    const emphasised = screen.getByText("automatic");

    expect(emphasised.tagName).toBe("STRONG");
  });

  it("gives the time a machine readable attribute", () => {
    const { container } = render(<QuakeCard quake={reviewedQuake} />);

    const time = container.querySelector("time");

    expect(time?.getAttribute("datetime")).toBe(reviewedQuake.time);
  });

  it("colours a major quake differently from a strong one", () => {
    const { container } = render(
      <QuakeCard quake={{ ...reviewedQuake, severity: "major", mmi: 9 }} />
    );

    expect(container.querySelector("article")?.className).toContain(
      "quake-major"
    );
  });
});
