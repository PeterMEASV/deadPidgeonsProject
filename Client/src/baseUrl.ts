import { TOKEN_KEY, tokenStorage} from "./Token.tsx";
import {AuthClient, BalanceClient, BoardClient, UserClient} from "./generated-ts-client.ts";

const isProduction = import.meta.env.PROD;

const prod = "https://mindst-2-commits-server.fly.dev"
const dev = "http://localhost:5211"

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
    return await fetch(url, init);
};


export const finalUrl = isProduction ? prod : dev;

export const userClient = new UserClient(finalUrl, { fetch: customFetch });

export const boardClient = new BoardClient(finalUrl, { fetch: customFetch });

export const authClient = new AuthClient(finalUrl, { fetch: customFetch });

export const balanceClient = new BalanceClient(finalUrl, { fetch: customFetch })
