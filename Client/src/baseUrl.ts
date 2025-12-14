import {AuthClient, BoardClient, UserClient} from "./generated-ts-client.ts";

const isProduction = import.meta.env.PROD;

const prod = "https://mindst-2-commits-server.fly.dev"
const dev = "http://localhost:5211"

export const finalUrl = isProduction ? prod : dev;

export const userClient = new UserClient(finalUrl);

export const boardClient = new BoardClient(finalUrl);

export const authClient = new AuthClient(finalUrl);
