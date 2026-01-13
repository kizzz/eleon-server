(function () {
  const doc = typeof document !== 'undefined' ? document : null;
  const win = typeof window !== 'undefined' ? window : null;
  if (!doc || !win) {
    return;
  }

  const classNames = {
    hidden: 'hidden',
  };

  const createSplashElement = () => {
    const splash = doc.createElement('div');
    splash.id = 'splash-screen';
    splash.className = 'splash-screen';
    splash.setAttribute('role', 'status');
    splash.setAttribute('aria-live', 'polite');
    splash.innerHTML = `
      <svg class="splash-spinner" viewBox="0 0 50 50">
        <circle class="path" cx="25" cy="25" r="20" fill="none" stroke-width="5"></circle>
      </svg>
    `;
    return splash;
  };

  const createRetryElement = () => {
    const retry = doc.createElement('div');
    retry.id = 'retry';
    retry.className = 'retry-screen hidden';
    retry.setAttribute('role', 'alert');
    retry.setAttribute('aria-live', 'assertive');
    retry.innerHTML = `
      <div class="retry-panel">
        <p>Auth server is offline, retry later</p>
        <button type="button" data-action="retry">
          Retry
        </button>
      </div>
    `;
    return retry;
  };

  const findRootElement = () => {
    const body = doc.body;
    if (!body) {
      return null;
    }

    const rootCandidate = Array.from(body.children).find((element) => {
      const tag = element.tagName ?? '';
      if (tag === 'NOSCRIPT' || tag === 'SCRIPT') {
        return false;
      }
      return tag.includes('-');
    });

    return rootCandidate ?? null;
  };

  const mountChrome = () => {
    const body = doc.body;
    if (!body) {
      return { splash: null, retry: null, retryButton: null };
    }

    const root = findRootElement();
    const referenceNode = root ?? body.firstChild;

    let splash = doc.getElementById('splash-screen');
    if (!splash) {
      splash = createSplashElement();
      if (referenceNode) {
        body.insertBefore(splash, referenceNode);
      } else {
        body.appendChild(splash);
      }
    }

    let retry = doc.getElementById('retry');
    if (!retry) {
      retry = createRetryElement();
      if (referenceNode) {
        body.insertBefore(retry, referenceNode);
      } else {
        body.appendChild(retry);
      }
    }

    const retryButton =
      retry.querySelector('[data-action="retry"]') ??
      retry.querySelector('button');

    return { splash, retry, retryButton };
  };

  let chromeState = mountChrome();

  const ensureChromeState = () => {
    if (
      !chromeState?.splash?.isConnected ||
      !chromeState?.retry?.isConnected
    ) {
      chromeState = mountChrome();
    }
    return chromeState;
  };

  const getSplash = () => ensureChromeState().splash;
  const getRetry = () => ensureChromeState().retry;
  const getRetryButton = () => {
    const state = ensureChromeState();
    attachRetryHandler(state.retryButton);
    return state.retryButton;
  };

  const hideSplash = () => getSplash()?.classList.add(classNames.hidden);
  const showSplash = () => getSplash()?.classList.remove(classNames.hidden);
  const hideRetry = () => getRetry()?.classList.add(classNames.hidden);
  const showRetry = () => {
    hideSplash();
    getRetry()?.classList.remove(classNames.hidden);
  };

  const attachRetryHandler = (button) => {
    if (!button || button.dataset.retryBound === 'true') {
      return;
    }
    button.dataset.retryBound = 'true';
    button.addEventListener('click', () => {
      hideRetry();
      win.location.reload();
    });
  };

  attachRetryHandler(chromeState.retryButton);

  win.addEventListener('load', () => {
    chromeState = mountChrome();
    win.requestAnimationFrame(() => hideSplash());
  });

  win.addEventListener('eleon:auth-offline', () => {
    chromeState = mountChrome();
    showRetry();
  });

  win.addEventListener('eleon:auth-online', () => {
    hideRetry();
    hideSplash();
  });

  win.eleonHostShell = {
    hideSplash,
    showSplash,
    hideRetry,
    showRetry,
  };
})();
