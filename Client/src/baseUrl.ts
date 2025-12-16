import { TOKEN_KEY, tokenStorage, forceLogout } from "./Token.tsx";
import { AuthClient, BalanceClient, BoardClient, GameClient, UserClient } from "./generated-ts-client.ts";

const isProduction = import.meta.env.PROD;

const prod = "https://mindst-2-commits-server.fly.dev";
const dev = "http://localhost:5211";

const redirectToLogin = () => {
  if (window.location.pathname !== "/login") {
    const next = encodeURIComponent(window.location.pathname + window.location.search);
    window.location.replace(`/login?next=${next}`);
  }
};

const customFetch = async (url: RequestInfo, init?: RequestInit) => {
  const token = tokenStorage.getItem(TOKEN_KEY, null);

  if (token) {
        // Copy of existing init or new object, with copy of existing headers or
        // new object including Bearer token.
    init = {
      ...(init ?? {}),
      headers: {
        ...(init?.headers ?? {}),
        Authorization: `Bearer ${token}`,
      },
    };
  }

  const response = await fetch(url, init);

  if (response.status === 401 || response.status === 403) {
    forceLogout();
    redirectToLogin();
  }

  return response;
};

export const finalUrl = isProduction ? prod : dev;

export const userClient = new UserClient(finalUrl, { fetch: customFetch });

export const boardClient = new BoardClient(finalUrl, { fetch: customFetch });

export const authClient = new AuthClient(finalUrl, { fetch: customFetch });

export const balanceClient = new BalanceClient(finalUrl, { fetch: customFetch })

export const gameClient = new GameClient(finalUrl, { fetch: customFetch })
