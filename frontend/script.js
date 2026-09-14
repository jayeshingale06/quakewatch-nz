const API_BASE = "http://localhost:5055";

let quakes = [];
let activeFilter = "all";

function formatTime(isoString) {
  const date = new Date(isoString);

  return date.toLocaleString("en-NZ", {
    day: "numeric",
    month: "long",
    year: "numeric",
    hour: "numeric",
    minute: "2-digit",
  });
}

// Severity comes from the API, so the front end holds no band rules.
function getVisibleQuakes() {
  if (activeFilter === "all") {
    return quakes;
  }

  return quakes.filter(function (quake) {
    return quake.severity === activeFilter;
  });
}

function quakeToHTML(quake) {
  let qualityText = quake.quality;

  if (quake.quality !== "best") {
    qualityText = `<strong>${quake.quality}</strong>`;
  }

  return `
    <article class="quake quake-${quake.severity}">
      <h2>${quake.publicID}</h2>
      <p>Magnitude: ${quake.magnitude}</p>
      <p>Depth: ${quake.depth} km</p>
      <p>Locality: ${quake.locality}</p>
      <p>Shaking (MMI): ${quake.mmi}</p>
      <p>Quality: ${qualityText}</p>
      <time datetime="${quake.time}">${formatTime(quake.time)}</time>
    </article>
  `;
}

function renderStats() {
  document.querySelector("#stat-total").textContent = quakes.length;
  document.querySelector("#quake-count").textContent = quakes.length;

  // Math.max of an empty list gives -Infinity, so guard it.
  if (quakes.length === 0) {
    document.querySelector("#stat-strongest").textContent = "-";
    document.querySelector("#stat-maxmmi").textContent = "-";
    return;
  }

  const magnitudes = quakes.map(function (quake) {
    return quake.magnitude;
  });

  const intensities = quakes.map(function (quake) {
    return quake.mmi;
  });

  document.querySelector("#stat-strongest").textContent =
    Math.max(...magnitudes).toFixed(1);

  document.querySelector("#stat-maxmmi").textContent =
    "MMI " + Math.max(...intensities);
}

function renderList() {
  const visible = getVisibleQuakes();
  const listElement = document.querySelector("#quake-list");

  if (visible.length === 0) {
    listElement.innerHTML = `<p class="empty">No earthquakes in this band.</p>`;
    return;
  }

  listElement.innerHTML = visible.map(quakeToHTML).join("");
}

function showStatus(message, isError) {
  const statusElement = document.querySelector("#status");

  // textContent, not innerHTML: this text is never treated as HTML.
  statusElement.textContent = message;
  statusElement.classList.toggle("status-error", isError === true);
  statusElement.hidden = false;
}

function hideStatus() {
  document.querySelector("#status").hidden = true;
}

async function loadQuakes() {
  showStatus("Loading earthquakes...");

  try {
    const response = await fetch(`${API_BASE}/api/quakes?minMmi=3`);

    if (!response.ok) {
      throw new Error(`The API answered with status ${response.status}`);
    }

    quakes = await response.json();

    hideStatus();
    renderStats();
    renderList();
  } catch (error) {
    console.error(error);

    showStatus(
      "Could not load earthquakes. Is the API running on port 5055?",
      true
    );

    quakes = [];
    renderStats();
    renderList();
  }
}

const filterButtons = document.querySelectorAll(".filter-button");

filterButtons.forEach(function (button) {
  button.addEventListener("click", function () {
    activeFilter = button.dataset.filter;

    filterButtons.forEach(function (otherButton) {
      otherButton.classList.remove("is-active");
    });

    button.classList.add("is-active");

    renderList();
  });
});

document.querySelector("#reload-button").addEventListener("click", loadQuakes);

loadQuakes();
