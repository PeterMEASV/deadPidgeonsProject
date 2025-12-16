import { atom } from "jotai";
import { atomWithStorage, createJSONStorage } from "jotai/utils";
import { authClient } from "./baseUrl.ts";
import type { User } from "./generated-ts-client.ts";

// Storage key for JWT
export const TOKEN_KEY = "token";
export const tokenStorage = createJSONStorage<string | null>(
    () => sessionStorage,
);

export const tokenAtom = atomWithStorage<string | null>(
    TOKEN_KEY,
    null,
    tokenStorage,
);

// Writable cache atom (this is what components should read for user.id)
export const userInfoAtom = atom<User | null>(null);

// Read-only async atom that fetches when token changes
export const userInfoQueryAtom = atom<Promise<User | null>>(async (get) => {
  const token = get(tokenAtom);
  if (!token) return null;
  return await authClient.getUserInfo();
});