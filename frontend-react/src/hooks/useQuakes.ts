import { useEffect, useState } from "react";

import { fetchQuakes } from "../api/quakeApi";
import type { Quake } from "../types";

interface UseQuakesResult {
  quakes: Quake[];
  isLoading: boolean;
  errorMessage: string | null;
  reload: () => void;
}

// Keeps the fetching logic out of the components.
export function useQuakes(minMmi: number): UseQuakesResult {
  const [quakes, setQuakes] = useState<Quake[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Bumping this is how a reload is requested.
  const [reloadCounter, setReloadCounter] = useState(0);

  useEffect(() => {
    // If the effect is cleaned up before the fetch finishes, the answer is no
    // longer wanted and must not reach the screen.
    let isCurrent = true;

    async function load() {
      setIsLoading(true);
      setErrorMessage(null);

      try {
        const data = await fetchQuakes(minMmi);

        if (isCurrent) {
          setQuakes(data);
        }
      } catch (error) {
        console.error(error);

        if (isCurrent) {
          setQuakes([]);
          setErrorMessage(
            "Could not load earthquakes. Is the API running on port 5055?"
          );
        }
      } finally {
        if (isCurrent) {
          setIsLoading(false);
        }
      }
    }

    load();

    return () => {
      isCurrent = false;
    };
  }, [minMmi, reloadCounter]);

  function reload() {
    setReloadCounter((current) => current + 1);
  }

  return { quakes, isLoading, errorMessage, reload };
}
