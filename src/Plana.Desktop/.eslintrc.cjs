module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    "airbnb-base",
    "airbnb-typescript/base",
    "plugin:import/typescript",
    "eslint-config-prettier",
  ],
  ignorePatterns: ["dist", ".eslintrc.cjs"],
  parser: "@typescript-eslint/parser",
};
