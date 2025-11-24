const isProduction = import.meta.env.PROD;

const prod = "https://mindst-2-commits-server.fly.dev"
const dev = "http://localhost:5035"

export const finalUrl = isProduction ? prod : dev;
