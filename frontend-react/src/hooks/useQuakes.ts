import { useEffect, useState } from "react";

import { fetchQuakes } from "../api/quakeApi";
import type { Quake } from "../types";

// Everything a component needs to show earthquakes.
interface UseQuakesResult {
  quakes: Quake[];
  isLoading: boolean;
  errorMessage: string | null;
  reload: () => void;
}

// A CUSTOM HOOK. It is just a function that uses other hooks
// inside it, and whose name begins with "use".
//
// All the loading logic lives here, so no component has to know
// how earthquakes are fetched.

export function useQuakes(minMmi: number): UseQuakesResult {
  const [quakes, setQuakes] = useState<Quake[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Changing this number is how we ask for a fresh load.
  const [reloadCounter, setReloadCounter] = useState(0);

  useEffect(() => {
    // If this effect is cleaned up before the fetch finishes, the
    // answer is no longer wanted. This flag is how we know to
    // throw it away instead of putting it on screen.
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

    // The CLEANUP function. React runs this before the next effect
    // and when the component disappears.
    return () => {
      isCurrent = false;
    };
  }, [minMmi, reloadCounter]);

  function reload() {
    // The updater form: "whatever the current value is, add one".
    setReloadCounter((current) => current + 1);
  }

  return { quakes, isLoading, errorMessage, reload };
}
