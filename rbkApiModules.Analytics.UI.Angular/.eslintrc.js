module.exports = {
    "env": {
        "browser": true,
        "es6": true,
        "node": true
    },
    "parser": "@typescript-eslint/parser",
    "parserOptions": {
        "sourceType": "module"
    },
    "ignorePatterns": [
        ".eslintrc.js"
    ],
    extends: [
        'eslint:recommended',
        'plugin:@typescript-eslint/recommended',
    ],
    "plugins": [
        "@typescript-eslint",
        "@angular-eslint",
        "import"
    ],
    "rules": {
        "@angular-eslint/component-class-suffix": "error",
        "@angular-eslint/component-selector": "off",
        "@angular-eslint/directive-class-suffix": "error",
        "@angular-eslint/directive-selector": [
            "error",
            {
                "type": "attribute",
                "prefix": "fury",
                "style": "camelCase"
            }
        ],
        "@angular-eslint/no-input-rename": "error",
        "@angular-eslint/no-output-on-prefix": "error",
        "@angular-eslint/no-output-rename": "error",
        "@angular-eslint/use-pipe-transform-interface": "error",
        "@angular-eslint/use-lifecycle-interface": "error",
        "@typescript-eslint/no-namespace": "off",
        "@typescript-eslint/typedef": [
            "error",
            {
                "parameter": true
            }
        ],
        "@typescript-eslint/explicit-module-boundary-types": "error",
        "@typescript-eslint/explicit-function-return-type": [
            "error",
            {
                "allowConciseArrowFunctionExpressionsStartingWithVoid": true
            }
        ],
        "@typescript-eslint/no-unused-vars": "error",
        "@typescript-eslint/consistent-type-definitions": "error",
        // note you must disable the base rule as it can report incorrect errors
        "dot-notation": "off",
        "@typescript-eslint/dot-notation": "off",
        "@typescript-eslint/explicit-member-accessibility": [
            "error",
            {
                "accessibility": "explicit",
                overrides: {
                    constructors: 'no-public',
                }
            }
        ],
        // note you must disable the base rule as it can report incorrect errors
        "indent": "off",
        "@typescript-eslint/indent": [
            "error",
            2
        ],
        "@typescript-eslint/member-delimiter-style": [
            "error",
            {
                "multiline": {
                    "delimiter": "comma",
                    "requireLast": false
                },
                "singleline": {
                    "delimiter": "comma",
                    "requireLast": false
                },
                "overrides": {
                    "interface": {
                        "multiline": {
                            "delimiter": "semi",
                            "requireLast": true
                        },
                        "singleline": {
                            "delimiter": "semi",
                            "requireLast": true
                        }
                    }
                }
            }
        ],
        "@typescript-eslint/member-ordering": "error",
        "@typescript-eslint/naming-convention": [
            "error",
            {
                "selector": "enum",
                "format": ["PascalCase", "UPPER_CASE"]
            }
        ],
        "@typescript-eslint/no-empty-function": "off",
        "@typescript-eslint/no-empty-interface": "off",
        "@typescript-eslint/no-inferrable-types": [
            "error",
            {
                "ignoreParameters": true
            }
        ],
        "@typescript-eslint/no-misused-new": "error",
        "@typescript-eslint/no-non-null-assertion": "error",
        // note you must disable the base rule as it can report incorrect errors
        "no-unused-expressions": "off",
        "@typescript-eslint/no-unused-expressions": "error",
        "@typescript-eslint/prefer-function-type": "error",
        // note you must disable the base rule as it can report incorrect errors
        "quotes": "off",
        "@typescript-eslint/quotes": [
            "error",
            "single"
        ],
        // note you must disable the base rule as it can report incorrect errors
        "semi": "off",
        "@typescript-eslint/semi": [
            "error",
            "always"
        ],
        "@typescript-eslint/type-annotation-spacing": "error",
        "@typescript-eslint/unified-signatures": "error",
        // note you must disable the base rule as it can report incorrect errors
        "brace-style": "off",
        "@typescript-eslint/brace-style": [
            "error",
            "stroustrup",
            {
                "allowSingleLine": true
            }
        ],
        // note you must disable the base rule as it can report incorrect errors
        "no-useless-constructor": "off",
        "@typescript-eslint/no-useless-constructor": "error",
        // note you must disable the base rule as it can report incorrect errors
        "no-shadow": "off",
        "@typescript-eslint/no-shadow": [
            "error",
            {
                "hoist": "all"
            }
        ],
        "arrow-body-style": "error",
        "constructor-super": "error",
        "curly": [
            "error",
            "multi-line"
        ],
        "eol-last": "off",
        "eqeqeq": [
            "error",
            "always",
            {
                "null": "ignore"
            }
        ],
        "guard-for-in": "error",
        "id-blacklist": "off",
        "id-match": "off",
        "import/no-deprecated": "error",
        "max-len": [
            "error",
            {
                "code": 240
            }
        ],
        "no-bitwise": "error",
        "no-caller": "error",
        "no-console": "error",
        "no-debugger": "error",
        "no-empty": "off",
        "no-eval": "error",
        "no-fallthrough": "error",
        "no-new-wrappers": "error",
        "no-restricted-imports": [
            "error",
            "rxjs/Rx"
        ],
        "no-throw-literal": "error",
        "no-trailing-spaces": "error",
        "no-multi-spaces": "error",
        "no-undef-init": "error",
        "no-underscore-dangle": "error",
        "no-unused-labels": "error",
        "no-var": "error",
        "prefer-const": "error",
        "radix": "error",
        "spaced-comment": [
            "error",
            "always",
            {
                "markers": [
                    "/"
                ]
            }
        ]
    }
};
